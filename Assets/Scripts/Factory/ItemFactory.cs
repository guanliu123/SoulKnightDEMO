using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class ItemFactory : SingletonBase<ItemFactory>
{
    public PopupNum GetPopupNum(Vector2 pos)
    {
        string name = GetItemPoolName("PopupNum");
        GameObject obj = GetItemObj(name);
        var pop = new PopupNum(obj);
        
        pop?.SetPoolName(name);
        pop?.SetPosition(pos);
        pop?.AddToController();
        return pop;
    }
    public Item GetEffect(EffectType type, Vector2 pos)
    {
        string name = GetEffectPoolName(type.ToString());
        GameObject obj = GetItemObj(name);
        Item effect = null;
        switch (type)
        {
            case EffectType.EffectBoom:
                effect = new EffectBoom(obj);
                break;
            case EffectType.Pane:
                effect = new EffectPane(obj); break;
        }
        
        effect?.SetPoolName(GetEffectPoolName(type.ToString()));
        effect?.SetPosition(pos);
        effect?.AddToController();
        return effect;
    }

    private GameObject GetItemObj(string name)
    {
        //var completeName = GetEffectPoolName(name);
        var objPool = ObjectPoolManager.Instance.GetPool(name);
        return objPool.SynSpawn(name);
    }

    private string GetEffectPoolName(string name)
    {
        return ResourcePath.Effect + name + ".prefab";
    }

    private string GetItemPoolName(string name)
    {
        return ResourcePath.Item + name + ".prefab";
    }
}
