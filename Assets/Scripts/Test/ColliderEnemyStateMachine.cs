using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEnemyStateMachine : EnemyStateMachine
{
    private Fix64 changeTimer;
    //多长时间转换
    private Fix64 changeCool;
    public ColliderEnemyStateMachine(EnemyBase enemy) : base(enemy)
    {
        ChangeState<EnemyTrackState>();
        ResetChange();
    }
    
    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (currentState is EnemyTrackState)
        {
            if (changeTimer >= changeCool)
            {
                ChangeState<ColliderAttackState>();
                ResetChange();
            }
            else
            {
                changeTimer += Time.deltaTime;
            }
        }
        else if (currentState is ColliderAttackState)
        {
            var t =currentState as ColliderAttackState;
            if (t.needChange)
            {
                ChangeState<EnemyTrackState>();
            }
        }
    }

    private void ResetChange()
    {
        changeTimer = (Fix64)0;
        changeCool = (Fix64)Random.Range(1,5);
    }
}
