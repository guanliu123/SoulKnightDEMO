﻿
    using System.Collections;
    using UnityEngine;

    public class PetFollowPlayerState:PetStateBase
    {
        private int currentPathIndex;
        private Vector3 moveDir;
        public PetFollowPlayerState(PetStateMachine machine) : base(machine)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            anim.Play("Walk");
            MonoManager.Instance.StartCoroutine(this, SeekerLoop());
        }

        public override void OnExit()
        {
            base.OnExit();
            MonoManager.Instance.StopAllCoroutineInObj(this);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            MoveToTarget();
            pet.IsLeft = !(moveDir.x > 0);
        }

        public void MoveToTarget()
        {
            if (path == null||currentPathIndex>=path.vectorPath.Count) return;
            moveDir = (path.vectorPath[currentPathIndex] - transform.position).normalized;
            transform.position +=  moveDir* pet.Attribute.Speed *
                                  Time.deltaTime;
            if (Vector2.Distance(transform.position, path.vectorPath[currentPathIndex]) < 1)
            {
                currentPathIndex++;
            }
        }

        private IEnumerator SeekerLoop()
        {
            while (true)
            {
                if (seeker.IsDone())
                {
                    seeker.StartPath(transform.position, owner.transform.position, (p) =>
                    {
                        path = p;
                        currentPathIndex = 0;
                    });
                } 
                yield return new WaitForSeconds(0.5f);
            }   
        }
    }
