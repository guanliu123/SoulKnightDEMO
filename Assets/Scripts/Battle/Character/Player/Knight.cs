using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class Knight : PlayerBase
{
    public Knight(GameObject obj,PlayerType type) : base(obj,type)
    {
    }

    protected override void OnCharacterStart()
    {
        base.OnCharacterStart();
        stateMachine = new NormalCharacterStateMachine(this );
    }
}
