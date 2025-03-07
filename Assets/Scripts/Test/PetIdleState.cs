public class PetIdleState:PetStateBase
{
    public PetIdleState(PetStateMachine machine,PlayerBase onwer) : base(machine,onwer)
    {
            
    }

    public override void OnEnter()
    {
        base.OnEnter();
        anim.Play("Idle");
    }
}