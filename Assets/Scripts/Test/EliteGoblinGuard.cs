using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteGoblinGuard : EnemyBase
{
        public EliteGoblinGuard(GameObject obj) : base(obj)
        {
        }
    
        protected override void OnInit()
        {
            base.OnInit();
            stateMachine = new NormalEnemyStateMachine(this);
        }
}
