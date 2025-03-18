using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinGuard : EnemyBase
{
    public GoblinGuard(GameObject obj) : base(obj)
    {
    }
    
    protected override void OnInit()
    {
        base.OnInit();
        stateMachine = new NormalEnemyStateMachine(this);
    }
}