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

    public void UpdateData(TCharacterData config)
    {
        //todo:等服务器设计好字段发过来
    }
}

public class EnemyData : CharacterDataBase
{
    public void UpdateData(Enemy config)
    {
        ID = config.Id;
        Name = config.Name;
        HP = config.Hp;
        Speed = config.Speed;
    }
}

//宠物的攻击力是自身决定的，角色等都是由持有的武器决定
public class PetData : CharacterDataBase
{
    public int Atk;
    public void UpdateData(Pet config)
    {
        ID = config.Id;
        Name = config.Name;
        HP = config.Hp;
        Speed = config.Speed;
    }
}

public class CharacterDataCenter:SingletonBase<CharacterDataCenter>
{
    private Dictionary<int,PlayerData> playerDatas;
    private Dictionary<int, PetData> petDatas;
    private Dictionary<int, EnemyData> enemyDatas;


    public override void Init()
    {
        base.Init();
        playerDatas = new();
        petDatas = new();
        enemyDatas = new();
        
        InitPlayerData();
        InitPetData();
        InitEnemyData();
    }

    public void InitPlayerData()
    {
        var config = TableManager.Instance.Tables.TBPlayer.DataList;
        foreach (var item in config)
        {
            PlayerData t = new();

            //如果玩家有这个角色并且有升级后数据就用服务器的，否则用本地的初始配置
            if (UserData.PlayerCharacters.ContainsKey(item.Id))
            {
                t.UpdateData(UserData.PlayerCharacters[item.Id]);
            }
            else
            {
                t.UpdateData(item);
            }
            
            playerDatas.TryAdd(item.Id, t);
        }
    }
    
    public void InitEnemyData()
    {
        var config = TableManager.Instance.Tables.TBEnemy.DataList;
        foreach (var item in config)
        {
            EnemyData t = new();
            t.UpdateData(item);
            enemyDatas.TryAdd(item.Id, t);
        }
    }
    
    public void InitPetData()
    {
        var config = TableManager.Instance.Tables.TBPet.DataList;
        foreach (var item in config)
        {
            PetData t = new();
            t.UpdateData(item);
            petDatas.TryAdd(item.Id, t);
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
    
    public EnemyData GetEnemyData(EnemyType type)
    {
        if(!enemyDatas.TryGetValue((int)type,out var value))
        {
            LogTool.LogError(type.ToString()+"数据不存在！");
        }
        
        return value;
    }
}
