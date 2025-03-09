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
        Item effect = ItemFactory.Instance.GetEffect(EffectType.EffectBoom,transform.position);
    }
}
