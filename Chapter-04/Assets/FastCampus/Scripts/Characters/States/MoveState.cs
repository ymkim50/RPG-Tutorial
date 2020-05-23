using FastCampus.Characters;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace FastCampus.AI
{
    public class MoveState : State<EnemyController>
    {
        private Animator animator;
        private CharacterController controller;
        private NavMeshAgent agent;

        private int hashMove = Animator.StringToHash("Move");
        private int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();

            agent = context.GetComponent<NavMeshAgent>();
        }

        public override void OnEnter()
        {
            agent?.SetDestination(context.Target.position);
            animator?.SetBool(hashMove, true);
        }

        public override void Update(float deltaTime)
        {
            Transform enemy = context.SearchEnemy();
            if (enemy)
            {
                agent.SetDestination(context.Target.position);
                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    controller.Move(agent.velocity * Time.deltaTime);
                    animator.SetFloat(hashMoveSpeed, agent.velocity.magnitude / agent.speed, .1f, Time.deltaTime);
                    return;
                }
            }

            stateMachine.ChangeState<IdleState>();
        }

        public override void OnExit()
        {
            animator?.SetBool(hashMove, false);
            agent.ResetPath();
        }
    }
}