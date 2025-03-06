using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class Knight : PlayerBase
{
    public Knight(GameObject obj) : base(obj)
    {
    }

    protected override void OnCharacterStart()
    {
        base.OnCharacterStart();
        stateMachine = new NormalCharacterStateMachine(this );
    }
}
