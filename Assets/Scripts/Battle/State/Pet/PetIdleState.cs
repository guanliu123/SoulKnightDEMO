public class PetIdleState:PetStateBase
{
    public PetIdleState(PetStateMachine machine) : base(machine)
    {
            
    }

    public override void OnEnter()
    {
        base.OnEnter();
        anim.Play("Idle");
    }
}