
    using EnumCenter;
    using UnityEngine;

    public class BlueFireGatling:PlayerWeaponBase
    {
        public BlueFireGatling(GameObject obj, CharacterBase character) : base(obj, character)
        {
        }
        
        protected override void OnInit()
        {
            data = ItemDataCenter.Instance.GetWeaponData(WeaponType.BlueFireGatling);
            maxRapidCnt = data.MaxRapidCnt;
            base.OnInit();
        }
        
        public override void OnFire()
        {
            base.OnFire();
            //Bullet_1 bullet=new Bullet_1(LoadManager.Instance.)
            for (int i = 0; i < 5; i++)
            {
                var b = BulletFactory.Instance.GetPlayerBullet(BulletType.Bullet_1, this);
                float spreadAngle = Random.Range(-5f, 5f);
                Quaternion finalRotation = b.transform.rotation * Quaternion.Euler(0, 0, spreadAngle);

                // 应用最终旋转
                b.SetRotation(finalRotation); // 直接修改旋转
            }
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
