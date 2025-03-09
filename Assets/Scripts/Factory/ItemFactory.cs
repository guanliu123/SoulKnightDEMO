using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class ItemFactory : SingletonBase<ItemFactory>
{
    public Item GetEffect(EffectType type, Vector2 pos)
    {
        GameObject obj = GetEffectObj(type.ToString());
        Item effect = null;
        switch (type)
        {
            case EffectType.EffectBoom:
                effect = new EffectBoom(obj);
                break;
        }
        
        effect?.SetPoolName(GetEffectPoolName(type.ToString()));
        effect?.SetPosition(pos);
        effect?.AddToController();
        return effect;
    }

    private GameObject GetEffectObj(string name)
    {
        var completeName = GetEffectPoolName(name);
        var objPool = ObjectPoolManager.Instance.GetPool(completeName);
        return objPool.SynSpawn(completeName);
    }

    private string GetEffectPoolName(string name)
    {
        return ResourcePath.Effect + name + ".prefab";
    }
}
