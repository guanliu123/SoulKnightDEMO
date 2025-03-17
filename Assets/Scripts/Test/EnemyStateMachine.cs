using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : StateMachineBase
{
    public EnemyBase Enemy { get; protected set; }
    protected Fix64 hor, ver;

    public EnemyStateMachine(EnemyBase enemy) : base()
    {
        Enemy = enemy;
    }
}
