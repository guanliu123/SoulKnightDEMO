using System.Collections.Generic;
using cfg;
using EnumCenter;

public class ItemDataBase
{
    public int ID;
    public string Name;
}

public class WeaponData:ItemDataBase
{
    public int Damage;
    public int MC;
    public float FireRate;
    public int Probability;
    public int Price;
    public int MaxRapidCnt;

    public void UpdateData(Weapon data)
    {
        ID = data.Id;
        Name = data.Name;
        Damage=data.Damage;
        MC=data.Mc;
        FireRate=data.Firerate;
        Probability=data.Probability;
        Price=data.Price;
        MaxRapidCnt = data.RapidCnt;
    }
}

public class ItemDataCenter:SingletonBase<ItemDataCenter>
{
    private Dictionary<int,WeaponData> weaponDatas;

    public override void Init()
    {
        base.Init();
        weaponDatas = new();
        
        InitWeaponData();
    }

    public void InitWeaponData()
    {
        var config = TableManager.Instance.Tables.TBWeapon.DataList;
        foreach (var item in config)
        {
            WeaponData t = new();
            t.UpdateData(item);
            weaponDatas.TryAdd(item.Id, t);
        }
    }

    public WeaponData GetWeaponData(WeaponType type)
    {
        if(!weaponDatas.TryGetValue((int)type,out var value))
        {
            LogTool.LogError(type.ToString()+"数据不存在！");
        }
        
        return value;
    }
}
