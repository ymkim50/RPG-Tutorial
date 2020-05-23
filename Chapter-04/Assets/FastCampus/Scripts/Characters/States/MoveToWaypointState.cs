using FastCampus.Characters;
using UnityEngine;
using UnityEngine.AI;

namespace FastCampus.AI
{
    public class MoveToWaypointState : State<EnemyController>
    {
        private Animator animator;
        private CharacterController controller;
        private NavMeshAgent agent;

        private EnemyController_Patrol patrolController;

        private int hashMove = Animator.StringToHash("Move");
        private int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();
            agent = context.GetComponent<NavMeshAgent>();

            patrolController = context as EnemyController_Patrol;
        }

        public override void OnEnter()
        {
            agent.stoppingDistance = 0.0f;

            if (patrolController?.targetWaypoint == null)
            {
                patrolController?.FindNextWaypoint();
            }

            if (patrolController?.targetWaypoint != null)
            {
                Vector3 destination = patrolController.targetWaypoint.position;
                agent?.SetDestination(destination);
                animator?.SetBool(hashMove, true);
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
            else
            {

                if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance))
                {
                    FindNextWaypoint();
                    stateMachine.ChangeState<IdleState>();
                }
                else
                {
                    controller.Move(agent.velocity * Time.deltaTime);
                    animator.SetFloat(hashMoveSpeed, agent.velocity.magnitude / agent.speed, .1f, Time.deltaTime);
                }
            }
        }

        public override void OnExit()
        {
            agent.stoppingDistance = context.attackRange;
            animator?.SetBool(hashMove, false);
            agent.ResetPath();
        }

        private void FindNextWaypoint()
        {
            Transform targetWaypoint = patrolController.FindNextWaypoint();
            if (targetWaypoint != null)
            {
                agent?.SetDestination(targetWaypoint.position);
            }
        }
    }
}