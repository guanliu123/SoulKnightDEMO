using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightStateMachine : PlayerStateMachine
{
    public KnightStateMachine(PlayerBase player) : base(player)
    {
        
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (currentState is NormalCharacterIdleState)
        {
            if (hor != (Fix64)0 || ver != (Fix64)0)
            {
                ChangeState<NormalCharacterWalkState>();
            }
        }
        else if (currentState is NormalCharacterWalkState)
        {
            if (hor == (Fix64)0 || ver == (Fix64)0)
            {
                ChangeState<NormalCharacterIdleState>();
            }
        }
    }
}
