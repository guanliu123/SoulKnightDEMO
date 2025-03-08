using EnumCenter;
using UnityEngine;

//挂载在角色身上的根，用来标识角色Tag和放置一些通用的组件，在别的脚本中方便获取到
public class CharacterRoot : MonoBehaviour
{
    public CharacterType Type { get; protected set; }

    public Animator animator;
    
    public Animator GetAnimator()
    {
        if (animator == null)
        {
            LogTool.LogError($"角色上的Animator组件未赋值！");
        }

        return animator;
    }
}
