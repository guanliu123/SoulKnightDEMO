using EnumCenter;
using UnityEngine;

public class EnemyFactory:SingletonBase<EnemyFactory>
{
    public EnemyBase GetEnemy(EnemyType type)
    {
        GameObject enemyObj=GetEnemyObj(type.ToString());

        return CreateEnemy(type,enemyObj);
    }

    //直接在场景中获取敌人物体，不要Load创建
    public EnemyBase GetEnemyInScene(EnemyType type)
    {
        GameObject enemyObj = GameObject.Find(type.ToString());
        
        return CreateEnemy(type,enemyObj);
    }

    private EnemyBase CreateEnemy(EnemyType type,GameObject obj)
    {
        EnemyBase enemy = null;

        switch (type)
        {
            case EnemyType.Stake:
                enemy = new Stake(obj); break;
        }

        return enemy;
    }
    
    public GameObject GetEnemyObj(string name)
    {
        var completeName = GetPoolName(name);
        var objPool = ObjectPoolManager.Instance.GetPool(completeName);
        return objPool.SynSpawn(completeName);
    }

    public string GetPoolName(string name)
    {
        return ResourcePath.Enemy + name + ".prefab";
    }
}
