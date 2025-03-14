using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : CharacterBase
{
    //敌人当前所属的房间
    public Room room;
    //敌人是否正在运作
    public bool isWork;
    public EnemyRoot root { get; protected set; }
    public EnemyBase(GameObject obj) : base(obj)
    {
    }

    protected override void OnInit()
    {
        base.OnInit();

        root=(EnemyRoot)Root;
        Attribute = new EnemyAttribute(root.enemyType);
        EventManager.Instance.On<Room>(EventId.OnPlayerEnterBattleRoom, (r) =>
        {
            if (room == r)
            {
                isWork = true;
            }
        });
    }

    public override void UnderAttack(int damage)
    {
        if (isWork)
        {
            base.UnderAttack(damage);
        }
    }

    protected override void OnCharacterUpdate()
    {
        if (isWork)
        {
            base.OnCharacterUpdate();
        }
    }
}
