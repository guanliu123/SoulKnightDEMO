
    using UnityEngine;

    public class BlueFireGatling:PlayerWeaponBase
    {
        public BlueFireGatling(GameObject obj, CharacterBase character) : base(obj, character)
        {
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
