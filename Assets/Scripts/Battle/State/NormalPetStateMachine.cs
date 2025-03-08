public class NormalPetStateMachine:PetStateMachine
{
    public NormalPetStateMachine(PetBase pet) : base(pet)
    {
        ChangeState<PetIdleState>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (currentState is PetIdleState)
        {
            if (GetDistanceToPlayer() > 5)
            {
                ChangeState<PetFollowPlayerState>();
            }
        }
        else if (currentState is PetFollowPlayerState)
        {
            if (GetDistanceToPlayer() < 2)
            {
                ChangeState<PetIdleState>();
            }
        }
    }
}
