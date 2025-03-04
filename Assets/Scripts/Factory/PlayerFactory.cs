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
            case PlayerType.Knight: player = new Knight(obj,type);
                break;
            case PlayerType.Rogue: player = new Rogue(obj,type);
                break;
            default: break;
        }

        return player;
    }
}
