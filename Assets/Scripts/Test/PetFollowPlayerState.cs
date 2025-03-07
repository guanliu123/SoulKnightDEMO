
    using System.Collections;
    using UnityEngine;

    public class PetFollowPlayerState:PetStateBase
    {
        private int currentPathIndex;
        public PetFollowPlayerState(PetStateMachine machine,PlayerBase onwer) : base(machine,onwer)
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
        }

        public void MoveToTarget()
        {
            if (path == null||currentPathIndex>=path.vectorPath.Count) return;
            transform.position += (path.vectorPath[currentPathIndex] - transform.position).normalized * pet.data.Speed *
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
                    seeker.StartPath(transform.position, onwer.transform.position, (p) =>
                    {
                        path = p;
                        currentPathIndex = 0;
                    });
                } 
                yield return new WaitForSeconds(0.5f);
            }   
        }
    }
