using System;
using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class EnemyRoot : CharacterRoot
{
    public EnemyType enemyType;

    //敌人被瞄准时脚下的红圈
    public GameObject targetCircle;
    
    private void Awake()
    {
        Type = CharacterType.Enemy;
    }

    public GameObject GetTargetCircle()
    {
        if (targetCircle == null)
        {
            LogTool.LogError("角色上的TargetCircle组件未赋值！");
        }

        return targetCircle;
    }
}
