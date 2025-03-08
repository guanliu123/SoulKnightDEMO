using EnumCenter;
using UnityEngine;

public class PlayerRoot : CharacterRoot
{
    public PlayerType playerType;
    public Rigidbody2D rigidBody;
    public Transform weaponOriginPoint;

    private void Awake()
    {
        Type = CharacterType.Player;
    }

    public Rigidbody2D GetRigidBody()
    {
        if (rigidBody == null)
        {
            LogTool.LogError($"角色上的Rigidbody2D组件未赋值！");
        }

        return rigidBody;
    }
    
    public Transform GetWeaponOriginPoint()
    {
        if (weaponOriginPoint == null)
        {
            LogTool.LogError($"角色上的WeaponOriginPoint组件未赋值！");
        }

        return weaponOriginPoint;
    }
}
