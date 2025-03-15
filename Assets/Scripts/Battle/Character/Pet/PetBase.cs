using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBase : CharacterBase
{
    //public PetData data { get; protected set; }
    public PetRoot root { get;protected set; }
    protected PetStateMachine stateMachine;
    public PlayerBase Player { get; protected set; }
    public PetBase(GameObject obj,PlayerBase player) : base(obj)
    {
        Player = player;
    }

    protected override void OnInit()
    {
        base.OnInit();
        root = (PetRoot)Root;
        Attribute = new PetAttribute(root.petType);
    }

    protected override void OnCharacterUpdate()
    {
        base.OnCharacterUpdate();
        stateMachine?.GameUpdate();
        if (Vector3.Distance(Player.transform.position, root.transform.position) > 9f)
        {
            ResetPos();
        }
    }

    public void ResetPos()
    {
        transform.position = Player.transform.position+Vector3.back;
    }
}
