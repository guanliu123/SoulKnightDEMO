using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : CharacterBase
{
    //敌人当前所属的房间
    public Room room;
    //敌人是否正在运作
    public bool isWork;

    //敌人当前的运动速度
    public FixVector2 Velocity;
    private PlayerBase player;
    public Animator animator { get;protected set; }
    public EnemyRoot root { get; protected set; }
    
    protected EnemyStateMachine stateMachine;
    public EnemyBase(GameObject obj) : base(obj)
    {
    }

    protected override void OnInit()
    {
        base.OnInit();

        root=(EnemyRoot)Root;
        //rectCollider.EnableCollision();
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
        
        stateMachine = new NormalEnemyStateMachine(this);
        
        player=AbstractManager.Instance.GetController<PlayerController>().MainPlayer;
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
            Velocity=new FixVector2(player.transform.position - this.transform.position).GetNormalized();
            stateMachine.GameUpdate();
        }
    }

    protected override void OnCharacterDieStart()
    {
        base.OnCharacterDieStart();
        animator.Play("Die");
        //关闭碰撞器
        //rectCollider.DisableCollision();
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
