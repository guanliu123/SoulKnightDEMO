using UnityEngine;
using System;

/// <summary>
/// 线段几何计算工具类（基于Unity坐标系）
/// 参考《计算几何算法与实现》第3章线段处理
/// </summary>
public struct Line
{
    public Vector2 Start;
    public Vector2 End;
    public Vector2 Direction => End - Start;
    public Vector2 Normal => GetLeftNormal();

    public Line(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    /// 获取线段左侧法线（面向线段终点方向）
    /// </summary>
    private Vector2 GetLeftNormal()
    {
        Vector2 dir = Direction.normalized;
        return new Vector2(-dir.y, dir.x); // 逆时针旋转90度
    }

    /// <summary>
    /// 判断点在线段的哪一侧（基于叉积）
    /// 返回值：正数在左侧，负数在右侧，0在线上
    /// </summary>
    public float PointSide(Vector2 point)
    {
        Vector2 a = point - Start;
        Vector2 b = End - Start;
        return a.x * b.y - a.y * b.x; // 二维叉积
    }

    /// <summary>
    /// 计算点到线段的最近点
    /// 参考《Real-Time Collision Detection》5.1.2节
    /// </summary>
    public Vector2 ClosestPoint(Vector2 point)
    {
        Vector2 ab = End - Start;
        float t = Vector2.Dot(point - Start, ab) / ab.sqrMagnitude;

        if (t < 0.0f) return Start;
        if (t > 1.0f) return End;
        return Start + t * ab;
    }

    /// <summary>
    /// 生成ORCA速度约束线（基于智能体半径和时间范围）
    /// 参考《Reciprocal n-body Collision Avoidance》算法1
    /// </summary>
    public void GenerateORCAConstraint(
        Vector2 agentPos,
        Vector2 agentVel,
        float radius,
        float timeHorizon,
        out Vector2 constraintDir,
        out Vector2 constraintPoint)
    {
        // 计算最近点相对位置
        Vector2 closest = ClosestPoint(agentPos);
        Vector2 relativePos = closest - agentPos;

        // 计算速度障碍区域
        float dist = relativePos.magnitude;
        float combinedRadius = radius + 0.1f; // 添加安全阈值
        float timeToCollision = Mathf.Max(dist - combinedRadius, 0) / agentVel.magnitude;

        // 构建约束方向
        Vector2 u = (relativePos / timeHorizon) - agentVel;
        constraintDir = u.normalized;
        constraintPoint = closest + constraintDir * combinedRadius;
    }

    /// <summary>
    /// 线段相交检测（使用快速排斥+跨立实验）
    /// 参考《计算机图形学几何工具算法详解》第5章
    /// </summary>
    public static bool SegmentsIntersect(Line a, Line b)
    {
        // 快速排斥实验
        if (Mathf.Max(a.Start.x, a.End.x) < Mathf.Min(b.Start.x, b.End.x) ||
            Mathf.Max(b.Start.x, b.End.x) < Mathf.Min(a.Start.x, a.End.x) ||
            Mathf.Max(a.Start.y, a.End.y) < Mathf.Min(b.Start.y, b.End.y) ||
            Mathf.Max(b.Start.y, b.End.y) < Mathf.Min(a.Start.y, a.End.y))
            return false;

        // 跨立实验
        float c1 = Cross(b.Start - a.Start, a.End - a.Start);
        float c2 = Cross(b.End - a.Start, a.End - a.Start);
        float c3 = Cross(a.Start - b.Start, b.End - b.Start);
        float c4 = Cross(a.End - b.Start, b.End - b.Start);

        return (c1 * c2 <= 0) && (c3 * c4 <= 0);
    }

    private static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    /// <summary>
    /// 计算线段长度
    /// </summary>
    public float Length()
    {
        return Vector2.Distance(Start, End);
    }

    /// <summary>
    /// 调试用可视化方法
    /// </summary>
    public void DrawDebugLine(Color color, float duration = 0)
    {
        Debug.DrawLine(Start, End, color, duration);
        Debug.DrawLine(Start, Start + Normal * 0.3f, Color.blue, duration);
    }
}
