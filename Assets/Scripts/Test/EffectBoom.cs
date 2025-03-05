using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBoom : Item
{
    private float duration;
    public EffectBoom(GameObject obj) : base(obj)
    {

    }

    protected override void OnInit()
    {
        base.OnInit();
        duration=gameObject.GetComponentInChildren<Animator>().runtimeAnimatorController.animationClips[0].length;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        TimerManager.Register(duration, () =>
        {
            Remove();
        });
    }

    protected override void OnExit()
    {
        base.OnExit();
        ObjectPoolManager.Instance.GetPool(PoolName).DeSpawn(gameObject,PoolName);
    }
}