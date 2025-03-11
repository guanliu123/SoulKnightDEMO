using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class PlayerBulletBase : BulletBase
{
    public PlayerBulletBase(GameObject obj,PlayerWeaponBase holdWeapon) : base(obj)
    {
        
    }

    protected override void OnInit()
    {
        base.OnInit();
        if (detection)
        {
            detection.AddTriggerListener(TriggerType.TriggerEnter,"Enemy", (obj) =>
            {
                Remove();
                OnHitEnemy(obj.transform.parent.GetComponent<EnemyRoot>()?.Character as EnemyBase);
            });
        }
    }

    protected virtual void OnHitEnemy(EnemyBase enemy)
    {
        
    }
}
