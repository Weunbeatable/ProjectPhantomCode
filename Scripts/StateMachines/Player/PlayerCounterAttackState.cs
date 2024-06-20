using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerBaseState

{
    private readonly int CounterHash = Animator.StringToHash("Counter_Attack");

    private const float CrossFadeDuration = 0.1f;
    public PlayerCounterAttackState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(CounterHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Counter");
        if (normalizedTime > 1f)
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            return;
        }
        else return;
    }
    public override void Exit()
    {
        
    }


}
