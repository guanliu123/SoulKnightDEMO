using System;
using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class EnemyRoot : CharacterRoot
{
    public EnemyType enemyType;
    
    private void Awake()
    {
        Type = CharacterType.Enemy;
    }
}
