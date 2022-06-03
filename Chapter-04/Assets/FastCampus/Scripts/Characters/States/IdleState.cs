using FastCampus.Characters;
using UnityEngine;

namespace FastCampus.AI
{
    public class IdleState : State<EnemyController>
    {
        bool isPatrol = false;
        private float minIdleTime = 0.0f;
        private float maxIdleTime = 3.0f;
        private float idleTime = 0.0f;

        private Animator animator;
        private CharacterController controller;

        protected int hashMove = Animator.StringToHash("Move");
        protected int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();
        }

        public override void OnEnter()
        {
            animator?.SetBool(hashMove, false);
            animator.SetFloat(hashMoveSpeed, 0);
            controller?.Move(Vector3.zero);

            if (context is EnemyController_Patrol)
            {
                isPatrol = true;
                idleTime = Random.Range(minIdleTime, maxIdleTime);
            }
        }

        public override void Update(float deltaTime)
        {
            // if searched target
            // change to move state
            Transform enemy = context.SearchEnemy();
            if (enemy)
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