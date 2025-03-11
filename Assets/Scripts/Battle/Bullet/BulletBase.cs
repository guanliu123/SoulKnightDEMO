using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class BulletBase : Item
{
    protected TriggerDetection detection;
    public BulletBase(GameObject obj) : base(obj)
    {
        
    }

    protected override void OnInit()
    {
        base.OnInit();
        detection = gameObject.GetComponent<TriggerDetection>();
        if (detection)
        {
            detection.AddTriggerListener(TriggerType.TriggerEnter,"Obstacles", (obj) =>
            {
                Remove();
                OnHitObstacle();
            });
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        transform.position += transform.right * 30 * Time.deltaTime;
    }

    protected override void OnExit()
    {
        base.OnExit();
        ObjectPoolManager.Instance.GetPool(PoolName).DeSpawn(gameObject,PoolName);
    }

    protected virtual void OnHitObstacle()
    {
        
    }
}
