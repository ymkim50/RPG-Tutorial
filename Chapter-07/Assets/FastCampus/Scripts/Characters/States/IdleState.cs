using FastCampus.Characters;
using System;
using UnityEngine;

namespace FastCampus.AI
{
    [Serializable]
    public class IdleState : State<EnemyController>
    {
        bool isPatrol = false;
        private float minIdleTime = 0.0f;
        private float maxIdleTime = 3.0f;
        private float idleTime = 0.0f;

        private Animator animator;
        private CharacterController controller;

        protected int isMoveHash = Animator.StringToHash("IsMove");
        protected int moveSpeedHash = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();
        }

        public override void OnEnter()
        {
            animator?.SetBool(isMoveHash, false);
            animator.SetFloat(moveSpeedHash, 0);
            controller?.Move(Vector3.zero);

            if (context is EnemyController_Patrol)
            {
                isPatrol = true;
                idleTime = UnityEngine.Random.Range(minIdleTime, maxIdleTime);
            }
        }

        public override void Update(float deltaTime)
        {
            // if searched target
            // change to move state
            if (context.Target)
            {
                if (context.IsAvailableAttack)
                {
                    // check attack cool time
                    // and transition to attack state
                    stateMachine.ChangeState<AttackState>();
                }
                else
                {
                    stateMachine.ChangeState<MoveState>();
                }
            }
            else if (isPatrol && stateMachine.ElapsedTimeInState > idleTime)
            {
                stateMachine.ChangeState<MoveToWaypointState>();
            }
        }

        public override void OnExit()
        {
        }
    }
}