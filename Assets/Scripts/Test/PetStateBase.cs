
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class PetStateBase :StateBase
{
    protected PetBase pet;
    protected Transform transform => pet.transform;
    protected Animator anim;
    //获取行走路径
    protected Seeker seeker;
    //当前路径
    protected Path path;
    
    public PetStateBase(PetStateMachine machine) : base(machine)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        pet = (Machine as PetStateMachine).Pet;
    }
}
