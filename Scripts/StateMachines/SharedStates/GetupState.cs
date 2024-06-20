using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        stateMachine.health.setInVulnerable(false);
    }

    

}
