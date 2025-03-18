using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class GoblinMagicStaff : EnemyWeaponBase
{
    public GoblinMagicStaff(GameObject obj, EnemyBase character) : base(obj, character)
    {
    }

    protected override void OnInit()
    {
        data = ItemDataCenter.Instance.GetWeaponData(WeaponType.GoblinMagicStaff);
        base.OnInit();
    }
    
    public override void OnFire()
    {
        base.OnFire();
        BulletFactory.Instance.GetEnemyBullet(BulletType.EnemyBullet5, this);
    }
}
