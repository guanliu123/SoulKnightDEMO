using System;
using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;
using HedgehogTeam.EasyTouch;

public class PlayerController : AbstractController
{
    public PlayerBase MainPlayer { get; private set; }

    protected List<PetBase> pets;
    public PlayerController(){}

    protected override void Init()
    {
        base.Init();
        pets = new List<PetBase>();
    }


    protected override void OnAfterRunUpdate()
    {
        base.OnAfterRunUpdate();
        MainPlayer?.GameUpdate();
        foreach (var pet in pets)
        { 
            pet.GameUpdate();
        }
    }

    public void SetMainPlayer(PlayerType type)
    {
        MainPlayer = PlayerFactory.Instance.GetPlayerInScene(type);
        MainPlayer.SetInput(AbstractManager.Instance.GetController<InputController>().input);
    }

    public void AddPlayerPet(PetType type,PlayerBase player)
    {
        pets.Add(PlayerFactory.Instance.GetPetInScene(type,player));
    }
}
