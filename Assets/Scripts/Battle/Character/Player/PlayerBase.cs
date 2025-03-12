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
    
    protected PlayerStateMachine stateMachine;
    protected List<PlayerWeaponBase> playerWeapons;
    protected int nowWeaponIdx;
    private int maxWeaponCnt;

    public PlayerBase(GameObject obj) : base(obj)
    {
        playerWeapons = new List<PlayerWeaponBase>();
        maxWeaponCnt = 2;
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
        
        if (NowPlayerWeapon != null)
        {
            NowPlayerWeapon.OnUpdate();
            NowPlayerWeapon.ControlWeapon(input.isAttack);
            NowPlayerWeapon.RotateWeapon(input.WeaponAnimPos);
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
