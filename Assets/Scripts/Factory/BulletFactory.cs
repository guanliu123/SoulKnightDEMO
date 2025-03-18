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
        GameObject bulletObj = GetBulletObj(type.ToString());

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
    
    public GameObject GetBulletObj(string name)
    {
        var completeName = GetPoolName(name);
        var objPool = ObjectPoolManager.Instance.GetPool(completeName);
        return objPool.SynSpawn(completeName);
    }

    public string GetPoolName(string name)
    {
        return ResourcePath.Bullet + name + ".prefab";
    }
    
    /// <summary>
    /// 通过Type生成WeaponBase
    /// </summary>
    /// <param name="type">枪支类型</param>
    /// <param name="character">持有角色</param>
    /// <returns></returns>
    public EnemyBulletBase GetEnemyBullet(BulletType type, EnemyWeaponBase weapon,Transform parent=null)
    {
        Transform origin;
        if (parent == null)
        {
            origin = weapon.Root.GetFirePoint();
        }
        else
        {
            origin=parent;
        }
        GameObject bulletObj = GetBulletObj(type.ToString());

        EnemyBulletBase bullet=null;
        switch (type)
        {
            case BulletType.EnemyBullet1:
                bullet = new EnemyBullet1(bulletObj,weapon);
                break;
            case BulletType.EnemyBullet5: 
                bullet=new EnemyBullet5(bulletObj,weapon); 
                break;
            case BulletType.EnemyBullet3: 
                bullet=new EnemyBullet3(bulletObj,weapon); 
                break;
        }

        bullet?.SetPoolName(GetPoolName(type.ToString()));
        bullet?.SetPosition(origin.position);
        bullet?.SetRotation(weapon==null?parent.rotation: weapon.RotOrigin.rotation);
        bullet?.AddToController();
        return bullet;
    }
}