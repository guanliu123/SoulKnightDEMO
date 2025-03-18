using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class EnemyBullet1 : EnemyBulletBase
{
    public EnemyBullet1(GameObject obj,EnemyWeaponBase weapon) : base(obj,weapon)
    {
        
    }

    protected override void OnInit()
    {
        base.OnInit();
        speed = 10f;
        damage = 2;
    }

    protected override void OnHitObstacle()
    {
        base.OnHitObstacle();
        ItemFactory.Instance.GetEffect(EffectType.EffectBoom,transform.position);
    }

    protected override void OnHitPlayer(PlayerBase player)
    { 
        base.OnHitPlayer(player);
        ItemFactory.Instance.GetEffect(EffectType.EffectBoom,transform.position);
        player.UnderAttack(damage);
    }
}