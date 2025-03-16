using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class BulletBase : Item
{
    protected TriggerDetection detection;
    protected RectCollider rectCollider;
    public BulletBase(GameObject obj) : base(obj)
    {
        
    }

    protected override void OnInit()
    {
        base.OnInit();
        rectCollider=gameObject.GetComponent<RectCollider>();
        rectCollider.EnableCollision();
        //detection = gameObject.GetComponent<TriggerDetection>();
        TriggerManager.Instance.RegisterObserver(TriggerType.TriggerEnter,gameObject,"Obstacles",ColliderObstacleEvent);
        /*if (detection)
        {
            detection.AddTriggerListener(TriggerType.TriggerEnter,"Obstacles", (obj) =>
            {
                Remove();
                OnHitObstacle();
            });
        }*/
    }

    private void ColliderObstacleEvent(GameObject obj)
    {
        Remove();
        OnHitObstacle();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        transform.position += transform.right * 30 * Time.deltaTime;
    }

    protected override void OnExit()
    {
        base.OnExit();
        //detection.OnExit();
        rectCollider.DisableCollision();
        TriggerManager.Instance.RemoveObserver(TriggerType.TriggerEnter,gameObject);
        ObjectPoolManager.Instance.GetPool(PoolName).DeSpawn(gameObject,PoolName);
    }

    protected virtual void OnHitObstacle()
    {
        
    }
}
