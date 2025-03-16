using cfg;
using EnumCenter;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerWeaponBase:WeaponBase
{
    public new PlayerBase player{get=>base.character as PlayerBase;
        set => base.character = value;
    }
    public WeaponRoot Root { get; private set; }

    public Transform RotOrigin { get; protected set; }

    //private bool isAttackKeyDown;
    private float fireCoolTime;
    private float fireTimer;
    //连发次数（单发=1，n连发=n，>9=持续连发）
    protected int maxRapidCnt;
    protected int rapidNum;
    
    public PlayerWeaponBase(GameObject obj, CharacterBase character) : base(obj, character)
    {
    }

    protected override void OnInit()
    {
        base.OnInit();
        Root = gameObject.GetComponent<WeaponRoot>();
        if (!Root)
        {
            LogTool.LogError("武器上未挂载WeaponRoot！");
        }

        RotOrigin = Root.GetRotOrigin();
        fireCoolTime = 1f / data.FireRate;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        //保证第一发子弹可以发射
        fireTimer = fireCoolTime;
        rapidNum = 0;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        fireTimer+=Time.deltaTime;
    }
    
    public void ControlWeapon(bool isAttack)
    {
        if (isAttack && fireTimer > fireCoolTime && (maxRapidCnt >10||rapidNum<maxRapidCnt))
        {
            fireTimer = 0;
            if (maxRapidCnt < 10)
            {
                rapidNum++;
            }
            OnFire();
        }
        else if (!isAttack)
        {
            rapidNum=0;
        }
    }

    public void RotateWeapon(FixVector2 dir)
    {
        if (canRotate)
        {
            float angle = 0;
            if (character.IsLeft)
            {
                angle = -Vector2.SignedAngle(Vector2.left, dir.ToVector2());

            }
            else
            {
                angle = Vector2.SignedAngle(Vector2.right, dir.ToVector2());
            }
            RotOrigin.localRotation=Quaternion.Euler(0,0,angle);
        }
    }
    
    public virtual void OnReplace()
    {
        //替换武器时重置，放下到场景
        gameObject.transform.SetParent(null);
        gameObject.transform.localRotation = Quaternion.identity;

        //放到场景中的武器挂载可互动根
        var ir = gameObject.GetComponent<InteractiveObjectRoot>();
        if (ir == null)
        {
            ir = gameObject.AddComponent<InteractiveObjectRoot>();
            ir.type = InteractiveObjectType.Weapon;
            ir.itemIndicator = GameTool.GetGameObjectFromChildren(gameObject, "ItemIndicator");
        }
        ir.IsInteractable = true;
    }
}