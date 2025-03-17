using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemyStateMachine : EnemyStateMachine
{
    private Fix64 changeTimer;
    //多长时间转换
    private Fix64 changeCool;
    public NormalEnemyStateMachine(EnemyBase enemy) : base(enemy)
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
                ChangeState<BoarAttackState>();
            }
            else
            {
                changeTimer += Time.deltaTime;
            }
        }
        else if (currentState is BoarAttackState)
        {
            var t =currentState as BoarAttackState;
            if (t.needChange)
            {
                ChangeState<EnemyTrackState>();
                ResetChange();
            }
        }
    }

    private void ResetChange()
    {
        changeTimer = (Fix64)0;
        changeCool = (Fix64)Random.Range(2,8);
    }
}