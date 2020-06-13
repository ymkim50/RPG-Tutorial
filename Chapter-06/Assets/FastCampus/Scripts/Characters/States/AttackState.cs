using FastCampus.Characters;
using FastCampus.Core;
using System.Diagnostics;
using UnityEngine;

namespace FastCampus.AI
{
    //public class AttackState_Original : State<EnemyController>
    //{
    //    private Animator animator;

    //    protected int attackTriggerHash = Animator.StringToHash("AttackTrigger");

    //    public override void OnInitialized()
    //    {
    //        animator = context.GetComponent<Animator>();
    //    }

    //    public override void OnEnter()
    //    {
    //        animator?.SetTrigger(attackTriggerHash);

    //        if (context.IsAvailableAttack)
    //        {
    //            animator?.SetTrigger(attackTriggerHash);
    //        }
    //        else
    //        {
    //            stateMachine.ChangeState<IdleState>();
    //        }
    //    }

    //    public override void Update(float deltaTime)
    //    {
    //    }
    //}

    public class AttackState : State<EnemyController>
    {
        private Animator animator;
        private AttackStateController attackStateController;
        private IAttackable attackable;

        protected int attackTriggerHash = Animator.StringToHash("AttackTrigger");
        protected int attackIndexHash = Animator.StringToHash("AttackIndex");

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            attackStateController = context.GetComponent<AttackStateController>();
            attackable = context.GetComponent<IAttackable>();
        }

        public override void OnEnter()
        {
            if (attackable == null || attackable.CurrentAttackBehaviour == null)
            {
                stateMachine.ChangeState<IdleState>();
                return;
            }


            attackStateController.enterAttackHandler += OnEnterAttackState;
            attackStateController.exitAttackHandler += OnExitAttackState;

            animator?.SetInteger(attackIndexHash, attackable.CurrentAttackBehaviour.animationIndex);
            animator?.SetTrigger(attackTriggerHash);
        }

        public override void Update(float deltaTime)
        {
        }

        public override void OnExit()
        {
            attackStateController.enterAttackHandler -= OnEnterAttackState;
            attackStateController.exitAttackHandler -= OnExitAttackState;
        }

        public void OnEnterAttackState()
        {
            UnityEngine.Debug.Log("OnEnterAttackState()");
        }

        public void OnExitAttackState()
        {
            UnityEngine.Debug.Log("OnExitAttackState()");
            stateMachine.ChangeState<IdleState>();
        }
    }
}