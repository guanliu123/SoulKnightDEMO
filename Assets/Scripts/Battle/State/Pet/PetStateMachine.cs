using UnityEngine;

public class PetStateMachine : StateMachineBase
{
    public PetBase Pet { get; protected set; }
    
    public PetStateMachine(PetBase pet)
    {
        Pet = pet;
    }

    protected float GetDistanceToPlayer()
    {
        return Vector2.Distance(Pet.transform.position, Pet.Player.transform.position);
    }
}
