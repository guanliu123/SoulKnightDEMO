using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : Item
{
    public BulletBase(GameObject obj) : base(obj)
    {
        
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Obstacle")))
        {
            Remove();
            OnHitObstacle();
        }
    }

    protected virtual void OnHitObstacle()
    {
        
    }
}
