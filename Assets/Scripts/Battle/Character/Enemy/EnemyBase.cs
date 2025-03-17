using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class EnemyBase : CharacterBase
{
    //敌人当前所属的房间
    public Room room;
    //敌人是否正在运作
    public bool isWork;

    //敌人当前的运动速度
    public FixVector2 Velocity;
    public RectCollider Collider { get; protected set; }
    public Animator animator { get;protected set; }
    public EnemyRoot root { get; protected set; }
    
    protected EnemyStateMachine stateMachine;
    
    private PlayerBase targetPlayer;
    
    public EnemyWeaponBase weapon { get;protected set; }

    public EnemyBase(GameObject obj) : base(obj)
    {
        root=(EnemyRoot)Root;
        int wd = CharacterDataCenter.Instance.GetEnemyData(root.enemyType).WeaponID;
        if (wd != 0)
        {
            weapon = WeaponFactory.Instance.GetEnemyWeapon((WeaponType)wd,this);
        }
    }

    protected override void OnInit()
    {
        base.OnInit();
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
        
        //stateMachine = new NormalEnemyStateMachine(this);
        Collider = root.GetRectCollider();
        targetPlayer=AbstractManager.Instance.GetController<PlayerController>().MainPlayer;
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
            Velocity=new FixVector2(targetPlayer.transform.position - this.transform.position).GetNormalized();
            stateMachine?.GameUpdate();
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
