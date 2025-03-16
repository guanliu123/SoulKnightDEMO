using cfg;
using EnumCenter;
using UnityEngine;

public class Bullet_1 : PlayerBulletBase
{
    public Bullet_1(GameObject obj,PlayerWeaponBase weapon) : base(obj,weapon)
    {
        
    }

    protected override void OnHitObstacle()
    {
        base.OnHitObstacle();
        ItemFactory.Instance.GetEffect(EffectType.EffectBoom,transform.position);
    }

    protected override void OnHitEnemy(EnemyBase enemy)
    {
        base.OnHitEnemy(enemy);
        ItemFactory.Instance.GetEffect(EffectType.EffectBoom,transform.position);
        enemy.UnderAttack(damage);
    }
}
