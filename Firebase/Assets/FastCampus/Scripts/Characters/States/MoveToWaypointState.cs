using FastCampus.Characters;
using UnityEngine;
using UnityEngine.AI;

namespace FastCampus.AI
{
    public class MoveToWaypointState : State<EnemyController>
    {
        #region Variables

        private Animator animator;
        private CharacterController controller;
        private NavMeshAgent agent;

        private EnemyController_Patrol patrolController;

        private Transform targetWaypoint = null;
        private int waypointIndex = 0;


        private int isMoveHash = Animator.StringToHash("IsMove");
        private int moveSpeedHash = Animator.StringToHash("MoveSpeed");

        #endregion Variables

        #region Properties

        private Transform[] Waypoints => ((EnemyController_Patrol)context)?.waypoints;

        #endregion Properties

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

            if (targetWaypoint == null)
            {
                FindNextWaypoint();
            } 

            if (targetWaypoint)
            {
                animator?.SetBool(isMoveHash, true);
                agent.SetDestination(targetWaypoint.position);
            }
            else
            {
                stateMachine.ChangeState<IdleState>();
            }
        }

        public override void Update(float deltaTime)
        {
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
                    animator.SetFloat(moveSpeedHash, agent.velocity.magnitude / agent.speed, .1f, Time.deltaTime);
                }
            }
        }

        public override void OnExit()
        {
            agent.stoppingDistance = context.AttackRange;
            animator?.SetBool(isMoveHash, false);
            agent.ResetPath();
        }

        public Transform FindNextWaypoint()
        {
            targetWaypoint = null;

            // Returns if no points have been set up
            if (Waypoints != null && Waypoints.Length > 0)
            {

                // Set the agent to go to the currently selected destination.
                targetWaypoint = Waypoints[waypointIndex];

                // Choose the next point in the array as the destination,
                // cycling to the start if necessary.
                waypointIndex = (waypointIndex + 1) % Waypoints.Length;
            }

            return targetWaypoint;
        }
    }
}