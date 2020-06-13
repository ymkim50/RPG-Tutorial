using FastCampus.AI;
using FastCampus.Core;
using FastCampus.UIs;
using System.Collections;
using UnityEngine;

namespace FastCampus.Characters
{
    public class EnemyController_Patrol : EnemyController, IDamagable
    {
        #region Variables

        public Collider weaponCollider;
        public Transform hitPoint;
        public GameObject hitEffect = null;

        public Transform[] waypoints;

        //public float maxHealth = 100f;
        //public float currentHealth = 100f;

        public NPCBattleUI healthBar;

        #endregion Variables

        #region Proeprties



        #endregion Properties

        #region Unity Methods

        protected override void Start()
        {
            base.Start();

            stateMachine.AddState(new MoveState());
            stateMachine.AddState(new AttackState());
            stateMachine.AddState(new MoveToWaypointState());

            health = maxHealth;

            if (healthBar)
            {
                healthBar = GetComponent<NPCBattleUI>();
                healthBar.MinimumValue = 0.0f;
                healthBar.MaximumValue = maxHealth;
                healthBar.Value = health;
            }
        }

        #endregion Unity Methods

        #region Helper Methods

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

        public void EnableAttackCollider()
        {
            Debug.Log("Check Attack Event");
            if (weaponCollider)
            {
                weaponCollider.enabled = true;
            }


            StartCoroutine("DisableAttackCollider");
        }

        IEnumerator DisableAttackCollider()
        {
            yield return new WaitForFixedUpdate();

            if (weaponCollider)
            {
                weaponCollider.enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != weaponCollider)
            {
                return;
            }

            if (((1 << other.gameObject.layer) & TargetMask) != 0)
            {
                //It matched one
                Debug.Log("Attack Trigger: " + other.name);
                PlayerCharacter playerCharacter = other.gameObject.GetComponent<PlayerCharacter>();
                playerCharacter?.TakeDamage(10, hitEffect);

            }

            if (((1 << other.gameObject.layer) & TargetMask) == 0)
            {
                //It wasn't in an ignore layer
            }
        }

        #endregion Helper Methods

        #region IDamagable interfaces

        public float maxHealth = 100f;

        private float health;

        public bool IsAlive => (health > 0);

        private int hitTriggerHash = Animator.StringToHash("HitTrigger");
        private int isAliveHash = Animator.StringToHash("IsAlive");

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
                healthBar.enabled = false;
                animator?.SetBool(isAliveHash, false);

                Destroy(gameObject, 3.0f);
            }
        }
        #endregion IDamagable interfaces
    }
}