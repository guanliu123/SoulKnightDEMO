
    using EnumCenter;
    using UnityEngine;

    public class Rogue: PlayerBase
    {
        public Rogue(GameObject obj,PlayerType type) : base(obj,type)
        {
        }

        protected override void OnCharacterStart()
        {
            base.OnCharacterStart();
            stateMachine = new NormalCharacterStateMachine(this);
        }
    }