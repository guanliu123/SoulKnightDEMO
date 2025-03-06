using UnityEngine;

public class PetStateMachine : StateMachineBase
{
    public PetBase Pet { get; protected set; }
    
    public PetStateMachine(PetBase pet)
    {
        Pet = pet;
    }
}
