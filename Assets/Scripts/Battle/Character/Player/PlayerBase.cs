using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerBase : CharacterBase
{
    //public PlayerData data { get; private set; }
    //与父类的root区分开，为了存放转换后的playerroot
    public PlayerRoot root { get; protected set; }

    private FixVector2 moveDir;
    public Animator animator { get; protected set; }
    
    public Rigidbody2D rigidBody{ get; protected set; }
    public PlayerControlInput input { get; protected set; }
    public PlayerWeaponBase NowPlayerWeapon { get; protected set; }
    
    //玩家目前正在瞄准的目标
    public Transform NowTarget { get; protected set; }
    
    protected PlayerStateMachine stateMachine;
    protected List<PlayerWeaponBase> playerWeapons;
    protected int nowWeaponIdx;
    private int maxWeaponCnt;

    public PlayerBase(GameObject obj) : base(obj)
    {
        playerWeapons = new List<PlayerWeaponBase>();
        maxWeaponCnt = 2;
    }

    protected override void OnCharacterStart()
    {
        base.OnCharacterStart();
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        rigidBody.gravityScale = 0;
    }

    protected override void OnInit()
    {
        base.OnInit();
        EventManager.Instance.On<object[]>(EventId.ON_INTERACTING_OBJECT,InteractingObject);

        root=(PlayerRoot)Root;
        nowWeaponIdx = 0;
        Attribute = new PlayerAttribute(root.playerType);
        animator = root.GetAnimator();
        rigidBody = root.GetRigidBody();
        AbstractManager.Instance.GetController<PlayerController>().AddPlayerPet(PetType.LittleCool,this);
        
        stateMachine = new NormalCharacterStateMachine(this);
    }

    protected override void OnCharacterUpdate()
    {
        base.OnCharacterUpdate();
        stateMachine.GameUpdate();

        NowTarget = AbstractManager.Instance.GetController<EnemyController>().GetNearestEnemy(transform.position);
        if (NowTarget)
        {
            IsLeft=NowTarget.position.x < transform.position.x;
        }
        
        if (NowPlayerWeapon != null)
        {
            NowPlayerWeapon.OnUpdate();
            NowPlayerWeapon.ControlWeapon(input.isAttack);
            var weaponRotate = input.WeaponAnimPos;
            if (NowTarget != null)
            {
                Vector3 toTarget = NowTarget.position - transform.position;
                weaponRotate =new FixVector2(toTarget.normalized); // 关键修改：将方向改为指向目标
            }
            NowPlayerWeapon.RotateWeapon(weaponRotate);
            if (input.isSwitchWeapon)
            {
                SwitchWeapon();
            }
        }
    }
    
    protected virtual void PickUpWeapon(GameObject weaponObj)
    {
        var weapon = WeaponFactory.Instance.GetPlayerWeaponInScene(weaponObj, this);
        if (playerWeapons.Count >= maxWeaponCnt)
        {
            ReplaceWeapon(weapon);
        }
        else
        {
            playerWeapons.Add(weapon);
            SwitchWeapon();
        }
    }

    protected void ReplaceWeapon(PlayerWeaponBase newWeapon)
    {
        if(NowPlayerWeapon!=null) NowPlayerWeapon.OnReplace();
        playerWeapons[nowWeaponIdx] = newWeapon;
        NowPlayerWeapon = newWeapon;
        NowPlayerWeapon.OnEnter();
    }

    protected void SwitchWeapon()
    {
        if(NowPlayerWeapon!=null) NowPlayerWeapon.OnExit();
        nowWeaponIdx = (nowWeaponIdx + 1) % playerWeapons.Count;
        NowPlayerWeapon = playerWeapons[nowWeaponIdx];
        NowPlayerWeapon.OnEnter();

        input.isSwitchWeapon = false;
    }

    public void SetInput(PlayerControlInput _input)
    {
        input = _input;
    }
    
    private void InteractingObject(object[] info)
    {
        InteractiveObjectType objType = (InteractiveObjectType)info[0];
        if (objType == InteractiveObjectType.Weapon)
        {
            InteractiveObjectRoot root = (InteractiveObjectRoot)info[1];
            PickUpWeapon(root.gameObject);
            root.IsInteractable=false;
        }
    }
}
