using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class EnemyBullet5 : EnemyBulletBase
{
    public EnemyBullet5(GameObject obj,EnemyWeaponBase weapon) : base(obj,weapon)
    {
        
    }

    protected override void OnInit()
    {
        base.OnInit();
        damage = 5;
        speed = 10f;
        TimerManager.Register(1f, () =>
        {
            float angle=-10f;
            for (int i = 0; i < 5; i++)
            {
                var b = BulletFactory.Instance.GetEnemyBullet(BulletType.EnemyBullet1, null,transform);
                Quaternion finalRotation = b.transform.rotation * Quaternion.Euler(0, 0, angle);

                angle += 5f;
                // 应用最终旋转
                b.SetRotation(finalRotation); // 直接修改旋转
            }
            
            Remove();
        });
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
