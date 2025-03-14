using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : CharacterBase
{
    //敌人当前所属的房间
    public Room room;
    //敌人是否正在运作
    public bool isWork;
    public Animator animator { get;protected set; }
    public EnemyRoot root { get; protected set; }
    public EnemyBase(GameObject obj) : base(obj)
    {
    }

    protected override void OnInit()
    {
        base.OnInit();

        root=(EnemyRoot)Root;
        root.GetTriggerBox().SetActive(true);
        animator = root.GetAnimator();
        Attribute = new EnemyAttribute(root.enemyType);
        EventManager.Instance.On<Room>(EventId.OnPlayerEnterBattleRoom, (r) =>
        {
            if (room == r)
            {
                isWork = true;
                AbstractManager.Instance.GetController<EnemyController>().AddInRoom(this);
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

    protected override void OnCharacterDieStart()
    {
        base.OnCharacterDieStart();
        animator.Play("Die");
        root.GetTriggerBox().SetActive(false);
        SetLocked(false);
        EventManager.Instance.Emit(EventId.EnemyDie,room);
        TimerManager.Register(30f, () =>
        {
            var name = root.enemyType.ToString();
            ObjectPoolManager.Instance.GetPool(name).DeSpawn(gameObject,name);
        });
    }

    public void SetLocked(bool isLocked)
    {
        root.GetTargetCircle().SetActive(isLocked);
    }
}
