/*
// 参考[ORCA论文](http://gamma.cs.unc.edu/ORCA/)实现

using System.Collections.Generic;
using UnityEngine;

public class ORCASolver
{
    public List<Line> constraints = new List<Line>();

    public Vector2 Solve(Vector2 prefVelocity, float radius, float timeHorizon)
    {
        // 1. 生成速度障碍区域
        foreach(var neighbor in neighbors)
        {
            Vector2 relativeVel = prefVelocity - neighbor.velocity;
            Vector2 center = relativeVel * 0.5f;
            float combinedRadius = radius + neighbor.radius;
            
            // 生成约束线（关键几何计算）
            Vector2 edgeDir = Vector2.Perpendicular(relativeVel).normalized;
            Line constraint = new Line(
                    point: center + edgeDir * combinedRadius,
                    direction: -edgeDir
                );
            constraints.Add(constraint);
        }

        // 2. 线性规划求解最优速度
        return LinearProgramming.Solve(prefVelocity, constraints);
    }
}

// 参考自ORCA论文附录A：https://gamma.cs.unc.edu/ORCA/ 第3章
public struct Line
{
    /// <summary>
    /// 约束线上的点（速度空间坐标系中的点）
    /// </summary>
    public Vector2 point;

    /// <summary>
    /// 约束线的法线方向（指向可行区域）
    /// </summary>
    public Vector2 direction;

    public Line(Vector2 p, Vector2 dir)
    {
        point = p;
        direction = dir.normalized;
    }

    /// <summary>
    /// 判断速度点是否在可行区域
    /// </summary>
    public bool IsValid(Vector2 velocity)
    {
        // 点积判断法线方向投影
        return Vector2.Dot(velocity - point, direction) >= 0;
    }
}
*/
