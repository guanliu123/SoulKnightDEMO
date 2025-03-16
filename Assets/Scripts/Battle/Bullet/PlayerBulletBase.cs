using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class PlayerBulletBase : BulletBase
{
    public PlayerBulletBase(GameObject obj,PlayerWeaponBase holdWeapon) : base(obj,holdWeapon.data.Damage)
    {
        
    }

    protected override void OnInit()
    {
        base.OnInit();
        
        TriggerManager.Instance.RegisterObserver(TriggerType.TriggerEnter,gameObject,"Enemy",ColliderEnemyEvent);
    }

    protected override void OnExit()
    {
        base.OnExit();
        TriggerManager.Instance.RemoveObserver(TriggerType.TriggerEnter,gameObject);
    }

    private void ColliderEnemyEvent(GameObject obj)
    {
        Remove();
        OnHitEnemy(GetRoot(obj));
    }

    protected virtual void OnHitEnemy(EnemyBase enemy)
    {
        
    }

    protected EnemyBase GetRoot(GameObject obj)
    {
        if (obj.transform.parent == null)
        {
            return obj.GetComponent<EnemyRoot>()?.Character as EnemyBase;
        }
        return obj.transform.parent.GetComponent<EnemyRoot>()?.Character as EnemyBase;
    }
}
