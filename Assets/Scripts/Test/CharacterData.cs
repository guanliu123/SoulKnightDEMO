//character包括主角、敌人、宠物、随从等，这个类是所有character都具备的基础属性比如生命值、移动速度
using System.Collections.Generic;
using cfg;
using EnumCenter;
using UnityEngine.Video;

public class CharacterDataBase
{
    public int ID;
    public string Name;

    public int HP;
    public float Speed;
}

public class PlayerData : CharacterDataBase
{
    public int MP;
    public int Shield;

    public void UpdateData(Player config)
    {
        ID = config.Id;
        Name = config.Name;
        HP = config.Hp;
        MP = config.Mp;
        Shield = config.Shield;
        Speed = config.Speed;
    }
}

//宠物的攻击力是自身决定的，角色等都是由持有的武器决定
public class PetData : CharacterDataBase
{
    public int Atk;
}

public class CharacterDataCenter:SingletonBase<CharacterDataCenter>
{
    private Dictionary<int,PlayerData> playerDatas;
    private Dictionary<int, PetData> petDatas;

    public override void Init()
    {
        base.Init();
        playerDatas = new();
        petDatas = new();
        InitPlayerData();
    }

    public void InitPlayerData()
    {
        var config = TableManager.Instance.Tables.TBPlayer.DataList;
        foreach (var item in config)
        {
            PlayerData t = new();
            t.UpdateData(item);
            playerDatas.TryAdd(item.Id, t);
        }
    }

    public PlayerData GetPlayerData(PlayerType type)
    {
        if(!playerDatas.TryGetValue((int)type,out var value))
        {
            LogTool.LogError(type.ToString()+"数据不存在！");
        }
        
        return value;
    }

    public PetData GetPetData(PetType type)
    {
        if(!petDatas.TryGetValue((int)type,out var value))
        {
            LogTool.LogError(type.ToString()+"数据不存在！");
        }
        
        return value;
    }
}
