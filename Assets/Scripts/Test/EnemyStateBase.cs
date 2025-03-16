using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateBase : StateBase
{
    protected Animator animator;
    protected EnemyBase enemy;
    
    public new EnemyStateMachine Machine{
        get=>base.Machine as EnemyStateMachine;
        set => base.Machine = value;
    }

    public EnemyStateBase(EnemyStateMachine machine) : base(machine)
    {
        
    }

    public override void OnInit()
    {
        base.OnInit();
        enemy = Machine.Enemy;
        animator = enemy.animator;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }
}
