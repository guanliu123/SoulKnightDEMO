
    using EnumCenter;
    using UnityEngine;

    public class Rogue: PlayerBase
    {
        public Rogue(GameObject obj) : base(obj)
        {
        }

        protected override void OnCharacterStart()
        {
            base.OnCharacterStart();
            stateMachine = new NormalCharacterStateMachine(this);
        }
    }