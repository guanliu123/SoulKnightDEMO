using UnityEngine;
public abstract class Item
{
    //Item基本都是可放入对象池的东西，这里设置poolname
    public string PoolName { get; protected set; }
    protected Vector2 position;
    protected Quaternion rotation;
    public GameObject gameObject { get; protected set; }
    public Transform transform => gameObject.transform;
    public bool IsAlreadyRemove { get; protected set; }

    private bool isInit;
    private bool shouldBeRemoved;

    public Item(GameObject obj)
    {
        gameObject = obj;
    }

    public void AddToController()
    {
        AbstractManager.Instance.GetController<ItemController>().AddItem(this);
    }

    public void GameUpdate()
    {
        if (shouldBeRemoved && !IsAlreadyRemove)
        {
            IsAlreadyRemove = true;
            OnExit();
        }
        else{ OnUpdate();}
    }
    
    protected virtual void OnInit(){}

    public virtual void OnEnter()
    {
        if (!isInit)
        {
            isInit = true;
            OnInit();
        }

        shouldBeRemoved = false;
        IsAlreadyRemove = false;
    }
    protected virtual void OnUpdate(){}
    protected virtual void OnExit(){}

    public void Remove()
    {
        shouldBeRemoved = true;
        IsAlreadyRemove = false;
    }

    public void SetPosition(Vector2 position)
    {
        FixVector2 pos = new FixVector2(position);
        transform.position = pos.ToVector2();
    }

    public void SetRotation(Quaternion rot)
    {
        rotation = rot;
        transform.rotation = rotation;
    }

    public void SetPoolName(string name)
    {
        this.PoolName = name;
    }
}
