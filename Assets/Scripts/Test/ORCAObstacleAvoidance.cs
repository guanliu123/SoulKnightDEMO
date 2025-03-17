// 新增ORCA计算模块

using System.Collections.Generic;
using UnityEngine;

public class ORCAObstacleAvoidance
{
    private const float TIME_HORIZON = 1.5f; // 预测时间范围
    
    public Vector2 CalculateORCA(
        EnemyBase agent, 
        IEnumerable<RectCollider> obstacles,
        float agentRadius)
    {
        List<Line> orcaLines = new List<Line>();
        Vector2 agentVel = agent.Velocity.ToVector2();
        
        // 处理静态障碍物
        foreach (var obstacle in obstacles)
        {
            // 将矩形转换为障碍线段
            var obstacleLines = ConvertRectToLines(obstacle);
            foreach (var line in obstacleLines)
            {
                // 计算ORCA速度约束
                var lineConstraint = GetLineConstraint(
                    agent.transform.position, 
                    agentVel,
                    line,
                    agentRadius,
                    TIME_HORIZON);
                
                if (lineConstraint.HasValue)
                {
                    orcaLines.Add(lineConstraint.Value);
                }
            }
        }

        // 求解线性规划问题
        return ORCASolver.Solve(agentVel, orcaLines);
    }

    private List<Line> ConvertRectToLines(RectCollider rect)
    {
        // 将矩形分解为四条边
        Vector2[] corners = new Vector2[4];
        corners[0] = new Vector2(rect.X - rect.Width/2, rect.Y + rect.Height/2); // 左上
        corners[1] = new Vector2(rect.X + rect.Width/2, rect.Y + rect.Height/2); // 右上
        corners[2] = new Vector2(rect.X + rect.Width/2, rect.Y - rect.Height/2); // 右下
        corners[3] = new Vector2(rect.X - rect.Width/2, rect.Y - rect.Height/2); // 左下

        List<Line> lines = new List<Line>();
        for (int i=0; i<4; i++)
        {
            Vector2 start = corners[i];
            Vector2 end = corners[(i+1)%4];
            lines.Add(new Line(start, end));
        }
        return lines;
    }

    private Line? GetLineConstraint(
        Vector2 agentPos,
        Vector2 agentVel,
        Line obstacleEdge,
        float agentRadius,
        float timeHorizon)
    {
        // 步骤1：计算智能体到线段的最短距离点
        Vector2 closestPoint = obstacleEdge.ClosestPoint(agentPos);
        Vector2 relativePos = closestPoint - agentPos;
    
        // 步骤2：考虑智能体半径计算安全距离
        float distance = relativePos.magnitude;
        float minDistance = agentRadius + 0.1f; // 添加安全余量
        if (distance > minDistance) return null; // 超出影响范围时不产生约束

        // 步骤3：计算速度障碍区域
        Vector2 normal = obstacleEdge.Normal.normalized;
        Vector2 u = (relativePos - normal * minDistance) / timeHorizon - agentVel;
    
        // 步骤4：构建ORCA速度约束线
        return new Line
        {
            Start = agentVel + u,
            End = agentVel + u + normal * 10f // 沿法线延伸约束线
        };
    }
}
