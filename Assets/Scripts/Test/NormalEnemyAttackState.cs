using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class NormalEnemyAttackState : EnemyStateBase
{
    public bool needChange { get; private set; }
    
    public NormalEnemyAttackState(EnemyStateMachine machine) : base(machine)
    {
        
    }

    public override void OnEnter()
    {
        base.OnEnter();
        needChange = false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        enemy.weapon.OnFire();
        needChange = true;
    }
}
