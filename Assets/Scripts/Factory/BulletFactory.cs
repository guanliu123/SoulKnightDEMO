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

        bulletObj.transform.localPosition = origin.position;
        bulletObj.transform.localRotation = weapon.RotOrigin.rotation;

        PlayerBulletBase bullet=null;
        switch (type)
        {
            case BulletType.Bullet_1:
                bullet=new Bullet_1(bulletObj,weapon);break;
        }

        bullet?.AddToController();
        return bullet;
    }
    
    public GameObject GetBulletObj(string name)
    {
        var completeName = "Prefabs/Bullets/" + name + ".prefab";
        var objPool = ObjectPoolManager.Instance.GetPool(completeName);
        return objPool.SynSpawn(completeName);
        //return LoadManager.Instance.Load<GameObject>("Prefabs/Weapon/" + name + ".prefab");
    }
}