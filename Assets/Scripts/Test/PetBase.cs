using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBase : CharacterBase
{
    public PetData data { get; protected set; }
    protected PetStateMachine stateMachine;
    public PlayerBase Player { get; protected set; }
    public PetBase(GameObject obj,PlayerBase player) : base(obj)
    {
        Player = player;
    }

    protected override void OnInit()
    {
        base.OnInit();
    }

    protected override void OnCharacterUpdate()
    {
        base.OnCharacterUpdate();
        stateMachine?.GameUpdate();
    }
}
