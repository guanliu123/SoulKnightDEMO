using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class Pike : EnemyWeaponBase
{
    public Pike(GameObject obj, EnemyBase character) : base(obj, character)
    {
    }
    
    protected override void OnInit()
    {
        data = ItemDataCenter.Instance.GetWeaponData(WeaponType.Pike);
        base.OnInit();
    }
}