using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBounceState : EnemyBaseState
{
    private readonly int getUpHash = Animator.StringToHash("Getup03_L");

    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;

    private float duration = 1.6f;
    public GroundBounceState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(getUpHash, 0.2f);
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

    }
}
