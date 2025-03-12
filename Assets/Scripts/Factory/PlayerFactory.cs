using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class PlayerFactory : SingletonBase<PlayerFactory>
{
    //动态创建玩家
    public PlayerBase GetPlayer(PlayerType type)
    {
        //玩家不会有特别多所以无需用对象池复用
        GameObject playerObj = GameObject.Instantiate(LoadManager.Instance.Load<GameObject>(ResourcePath.Player+type.ToString()+".prefab"));
        
        return CreatePlayer(type, playerObj);
    }
    
    public PlayerBase GetPlayerInScene(PlayerType type)
    {
        GameObject obj = GameObject.Find(type.ToString());

        return CreatePlayer(type,obj);
    }

    private PlayerBase CreatePlayer(PlayerType type, GameObject obj)
    {
        PlayerBase player = null;

        switch (type)
        {
            case PlayerType.Knight: player = new Knight(obj);
                break;
            case PlayerType.Rogue: player = new Rogue(obj);
                break;
            default: break;
        }

        //玩家不要销毁
        obj.transform.SetParent(null);
        GameObject.DontDestroyOnLoad(obj);
        
        return player;
    }

    public PetBase GetPetInScene(PetType type,PlayerBase player)
    {
        GameObject obj=GameObject.Find(type.ToString());

        return CreatePet(type,player,obj);
    }

    public PetBase GetPet(PetType type,PlayerBase player)
    {
        GameObject petObj = GameObject.Instantiate(LoadManager.Instance.Load<GameObject>(ResourcePath.Pet+type.ToString()+".prefab"));

        return CreatePet(type,player,petObj);
    }

    private PetBase CreatePet(PetType type, PlayerBase player,GameObject obj)
    {
        PetBase pet = null;
        switch (type)
        {
            case PetType.LittleCool: pet = new LittleCool(obj,player);
                break;
        }
        
        return pet;
    }
}
