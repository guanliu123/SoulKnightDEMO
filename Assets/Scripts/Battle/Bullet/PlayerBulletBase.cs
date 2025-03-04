using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletBase : BulletBase
{
    public PlayerBulletBase(GameObject obj,PlayerWeaponBase holdWeapon) : base(obj)
    {
        
    }

    protected override void OnExit()
    {
        base.OnExit();
        ObjectPoolManager.Instance.GetPool(PoolName).DeSpawn(gameObject,PoolName);
    }
}
