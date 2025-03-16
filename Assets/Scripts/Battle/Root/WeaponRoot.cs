using System;
using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using TMPro;
using UnityEngine;

public class WeaponRoot : InteractiveObjectRoot
{
    public WeaponType weaponType;

    //武器旋转点
    public Transform rotOrigin;
    public Transform firePoint;

    private void Awake()
    {
        Type=InteractiveObjectType.Weapon;
        objName = ItemDataCenter.Instance.GetWeaponData(weaponType).Name;
        var t = itemIndicator.GetComponent<UIContainer>().GetXXX("Text") as TMP_Text;
        t.text = objName;
    }

    public Transform GetFirePoint()
    {
        if (firePoint == null)
        {
            LogTool.LogError($"武器{firePoint.ToString()}上的FirePoint组件未赋值！");
        }

        return firePoint;
    }
    
    public Transform GetRotOrigin()
    {
        if (rotOrigin == null)
        {
            LogTool.LogError($"武器{firePoint.ToString()}上的RotOrigin组件未赋值！");
        }

        return rotOrigin;
    }
}
