using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinGiant : EnemyBase
{
    public GoblinGiant(GameObject obj) : base(obj)
    {
    }
    
    protected override void OnInit()
    {
        base.OnInit();
        stateMachine = new NormalEnemyStateMachine(this);
    }
}

