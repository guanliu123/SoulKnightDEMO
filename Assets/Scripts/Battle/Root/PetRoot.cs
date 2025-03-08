using EnumCenter;
using Pathfinding;

public class PetRoot:CharacterRoot
{
    public Seeker seeker;

    public PetType petType;

    private void Awake()
    {
        Type = CharacterType.Pet;
    }
    
    public Seeker GetSeekeer()
    {
        if (seeker == null)
        {
            LogTool.LogError($"角色上的Rigidbody2D组件未赋值！");
        }

        return seeker;
    }
}
