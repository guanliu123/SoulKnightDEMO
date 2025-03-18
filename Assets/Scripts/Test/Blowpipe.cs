using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class Blowpipe : EnemyWeaponBase
{
    public Blowpipe(GameObject obj, EnemyBase character) : base(obj, character)
    {
    }

    protected override void OnInit()
    {
        data = ItemDataCenter.Instance.GetWeaponData(WeaponType.Blowpipe);
        base.OnInit();
    }

    public override void OnFire()
    {
        base.OnFire();
        BulletFactory.Instance.GetEnemyBullet(BulletType.EnemyBullet3, this);
    }
}
