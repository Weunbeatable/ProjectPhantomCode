using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearchingState : EnemyBaseState
{
    private readonly int confusedTwo = Animator.StringToHash("confusedTwo");
    private readonly int confusedThree = Animator.StringToHash("confusedThree");

    private int confusedHash;
    private const float CrossFadeDuration = 0.1f;

    private float confusedDuration = 1.8f;
    public EnemySearchingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
       // stateMachine.Animator.applyRootMotion = true;
        int randIndex = UnityEngine.Random.Range(1, 2);
        if(randIndex == 1)
        {
            confusedHash = confusedTwo;
            stateMachine.Animator.CrossFadeInFixedTime(confusedHash, CrossFadeDuration);
        }
        else 
        {
            confusedHash += confusedThree;
            stateMachine.Animator.CrossFadeInFixedTime(confusedHash, CrossFadeDuration);
        }
    }
    public override void Tick(float deltaTime)
    {
        confusedDuration -= Time.deltaTime;
        if(confusedDuration <= 0)
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
        }
    }
    public override void Exit()
    {
        
    }
}
