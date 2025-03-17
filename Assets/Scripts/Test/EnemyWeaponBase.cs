using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponBase : WeaponBase
{ 
    public new EnemyBase enemy{get=>base.character as EnemyBase;
        set => base.character = value;
    }
    public WeaponRoot Root { get; private set; }

    public Transform RotOrigin { get; protected set; }
    
    public EnemyWeaponBase(GameObject obj, CharacterBase character) : base(obj, character)
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
}
