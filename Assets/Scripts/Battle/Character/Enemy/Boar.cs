using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : EnemyBase
{
    private GameObject attackBox;
    public Boar(GameObject obj) : base(obj)
    {
    }

    protected override void OnInit()
    {
        base.OnInit();
        stateMachine = new ColliderEnemyStateMachine(this);

        attackBox = GameTool.GetGameObjectFromChildren(gameObject, "HitPlayerBox");
        SwitchAttack(false);
    }

    public void SwitchAttack(bool isAttack)
    {
        attackBox?.SetActive(isAttack);
    }

    protected override void OnCharacterDieStart()
    {
        base.OnCharacterDieStart();
        SwitchAttack(false);
    }
}
