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
}
