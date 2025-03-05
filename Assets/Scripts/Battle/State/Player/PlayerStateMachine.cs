using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachineBase
{
    public PlayerBase Player { get; protected set; }
    protected Fix64 hor, ver;

    public PlayerStateMachine(PlayerBase player) : base()
    {
        Player = player;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        hor = (Fix64)ETCInput.GetAxis("Horizontal");
        ver = (Fix64)ETCInput.GetAxis("Vertical");
    }
}
