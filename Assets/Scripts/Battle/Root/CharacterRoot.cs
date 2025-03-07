using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;
using UnityEngine.Serialization;

//挂载在角色身上的根，用来标识角色Tag和放置一些通用的组件，在别的脚本中方便获取到
//可能是角色、宠物、敌人、随从的任意一种，使用的时候只选对应的
public class CharacterRoot : MonoBehaviour
{
    [FormerlySerializedAs("characterType")] public PlayerType playerType;
    public PetType petType;

    public Animator animator;
    public Rigidbody2D rigidBody;
    public Transform weaponOriginPoint;

    public Animator GetAnimator()
    {
        if (animator == null)
        {
            LogTool.LogError($"角色{playerType.ToString()}上的Animator组件未赋值！");
        }

        return animator;
    }
    
    public Rigidbody2D GetRigidBody()
    {
        if (rigidBody == null)
        {
            LogTool.LogError($"角色{playerType.ToString()}上的Rigidbody2D组件未赋值！");
        }

        return rigidBody;
    }
    
    public Transform GetWeaponOriginPoint()
    {
        if (weaponOriginPoint == null)
        {
            LogTool.LogError($"角色{playerType.ToString()}上的WeaponOriginPoint组件未赋值！");
        }

        return weaponOriginPoint;
    }
}
