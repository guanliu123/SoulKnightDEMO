using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class BoarAttackState : EnemyStateBase
{
    private FixVector2 AttackDir;
    //连续撞墙两次就切换状态
    private int triggerCnt;
    public bool needChange { get; private set; }
    
    public BoarAttackState(EnemyStateMachine machine) : base(machine)
    {
        
    }

    public override void OnEnter()
    {
        base.OnEnter();
        AttackDir = GetAttackDir();
        triggerCnt = 0;
        TriggerManager.Instance.RegisterObserver(TriggerType.TriggerEnter,enemy.gameObject,"Obstacles", (obj) =>
        {
            triggerCnt++;
            AttackDir = GetAttackDir();
        });
    }

    public override void OnExit()
    {
        base.OnExit();
        TriggerManager.Instance.RemoveObserver(TriggerType.TriggerEnter,enemy.gameObject);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (triggerCnt > 2)
        {
            needChange = true;
        }
        enemy.transform.position += (Vector3)AttackDir.ToVector2() *0.5f* Time.deltaTime;
            
        enemy.IsLeft = AttackDir.x < 0;
    }

    private FixVector2 GetAttackDir()
    {
        Fix64 spreadAngle = (Fix64)Random.Range(-20f, 20f);
        FixVector2 rotatedDir = spreadAngle * enemy.Velocity;
        
        return new FixVector2(rotatedDir) * FixVector2.Magnitude(enemy.Velocity);
    }
}
