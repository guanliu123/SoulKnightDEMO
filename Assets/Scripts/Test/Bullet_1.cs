using cfg;
using UnityEngine;

public class Bullet_1 : PlayerBulletBase
{
    public Bullet_1(GameObject obj,PlayerWeaponBase weapon) : base(obj,weapon)
    {
        
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        transform.position += transform.right * 30 * Time.deltaTime;
        if (Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Obstacle")))
        {
            Remove();
        }
    }
}
