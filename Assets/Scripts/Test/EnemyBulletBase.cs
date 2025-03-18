using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class EnemyBulletBase : BulletBase
{
public EnemyBulletBase(GameObject obj,EnemyWeaponBase holdWeapon) : base(obj,holdWeapon==null?0:holdWeapon.data.Damage)
{
        
}

protected override void OnInit()
{
    base.OnInit();
        
    TriggerManager.Instance.RegisterObserver(TriggerType.TriggerEnter,gameObject,"Player",ColliderPlayerEvent);
}

protected override void OnExit()
{
    base.OnExit();
    TriggerManager.Instance.RemoveObserver(TriggerType.TriggerEnter,gameObject);
}

private void ColliderPlayerEvent(GameObject obj)
{
    Remove();
    OnHitPlayer(GetRoot(obj));
}

protected virtual void OnHitPlayer(PlayerBase player)
{
        
}

protected PlayerBase GetRoot(GameObject obj)
{
    if (obj.transform.parent == null)
    {
        return obj.GetComponent<PlayerRoot>()?.Character as PlayerBase;
    }
    return obj.transform.parent.GetComponent<PlayerRoot>()?.Character as PlayerBase;
}
}