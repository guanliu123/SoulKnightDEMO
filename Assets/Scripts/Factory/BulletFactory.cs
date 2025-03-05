using EnumCenter;
using UnityEngine;

public class BulletFactory:SingletonBase<BulletFactory>
{
    /// <summary>
    /// 通过Type生成WeaponBase
    /// </summary>
    /// <param name="type">枪支类型</param>
    /// <param name="character">持有角色</param>
    /// <returns></returns>
    public PlayerBulletBase GetPlayerBullet(BulletType type, PlayerWeaponBase weapon)
    {
        Transform origin = weapon.Root.GetFirePoint();
        GameObject bulletObj = GetPlayerBulletObj(type.ToString());

        PlayerBulletBase bullet=null;
        switch (type)
        {
            case BulletType.Bullet_1:
                bullet=new Bullet_1(bulletObj,weapon);break;
        }

        bullet?.SetPoolName(GetPoolName(type.ToString()));
        bullet?.SetPosition(origin.position);
        bullet?.SetRotation(weapon.RotOrigin.rotation);
        bullet?.AddToController();
        return bullet;
    }
    
    public GameObject GetPlayerBulletObj(string name)
    {
        var completeName = GetPoolName(name);
        var objPool = ObjectPoolManager.Instance.GetPool(completeName);
        return objPool.SynSpawn(completeName);
    }

    public string GetPoolName(string name)
    {
        return "Prefabs/Bullets/" + name + ".prefab";
    }
}