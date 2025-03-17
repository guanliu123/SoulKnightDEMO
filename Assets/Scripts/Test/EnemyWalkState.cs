using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyWalkState : EnemyStateBase
{
    private BoidsCalculator boids = new BoidsCalculator();
    private List<EnemyBase> allEnemies; // 需从管理器获取
    private QuadTreeSystem quadTreeSystem;
     private AvoidanceCalculator avoidanceCalculator;

    public EnemyWalkState(EnemyStateMachine machine) : base(machine)
    {
        
    }

    public override void OnEnter()
    {
        base.OnEnter();
        allEnemies = AbstractManager.Instance.GetController<EnemyController>().enemysInRoom;
        quadTreeSystem = AbstractManager.Instance.GetSystem<QuadTreeSystem>();
        avoidanceCalculator = new AvoidanceCalculator(quadTreeSystem.QuadTree);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        GetBoidsMove();
    }

    private void GetBoidsMove()
    {
        // 获取所有敌人实例（建议通过管理器缓存）
        //var boidsDir = boids.CalculateBoidsMove(enemy, allEnemies);
        
        // 混合原始输入和Boids方向
        //Vector2 finalDir = Vector2.Lerp(enemy.Velocity.ToVector2(), boidsDir, 0.7f);
        var finalDir = UpdateMovement();
        if (finalDir.magnitude > 0)
        {
            enemy.transform.position += (Vector3)finalDir * 
                                            enemy.Attribute.Speed * Time.deltaTime;
            
                enemy.IsLeft = finalDir.x < 0;
        }
    }
    
    protected Vector2 UpdateMovement()
    {
        Vector2 baseDir = boids.CalculateBoidsMove(enemy, allEnemies);
        Vector2 avoidDir = CalculateAvoidance();

        var att = (EnemyAttribute)(enemy.Attribute);
        float aspeed = att.AvoidanceSpeedMultiplier;

        // 方向合成（保留原始速度的50%）
        Vector2 finalDir = Vector2.Lerp(baseDir, avoidDir*aspeed , 0.5f);
        return finalDir;
    }

    private Vector2 CalculateAvoidance()
    {
        // 使用优化后的四叉树查询
        var obstacles = quadTreeSystem.QueryCircle(
            enemy.transform.position, 
              3f
        ).OfType<RectCollider>().Where(c => c.IsObstacle).ToList();

        return avoidanceCalculator.Calculate(enemy.rectCollider, obstacles);
    }
}