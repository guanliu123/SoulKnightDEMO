using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class PlayerFactory : SingletonBase<PlayerFactory>
{
    public PlayerBase GetPlayer(PlayerType type)
    {
        PlayerBase player = null;

        GameObject obj = GameObject.Find(type.ToString());
        switch (type)
        {
            case PlayerType.Knight: player = new Knight(obj);
                break;
            case PlayerType.Rogue: player = new Rogue(obj);
                break;
            default: break;
        }

        return player;
    }

    public PetBase GetPet(PetType type)
    {
        // GameObject obj =
        //     GameObject.Instantiate(
        //         LoadManager.Instance.Load<GameObject>("Prefabs/Character" + type.ToString() + ".prefab"));
        
        GameObject obj=GameObject.Find(type.ToString());
        PetBase pet = null;
        switch (type)
        {
            case PetType.LittleCool: pet = new LittleCool(obj);
                break;
        }

        return pet;
    }
}
