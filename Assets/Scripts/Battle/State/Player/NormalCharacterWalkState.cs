using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCharacterWalkState : PlayerStateBase
{
    public NormalCharacterWalkState(PlayerStateMachine machine) : base(machine)
    {
        
    }

    public override void OnEnter()
    {
        base.OnEnter();
        animator.Play("Walk");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        GetPlayerInputToMove();
    }

    private void GetPlayerInputToMove()
    {
        // Fix64 hor =(Fix64)ETCInput.GetAxis("Horizontal");
        // Fix64 ver =(Fix64)ETCInput.GetAxis("Vertical");
        // FixVector2 moveDir = new FixVector2(hor, ver);

        if (FixVector2.Magnitude(player.input.moveDir) > 0)
        {
            var dir = player.input.moveDir;
            dir.Normalize();
            rigidBody.transform.position += (Vector3)(dir * (Fix64)player.Attribute.Speed * (Fix64)Time.deltaTime).ToVector2();
            player.IsLeft = player.input.hor < 0;
        }
    }
}
