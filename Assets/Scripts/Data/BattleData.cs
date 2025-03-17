//角色属性，迥异于data，储存的是实际的属性，在战斗中可能会有变动

using System.Globalization;
using EnumCenter;
using UnityEngine;

public class CharacterAttribute
{
    public int MaxHp;
    public int CurrentHp;
    public float Speed;
}

public class PlayerAttribute : CharacterAttribute
{
    public int MaxMp;
    public int CurrentMp;

    public PlayerAttribute(PlayerType type)
    {
       var data = CharacterDataCenter.Instance.GetPlayerData(type);
       CurrentHp = MaxHp = data.HP;
       this.CurrentMp = MaxMp = data.MP;
       Speed=data.Speed;
    }
}

public class PetAttribute : CharacterAttribute
{
    public PetAttribute(PetType type)
    {
        var data=CharacterDataCenter.Instance.GetPetData(type);
        CurrentHp=MaxHp=data.HP;
        Speed=data.Speed;
    }
}

public class EnemyAttribute : CharacterAttribute
{
    [Tooltip("避障时的速度补偿")]
    public float AvoidanceSpeedMultiplier = 1.5f; 
    public EnemyAttribute(EnemyType type)
    {
        var data=CharacterDataCenter.Instance.GetEnemyData(type);
        CurrentHp=MaxHp=data.HP;
        Speed=data.Speed;
    }
}