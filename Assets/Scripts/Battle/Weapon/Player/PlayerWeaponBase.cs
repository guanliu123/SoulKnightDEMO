using cfg;
using EnumCenter;
using UnityEngine;

public class PlayerWeaponBase:WeaponBase
{
    public new PlayerBase player{get=>base.character as PlayerBase;
        set => base.character = value;
    }
    public WeaponRoot Root { get; private set; }

    public Transform RotOrigin { get; protected set; }

    private bool isAttackKeyDown;
    
    public PlayerWeaponBase(GameObject obj, CharacterBase character) : base(obj, character)
    {
        Root = gameObject.GetComponent<WeaponRoot>();
        if (!Root)
        {
            LogTool.LogError("武器上未挂载WeaponRoot！");
        }

        RotOrigin = Root.GetRotOrigin();
    }

    //射击按钮按下松开发射一次
    public void ControlWeapon(bool isAttack)
    {
        if (isAttackKeyDown!=isAttack&&isAttack)
        {
            OnFire();
        }
        isAttackKeyDown = isAttack;
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