using EnumCenter;
using UnityEngine;

public class WeaponFactory:SingletonBase<WeaponFactory>
{
    /// <summary>
    /// 通过Type生成WeaponBase
    /// </summary>
    /// <param name="type">枪支类型</param>
    /// <param name="character">持有角色</param>
    /// <returns></returns>
    public PlayerWeaponBase GetPlayerWeapon(WeaponType type, PlayerBase character)
    {
        Transform origin = character.root.GetWeaponOriginPoint();
        GameObject weaponObj = GameObject.Instantiate(GetWeaponObj(type.ToString()),origin);
        weaponObj.name = type.ToString();
        weaponObj.transform.localPosition=Vector3.zero;

        return CreateWeapon(type,character,weaponObj);
    }

    /// <summary>
    /// 从场景中捡到枪支，为枪支生成WeaponBase脚本
    /// </summary>
    /// <param name="weapon">枪支物体</param>
    /// <param name="character">持有角色</param>
    /// <returns></returns>
    public PlayerWeaponBase GetPlayerWeaponInScene(GameObject weaponObj,PlayerBase character)
    {
        WeaponRoot root = weaponObj.GetComponent<WeaponRoot>();
        if (!root)
        {
            LogTool.LogError("场景中捡到的枪支未挂载WeaponRoot！");
            return null;
        }
        
        weaponObj.transform.localRotation = character.transform.rotation;

        Transform origin = character.root.GetWeaponOriginPoint();
        weaponObj.transform.SetParent(origin);
        weaponObj.transform.localPosition=Vector3.zero;
        
        return CreateWeapon(root.weaponType,character,weaponObj);
    }

    private PlayerWeaponBase CreateWeapon(WeaponType type, PlayerBase character,GameObject weaponObj)
    {
        PlayerWeaponBase weapon=null;
        switch (type)
        {
            case WeaponType.BadPistol:
                weapon=new BadPistol(weaponObj,character);
                break;
            case WeaponType.AK47:
                weapon = new AK47(weaponObj, character);
                break;
            case WeaponType.DoubleBladeSword:
                weapon = new DoubleBladeSword(weaponObj, character);
                break;
            case WeaponType.BlueFireGatling:
                weapon = new BlueFireGatling(weaponObj, character);
                break;
        }

        return weapon;
    }
    
    public GameObject GetWeaponObj(string name)
    {
        return LoadManager.Instance.Load<GameObject>(ResourcePath.Weapon + name + ".prefab");
    }
    
    /// <summary>
    /// 通过Type生成WeaponBase
    /// </summary>
    /// <param name="type">枪支类型</param>
    /// <param name="character">持有角色</param>
    /// <returns></returns>
    public EnemyWeaponBase GetEnemyWeapon(WeaponType type, EnemyBase enemy)
    {
        Transform origin = enemy.root.GetWeaponOriginPoint();
        GameObject weaponObj = GameObject.Instantiate(GetWeaponObj(type.ToString()),origin);
        weaponObj.name = type.ToString();
        weaponObj.transform.localPosition=Vector3.zero;

        return CreateWeapon(type,enemy,weaponObj);
    }
    
    private EnemyWeaponBase CreateWeapon(WeaponType type, EnemyBase enemy,GameObject weaponObj)
    {
        EnemyWeaponBase weapon=null;
        switch (type)
        {
            case WeaponType.Blowpipe:
                weapon=new Blowpipe(weaponObj,enemy);
                break;
            case WeaponType.Pike:
                weapon = new Pike(weaponObj,enemy); 
                break;
            case WeaponType.GoblinMagicStaff:
                weapon = new GoblinMagicStaff(weaponObj, enemy); 
                break;
        }

        return weapon;
    }
}