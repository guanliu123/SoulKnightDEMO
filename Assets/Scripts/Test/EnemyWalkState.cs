using System.Collections.Generic;
using UnityEngine;

public class EnemyWalkState : EnemyStateBase
{
    private PlayerBase player;
    private BoidsCalculator boids = new BoidsCalculator();
    private List<EnemyBase> allEnemies; // 需从管理器获取

    public EnemyWalkState(EnemyStateMachine machine) : base(machine)
    {
        
    }

    public override void OnEnter()
    {
        base.OnEnter();
        player = AbstractManager.Instance.GetController<PlayerController>().MainPlayer;
        allEnemies = AbstractManager.Instance.GetController<EnemyController>().enemysInRoom;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        GetBoidsMove();
    }

    private void GetBoidsMove()
    {
        // 获取所有敌人实例（建议通过管理器缓存）
        var boidsDir = boids.CalculateBoidsMove(enemy, allEnemies);
        
        // 混合原始输入和Boids方向
        Vector2 finalDir = Vector2.Lerp(enemy.Velocity.ToVector2(), boidsDir, 0.7f);
        
        if (finalDir.magnitude > 0)
        {
            enemy.transform.position += (Vector3)finalDir * 
                                            enemy.Attribute.Speed * Time.deltaTime;
            
                enemy.IsLeft = finalDir.x < 0;
        }
    }
}