using System.Collections.Generic;
using System.Collections.Generic;
using OPH.Collision.QuadTree;
using UnityEditor;
using UnityEngine;

using UnityEngine.Pool;

static class ListPool<T>
{
    private static readonly ObjectPool<List<T>> _pool = new(
        () => new List<T>(64),
        list => list.Clear(),
        list => list.Clear()
    );

    public static List<T> Get() => _pool.Get();
    public static void Release(List<T> list) => _pool.Release(list);
}

[ExecuteInEditMode]
public class RectCollider : MonoBehaviour, IRect
{
    [SerializeField] private Vector2 _size = Vector2.one;
    
    private Vector3 _lastPosition;
    private Vector2 _lastSize;
    private static QTree<IRect> _quadTree;
    private bool _isInTree;
    
    public float X
    {
        get => transform.position.x;
        set => transform.position = new Vector3(value, Y, transform.position.z);
    }

    public float Y
    {
        get => transform.position.y;
        set => transform.position = new Vector3(X, value, transform.position.z);
    }

    public float Width
    {
        get => _size.x;
        set => _size.x = value;
    }

    public float Height
    {
        get => _size.y;
        set => _size.y = value;
    }

    // 碰撞检测管理系统（参考四叉树优化思路）
    private static HashSet<IRect> _allColliders = new();
    /*void OnEnable() => _allColliders.Add(this);
    void OnDisable() => _allColliders.Remove(this);*/
    
    void OnEnable()
    {
        // 延迟四叉树初始化
        if (_quadTree == null)
        {
            const int worldSize = 150; // 根据场景实际大小设置
            _quadTree = QTree<IRect>.CreateRoot()
                .InitRect(0, 0, worldSize * 2, worldSize * 2);
        }
    
        // 延迟插入四叉树
        if (!_isInTree)
        {
            _quadTree.Insert(this);
            _isInTree = true;
        }
    }

    void OnDisable()
    {
        if (_isInTree)
        {
            _quadTree.Remove(this);
            _isInTree = false;
        }
    }

    void Update()
    {
        /*foreach (var other in _allColliders)
        {
            if (other != this && CheckCollision(this, other))
            {
                // 碰撞事件处理
                Debug.Log($"{name} collided with {((MonoBehaviour)other).name}");
            }
        }*/
        // 检测位置变化
        if (transform.position != _lastPosition || _size != _lastSize)
        {
            UpdateQuadTreePosition();
            _lastPosition = transform.position;
            _lastSize = _size;
        }
        // 使用四叉树查询潜在碰撞对象（泛型队列重用优化）
        var candidates = new List<IRect>();
        _quadTree.GetAroundObj(this, candidates);

        foreach (var other in candidates)
        {
            // 防止重复检测（ID比较优化）
            /*if (GetInstanceID() < ((MonoBehaviour)other).GetInstanceID())
            {
                if (CheckCollision(this, other))
                {
                    // 触发碰撞事件
                    Debug.Log($"Collision: {name} & {((MonoBehaviour)other).name}");
                }
            }*/
            if (CheckCollision(this, other))
            {
                // 触发碰撞事件
                Debug.Log($"Collision: {name} & {((MonoBehaviour)other).name}");
            }
        }
        
         // 正确释放到对象池
        ListPool<IRect>.Release(candidates);
    }
    
    // 添加动态更新四叉树逻辑（在位置/尺寸变化时触发）
    void UpdateQuadTreePosition()
    {
        if (_isInTree)
        {
            _quadTree.Remove(this);
            _quadTree.Insert(this);
        }
    }

// 添加编辑器触发更新（同时处理撤销操作）
#if UNITY_EDITOR
    void OnValidate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        if (_isInTree)
        {
            _quadTree.Remove(this);
            _quadTree.Insert(this);
        }
    }
#endif

    // 基于轴对齐矩形相交算法 ([cxyzjd.com](https://www.cxyzjd.com/article/zouxin_88/100831313))
    bool CheckCollision(IRect a, IRect b)
    {
        float dx = Mathf.Abs(a.X - b.X);
        float dy = Mathf.Abs(a.Y - b.Y);
        return dx <= (a.Width + b.Width)/2 && dy <= (a.Height + b.Height)/2;
    }

    // 编辑器绘制 ([testerhome.com](https://testerhome.com/topics/31659))
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 size = new Vector3(Width, Height, 0.1f);
        Gizmos.DrawWireCube(transform.position, size);
    }
}