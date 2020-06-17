using FastCampus.AI;
using FastCampus.Core;
using FastCampus.UIs;
using System.Collections.Generic;
using UnityEngine;

namespace FastCampus.Characters
{
    public class EnemyController_Melee : EnemyController, IAttackable, IDamagable
    {
        #region Variables
        [SerializeField]
        public Transform hitPoint;

        public Transform[] waypoints;

        [SerializeField]
        private NPCBattleUI healthBar;

        public float maxHealth => 100f;
        private float health;

        private int hitTriggerHash = Animator.StringToHash("HitTrigger");
        private int isAliveHash = Animator.StringToHash("IsAlive");

        #endregion Variables

        #region Proeprties
        public override bool IsAvailableAttack
        {
            get
            {
                if (!Target)
                {
                    return false;
                }

                float distance = Vector3.Distance(transform.position, Target.position);
                return (distance <= AttackRange);
            }
        }

        #endregion Properties

        #region Unity Methods

        protected override void Start()
        {
            base.Start();

            stateMachine.AddState(new MoveState());
            stateMachine.AddState(new AttackState());
            stateMachine.AddState(new DeadState());

            health = maxHealth;

            if (healthBar)
            {
                healthBar.MinimumValue = 0.0f;
                healthBar.MaximumValue = maxHealth;
                healthBar.Value = health;
            }
        }

        private void OnAnimatorMove()
        {
            // Follow NavMeshAgent
            //Vector3 position = agent.nextPosition;
            //animator.rootPosition = agent.nextPosition;
            //transform.position = position;

            // Follow CharacterController
            Vector3 position = transform.position;
            position.y = agent.nextPosition.y;

            animator.rootPosition = position;
            agent.nextPosition = position;

            // Follow RootAnimation
            //Vector3 position = animator.rootPosition;
            //position.y = agent.nextPosition.y;

            //agent.nextPosition = position;
            //transform.position = position;
        }

        #endregion Unity Methods

        #region Helper Methods
        #endregion Helper Methods

        #region IDamagable interfaces

        public bool IsAlive => (health > 0);

        public void TakeDamage(int damage, GameObject hitEffectPrefab)
        {
            if (!IsAlive)
            {
                return;
            }

            health -= damage;

            if (healthBar)
            {
                healthBar.Value = health;
            }

            if (hitEffectPrefab)
            {
                Instantiate(hitEffectPrefab, hitPoint);
            }

            if (IsAlive)
            {
                animator?.SetTrigger(hitTriggerHash);
            }
            else
            {
                if (healthBar != null)
                {
                    healthBar.enabled = false;
                }

                stateMachine.ChangeState<DeadState>();
            }
        }

        #endregion IDamagable interfaces

        #region IAttackable Interfaces

        public GameObject hitEffectPrefab = null;

        [SerializeField]
        private List<AttackBehaviour> attackBehaviours = new List<AttackBehaviour>();

        public AttackBehaviour CurrentAttackBehaviour
        {
            get;
            private set;
        }

        public void OnExecuteAttack(int attackIndex)
        {

        }

        #endregion IAttackable Interfaces
    }
}