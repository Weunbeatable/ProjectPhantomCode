using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChasingState : EnemyBaseState
{
    private readonly int SpeedHash = Animator.StringToHash("Speed"); // storing hash for locomotion speed parameter
    private readonly int LocomotionBlendTreeHash = Animator.StringToHash("Locomotion");

    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;
    public EnemyChasingState(EnemyStateMachine stateMachine) : base(stateMachine)  {}

    public override void Enter()
    {

        
        stateMachine.health.onTakeDamage += HandleTakeDamage;
        stateMachine.navMesh.enabled = true;
       // stateMachine.navMesh.updatePosition = true;
        // stateMachine.navMesh.updateRotation = true;
        stateMachine.Animator.CrossFadeInFixedTime(LocomotionBlendTreeHash, CrossFadeDuration);    
    }

    public override void Tick(float deltaTime)
    {
       
        if (!IsInChasingRange())
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
            return;
        }
        else if (IsInAttackRange())
        {
            stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
            return;
        }

        FacePlayer();
        MoveToPlayer(deltaTime);      
        stateMachine.Animator.SetFloat(SpeedHash, 1f, AnimatorDampTime, deltaTime);
    }

    public override void Exit()
    {
        stateMachine.health.onTakeDamage -= HandleTakeDamage;
        stateMachine.navMesh.ResetPath();
        //stateMachine.navMesh.velocity = Vector3.zero;
        stateMachine.navMesh.enabled = false;
    }

    private void HandleTakeDamage()
    {

        stateMachine.LoadStates();

    }
    private bool IsInAttackRange()
    {
        if (stateMachine.player.isDead) { return false; }
        float PlayerDistance = Vector3.Distance(stateMachine.transform.position, stateMachine.player.transform.position);
            
            return PlayerDistance <= stateMachine.AttackRange;
            
    }

    private void MoveToPlayer(float deltaTime)
    {
        if (stateMachine.navMesh.isOnNavMesh)
        {
            stateMachine.navMesh.destination = stateMachine.player.transform.position;

            Move(stateMachine.navMesh.desiredVelocity.normalized * stateMachine.MovementSpeed, deltaTime);
        }

        stateMachine.navMesh.velocity = stateMachine.Controller.velocity; // line is left out of if check to  make sure nav mesh is up to date.
    }

}
