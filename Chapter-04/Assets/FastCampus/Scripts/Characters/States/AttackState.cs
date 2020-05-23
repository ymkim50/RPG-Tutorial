using FastCampus.Characters;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

namespace FastCampus.AI
{
    public class AttackState : State<EnemyController>
    {
        private Animator animator;

        protected int hashAttack = Animator.StringToHash("Attack");

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
        }

        public override void OnEnter()
        {
            if (context.IsAvailableAttack)
            {
                animator?.SetTrigger(hashAttack);
            }
            else
            {
                stateMachine.ChangeState<IdleState>();
            }
        }

        public override void Update(float deltaTime)
        {
        }
    }
}