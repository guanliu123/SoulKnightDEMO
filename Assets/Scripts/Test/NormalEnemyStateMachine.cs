using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemyStateMachine : EnemyStateMachine
{
    public NormalEnemyStateMachine(EnemyBase enemy) : base(enemy)
    {
        ChangeState<EnemyTrackState>();
    }
}