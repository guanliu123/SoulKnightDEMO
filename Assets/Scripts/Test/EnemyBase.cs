using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : CharacterBase
{
    public EnemyRoot root { get; protected set; }
    public EnemyBase(GameObject obj) : base(obj)
    {
    }

    protected override void OnInit()
    {
        base.OnInit();

        root=(EnemyRoot)Root;
        Attribute = new EnemyAttribute(root.enemyType);
    }
}
