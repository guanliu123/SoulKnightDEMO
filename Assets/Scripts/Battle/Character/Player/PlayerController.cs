using System;
using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;
using HedgehogTeam.EasyTouch;

public class PlayerController : AbstractController
{
    public PlayerBase MainPlayer { get; private set; }
    public PlayerController(){}
    

    protected override void OnAfterRunUpdate()
    {
        base.OnAfterRunUpdate();
        MainPlayer?.OnUpdate();
    }

    public void SetMainPlayer(PlayerType type)
    {
        MainPlayer = PlayerFactory.Instance.GetPlayer(type);
        MainPlayer.SetInput(AbstractManager.Instance.GetController<InputController>().input);
    }
}
