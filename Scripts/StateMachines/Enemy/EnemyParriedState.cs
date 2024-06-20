using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParriedState : EnemyBaseState
{
    private readonly int EnemyParriedHash = Animator.StringToHash("Got_Parried");

    private const float CrossFadeDuration = 0.1f;
    public EnemyParriedState(EnemyStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        FacePlayer();
        stateMachine.Animator.CrossFadeInFixedTime(EnemyParriedHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        if (GetNormalizedTime(stateMachine.Animator, "Parried") > 1)
        {
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
            return;
        }
    }
    public override void Exit()
    {
        
    }

 
}
