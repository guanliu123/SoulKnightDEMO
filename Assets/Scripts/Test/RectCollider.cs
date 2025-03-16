using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using OPH.Collision.QuadTree;
using UnityEngine.Pool;

[ExecuteInEditMode]
public class RectCollider : MonoBehaviour, IRect
{
    #region 公共接口
    public float X
    {
        get => _worldPosition.x;
        set
        {
            Vector3 newPos = new Vector3(value, Y, transform.position.z);
            _offset = transform.InverseTransformPoint(newPos);
            MarkDirty();
        }
    }

    public float Y
    {
        get => _worldPosition.y;
        set
        {
            Vector3 newPos = new Vector3(X, value, transform.position.z);
            _offset = transform.InverseTransformPoint(newPos);
            MarkDirty();
        }
    }

    public float Width
    {
        get => _size.x;
        set
        {
            _size.x = Mathf.Max(0.1f, value);
            MarkDirty();
        }
    }

    public float Height
    {
        get => _size.y;
        set
        {
            _size.y = Mathf.Max(0.1f, value);
            MarkDirty();
        }
    }
    #endregion

    #region 序列化字段
    [SerializeField]public Vector2 _offset;
    [SerializeField] Vector2 _size = Vector2.one;
    [SerializeField] bool _drawGizmos = true;
    [SerializeField] Color _gizmoColor = Color.green;
    #endregion

    #region 私有变量
    private Vector2 _worldPosition;
    private Vector3 _lastTransformPosition;
    private Vector2 _lastSize;
    private Vector2 _lastOffset;
    private bool _isDirty;
    private bool _isInTree;
    private static QTree<IRect> _quadTree;
    #endregion

    #region 生命周期方法
    void Awake()
    {
        InitializeQuadTree();
        CacheTransformState();
    }

    void Update()
    {
        CheckTransformChanges();
        UpdateWorldPosition();
        
        if (_quadTree == null) return;

        // 使用对象池获取临时列表（避免GC分配）
        var candidates = ListPool<IRect>.Get();
        _quadTree.GetAroundObj(this, candidates);

        foreach (var other in candidates)
        {
            if (CheckCollision(this, other))
            {
                TriggerManager.Instance.NotisfyObserver(TriggerType.TriggerEnter,gameObject,((MonoBehaviour)other).gameObject);
            }
        }

        // 正确释放到对象池（修正语法错误）
        ListPool<IRect>.Release(candidates);
    }
    
    // 基于轴对齐矩形相交算法 ([cxyzjd.com](https://www.cxyzjd.com/article/zouxin_88/100831313))
    bool CheckCollision(IRect a, IRect b)
    {
        float dx = Mathf.Abs(a.X - b.X);
        float dy = Mathf.Abs(a.Y - b.Y);
        return dx <= (a.Width + b.Width)/2 && dy <= (a.Height + b.Height)/2;
    }

    void OnEnable()
    {
        StartCoroutine(DelayedRegistration());
    }

    void OnDisable()
    {
        RemoveFromQuadTree();
    }

    void OnDestroy()
    {
        RemoveFromQuadTree();
    }
    #endregion

    #region 四叉树管理
    private void InitializeQuadTree()
    {
        if (_quadTree ==null)
        {
            var sceneBounds = CalculateSceneBounds();
            _quadTree=QTree<IRect>.CreateRoot(4,6).InitRect(sceneBounds.center.x,sceneBounds.center.y,sceneBounds.size.x,
                sceneBounds.size.y);
            /*_quadTree = new QTree<IRect>(
                sceneBounds.center.x,
                sceneBounds.center.y,
                sceneBounds.size.x,
                sceneBounds.size.y
            );*/
        }
    }

    private IEnumerator DelayedRegistration()
    {
        yield return new WaitForEndOfFrame();
        AddToQuadTree();
    }

    private void AddToQuadTree()
    {
        if (_quadTree != null && !_isInTree)
        {
            _quadTree.Insert(this);
            _isInTree = true;
        }
    }

    private void RemoveFromQuadTree()
    {
        if (_quadTree != null && _isInTree)
        {
            _quadTree.Remove(this);
            _isInTree = false;
        }
    }

    private void UpdateQuadTree()
    {
        if (_quadTree != null && _isInTree)
        {
            _quadTree.UpdateNode(this);
        }
    }
    #endregion

    #region 状态管理
    private void CacheTransformState()
    {
        _lastTransformPosition = transform.position;
        _lastSize = _size;
        _lastOffset = _offset;
        UpdateWorldPosition();
    }

    private void CheckTransformChanges()
    {
        bool transformChanged = transform.position != _lastTransformPosition;
        bool sizeChanged = _size != _lastSize;
        bool offsetChanged = _offset != _lastOffset;

        if (transformChanged || sizeChanged || offsetChanged)
        {
            MarkDirty();
            CacheTransformState();
        }
    }

    private void UpdateWorldPosition()
    {
        _worldPosition = transform.TransformPoint(_offset);
    }

    private void MarkDirty()
    {
        if (!_isDirty)
        {
            StartCoroutine(DelayedUpdate());
            _isDirty = true;
        }
    }

    private IEnumerator DelayedUpdate()
    {
        yield return new WaitForEndOfFrame();
        UpdateQuadTree();
        _isDirty = false;
    }
    #endregion

    #region 可视化
    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!_drawGizmos) return;

        Gizmos.color = _gizmoColor;
        DrawColliderFrame();
        DrawOffsetHandle();
    }

    private void DrawColliderFrame()
    {
        Vector3 center = new Vector3(X, Y, 0);
        Vector3 size = new Vector3(Width, Height, 0.1f);
        
        Gizmos.DrawWireCube(center, size);
        Gizmos.color *= new Color(1,1,1,0.3f);
        Gizmos.DrawCube(center, size);
    }

    private void DrawOffsetHandle()
    {
        Vector3 handlePos = transform.TransformPoint(_offset);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(handlePos, 0.1f);
    }
    #endif
    #endregion

    #region 工具方法
    private Bounds CalculateSceneBounds()
    {
        var renderers = FindObjectsOfType<Renderer>();
        Bounds bounds = new Bounds();
        foreach (var r in renderers) bounds.Encapsulate(r.bounds);
        return bounds;
    }
    #endregion
}

