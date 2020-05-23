using FastCampus.AI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.AI;

namespace FastCampus.Characters
{
    public class EnemyController_Patrol : EnemyController
    {
        #region Variables
        protected StateMachine<EnemyController> stateMachine;

        public LayerMask targetMask;
        public Collider weaponCollider;

        private GameObject hitEffect = null;

        private FieldOfView fov;

        #endregion Variables

        #region Patrol Variables

        public Transform[] waypoints;

        [HideInInspector]
        public Transform targetWaypoint = null;
        private int waypointIndex = 0;

        #endregion Patrol Variables

        #region Properties
        public override Transform Target => fov?.NearestTarget;
        #endregion Properties

        #region Unity Methods

        protected override void Start()
        {
            base.Start();

            stateMachine = new StateMachine<EnemyController>(this, new MoveToWaypointState());
            stateMachine.AddState(new MoveState());
            stateMachine.AddState(new AttackState());
            stateMachine.AddState(new IdleState());

            fov = GetComponent<FieldOfView>();
        }

        // Update is called once per frame
        void Update()
        {
            stateMachine.Update(Time.deltaTime);

            if (!(stateMachine.CurrentState is MoveState))
            {
                FaceTarget();
            }
        }

        #endregion Unity Methods

        #region Helper Methods
        public R ChangeState<R>() where R : State<EnemyController>
        {
            return stateMachine.ChangeState<R>();
        }

        public override bool IsAvailableAttack
        {
            get
            {
                if (!Target)
                {
                    return false;
                }

                float distance = Vector3.Distance(transform.position, Target.position);
                return (distance <= CalcAttackRange);
            }
        }

        override public Transform SearchEnemy()
        {
            return Target;
        }

        public void FaceTarget()
        {
            if (!Target)
            {
                return;
            }

            Vector3 direction = (Target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
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
            if (((1 << other.gameObject.layer) & targetMask) != 0)
            {
                //It matched one
                Debug.Log("Attack Trigger: " + other.name);
                PlayerCharacter playerCharacter = other.gameObject.GetComponent<PlayerCharacter>();
                playerCharacter?.TakeDamage(10, hitEffect);

            }

            if (((1 << other.gameObject.layer) & targetMask) == 0)
            {
                //It wasn't in an ignore layer
            }
        }

        private void OnDrawGizmos()
        {
        }

        public Transform FindNextWaypoint()
        {
            targetWaypoint = null;
            // Returns if no points have been set up
            if (waypoints.Length == 0)
            {
                return targetWaypoint;
            }

            // Set the agent to go to the currently selected destination.
            targetWaypoint = waypoints[waypointIndex];

            // Choose the next point in the array as the destination,
            // cycling to the start if necessary.
            waypointIndex = (waypointIndex + 1) % waypoints.Length;

            return targetWaypoint;
        }

        #endregion Helper Methods
    }
}