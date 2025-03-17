using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

/// <summary>
/// 冲撞类敌人AI
/// </summary>
public class ColliderAttackState : EnemyStateBase
{
    private FixVector2 AttackDir;
    
    public bool needChange { get; private set; }
    
    public ColliderAttackState(EnemyStateMachine machine) : base(machine)
    {
        
    }

    public override void OnEnter()
    {
        base.OnEnter();
        AttackDir = GetAttackDir();
        if (enemy is Boar)
        {
            var t = enemy as Boar;
            t.SwitchAttack(true);
        }
        TriggerManager.Instance.RegisterObserver(TriggerType.TriggerEnter,enemy.gameObject,"Obstacles", (obj) =>
        {
            needChange = true;
        });
        TimerManager.Register(3f, () =>
        {
            needChange = true;
        });
    }

    public override void OnExit()
    {
        base.OnExit();
        needChange = false;
        TriggerManager.Instance.RemoveObserver(TriggerType.TriggerEnter,enemy.gameObject);
        if (enemy is Boar)
        {
            var t = enemy as Boar;
            t.SwitchAttack(false);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        enemy.transform.position += (Vector3)AttackDir.ToVector2() *enemy.Attribute.Speed*0.9f* Time.deltaTime;
            
        enemy.IsLeft = AttackDir.x < 0;
    }

    private FixVector2 GetAttackDir()
    {
        /*Fix64 spreadAngle = (Fix64)Random.Range(-20f, 20f);
        FixVector2 rotatedDir = spreadAngle * enemy.Velocity;
        
        return new FixVector2(rotatedDir) * FixVector2.Magnitude(enemy.Velocity);*/
        // 生成随机偏移角度（Fix64需转换）
        float spreadAngle = Random.Range(-20f, 20f);
        float radians = spreadAngle * Mathf.Deg2Rad; // 角度转弧度

// 计算旋转后的方向向量
        var cos = Mathf.Cos(radians);
        var sin = Mathf.Sin(radians);
        FixVector2 rotatedDir = new FixVector2(
            enemy.Velocity.x * cos - enemy.Velocity.y * sin, // 旋转矩阵X分量
            enemy.Velocity.x * sin + enemy.Velocity.y * cos  // 旋转矩阵Y分量
        );
        
        return rotatedDir * FixVector2.Magnitude(enemy.Velocity);
    }
}
