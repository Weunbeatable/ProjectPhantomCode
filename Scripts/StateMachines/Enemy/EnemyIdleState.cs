using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private readonly int SpeedHash = Animator.StringToHash("Speed"); // storing hash for locomotion speed parameter
    private readonly int LocomotionBlendTreeHash = Animator.StringToHash("Locomotion");

    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;
    public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine)   {   }

    public override void Enter()
    {
        if(stateMachine.navMesh.enabled == false) { stateMachine.enabled = true; }
        stateMachine.Animator.CrossFadeInFixedTime(LocomotionBlendTreeHash, CrossFadeDuration);

    }

    public override void Tick(float deltaTime)
    {
       
        Move(deltaTime);
        if (IsInChasingRange())
        {           
           stateMachine.SwitchState(new EnemyChasingState(stateMachine));
            return;
        }
        // should only face player if in suspicion range. 
        if (IsInSuspicionRange()) { FacePlayer(); }
       
        stateMachine.Animator.SetFloat(SpeedHash, 0f, AnimatorDampTime, deltaTime);
    }

    public override void Exit()
    {
     
    }
   
}
