using EnumCenter;
using UnityEngine;

public class BadPistol:PlayerWeaponBase
{
    public BadPistol(GameObject obj, CharacterBase character) : base(obj, character)
    {
    }

    protected override void OnInit()
    {
        data = ItemDataCenter.Instance.GetWeaponData(WeaponType.BadPistol);
        maxRapidCnt = data.MaxRapidCnt;
        base.OnInit();
    }

    protected override void OnFire()
    {
        base.OnFire();
        //Bullet_1 bullet=new Bullet_1(LoadManager.Instance.)
        BulletFactory.Instance.GetPlayerBullet(BulletType.Bullet_1, this);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        base.OnExit();
        gameObject.SetActive(false);
    }
}