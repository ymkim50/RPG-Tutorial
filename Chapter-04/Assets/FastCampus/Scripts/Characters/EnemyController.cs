using UnityEngine;
using UnityEngine.AI;

namespace FastCampus.Characters
{
    public class EnemyController : MonoBehaviour
    {
        #region Variables
        public float viewRadius = 5f;
        public float attackRange = 1.5f;
        public float CalcAttackRange => attackRange + 0.5f;

        protected Transform target;

        protected NavMeshAgent agent;
        protected Animator animator;
        protected CharacterController controller;

        protected int hashMoveSpeed = Animator.StringToHash("Move");
        protected int hashAttack = Animator.StringToHash("Attack");
        protected int hasAttackIndex = Animator.StringToHash("AttackIndex");

        #endregion Variables

        public virtual Transform Target => target;

        #region Unity Methods
        // Start is called before the first frame update
        protected virtual void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = attackRange;
            agent.updatePosition = false;
            agent.updateRotation = true;

            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (target)
            {
                float distance = Vector3.Distance(target.position, transform.position);
                if (distance <= viewRadius)
                {
                    agent.SetDestination(target.position);
                }

                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    controller.Move(agent.velocity * Time.deltaTime);
                }
                else
                {
                    controller.Move(Vector3.zero);
                }

                animator.SetFloat(hashMoveSpeed, agent.velocity.magnitude / agent.speed, .1f, Time.deltaTime);

                if (distance <= agent.stoppingDistance)
                {
                    // Attack the target
                    animator.SetTrigger(hashAttack);

                    // Face the target
                    FaceTarget();
                }
                else
                {
                }
            }
        }

        void FaceTarget()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, viewRadius);
        }

        private void OnAnimatorMove()
        {
            Vector3 position = agent.nextPosition;
            animator.rootPosition = agent.nextPosition;
            transform.position = position;
        }
        #endregion Unity Methods

        #region Helper Methods
        public virtual bool IsAvailableAttack => false;

        public virtual Transform SearchEnemy()
        {
            return null;
        }
        #endregion Helper Methods
    }
}