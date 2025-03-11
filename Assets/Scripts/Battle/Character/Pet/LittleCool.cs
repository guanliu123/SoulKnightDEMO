using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class LittleCool : PetBase
{
    public LittleCool(GameObject obj,PlayerBase player) : base(obj,player)
    {
    }

    protected override void OnInit()
    {
        base.OnInit();
        stateMachine = new NormalPetStateMachine(this);
    }
}
