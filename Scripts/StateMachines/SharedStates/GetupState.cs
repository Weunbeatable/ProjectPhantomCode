using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GetupState : EnemyBaseState
{
    private readonly int getUpHash = Animator.StringToHash("Getup03_L");

    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.2f;

    private float duration = 1.2f;
    public GetupState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.navMesh.enabled = false;
        stateMachine.health.setInVulnerable(true);
        stateMachine.Animator.CrossFadeInFixedTime(getUpHash, CrossFadeDuration);
    }
    public override void Tick(float deltaTime)
    {
        duration -= deltaTime;

        if (duration <= 0f)
        {
            
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
        }
    }
    public override void Exit()
    {
        stateMachine.navMesh.enabled = true;
        stateMachine.navMesh.updatePosition = true;
        stateMachine.navMesh.updateRotation = true;
        stateMachine.navMesh.ResetPath();
        stateMachine.health.setInVulnerable(false);
    }

    

}
