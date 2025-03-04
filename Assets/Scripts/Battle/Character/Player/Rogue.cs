
    using EnumCenter;
    using UnityEngine;

    public class Rogue: PlayerBase
    {
        public Rogue(GameObject obj) : base(obj)
        {
        }

        protected override void OnInit()
        {
            base.OnInit();
            stateMachine.ChangeState<NormalCharacterIdleState>();
        }
    }