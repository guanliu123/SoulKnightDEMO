using UnityEngine;
using System.Collections.Generic;

public static class ORCASolver
{
    private const int MAX_ITERATIONS = 10; // 迭代次数限制
    private const float EPSILON = 0.0001f;

    public static Vector2 Solve(Vector2 currentVel, List<Line> constraints)
    {
        // 步骤1：初始化可行速度区域
        Vector2 optimizedVel = currentVel;
        float stepSize = 0.5f; // 学习率

        // 步骤2：迭代优化过程
        for (int i = 0; i < MAX_ITERATIONS; i++)
        {
            Vector2 gradient = Vector2.zero;
            bool isValid = true;

            // 遍历所有约束线计算梯度
            foreach (var line in constraints)
            {
                Vector2 lineDir = (line.End - line.Start).normalized;
                float distance = SignedDistanceToLine(optimizedVel, line);
                
                if (distance < -EPSILON) // 违反约束
                {
                    gradient += lineDir * distance;
                    isValid = false;
                }
            }

            if (isValid) break; // 找到可行解
            
            // 梯度方向优化
            optimizedVel -= stepSize * gradient.normalized;
            stepSize *= 0.8f; // 衰减学习率
        }
        
        return optimizedVel;
    }

    private static float SignedDistanceToLine(Vector2 point, Line line)
    {
        Vector2 lineDir = line.End - line.Start;
        Vector2 pointDir = point - line.Start;
        return Vector2.Dot(pointDir, line.Normal) - 
               Vector2.Dot(lineDir, line.Normal) * 0.5f;
    }
}