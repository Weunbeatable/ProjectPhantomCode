using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerBaseState
{
    public PlayerWallJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    private readonly int WallJumpSpeedHash = Animator.StringToHash("wallJumpOriginal"); // storing hash for free look spee dparameter
    private readonly int WallJumpMirrorSpeedHash = Animator.StringToHash("wallJumpMirrored"); // storing hash for free look spee dparameter 

    private readonly int WallJumpBlendTreeHash = Animator.StringToHash("FreeLookBlendTree"); // Possible TODO: will leave hear for the time being

    private const float CrossFadeDuration = 0.1f;
    
    public override void Enter()
    {
        stateMachine.InputReader.DodgeEvent += OnDash;
        CheckForAnimationOrientation(WallJumpSpeedHash, WallJumpMirrorSpeedHash, CrossFadeDuration);
        stateMachine.exitingWall = false;
       

    }

    public override void Tick(float deltaTime)
    {
        
        
        var normalizedTime = GetNormalizedTime(stateMachine.Animator, "wallJumping") < 1f;
        if(GetNormalizedTime(stateMachine.Animator, "wallJumping") > 0.5f)
        {
            stateMachine.forceReceiver.Reset();
            stateMachine.forceReceiver.Jump(stateMachine.wallJumpUpForce);
            Vector3 sideToSide = new Vector3(stateMachine.characterController.velocity.x, 0f, stateMachine.InputReader.MovementValue.y);
            Move(sideToSide, deltaTime);
            WallJump();
            Move(stateMachine.InputReader.MovementValue, deltaTime);
        }
        
        if (normalizedTime) { return; }

        //WallJump();
        stateMachine.SwitchState(new PlayerFallingState(stateMachine));

            return;
        
    }
    public override void Exit()
    {
        stateMachine.InputReader.DodgeEvent -= OnDash;
    }

    private void OnDash()
    {
        stateMachine.SwitchState(new PlayerDashingState(stateMachine, stateMachine.InputReader.MovementValue)); // flexibility if we wantto dodge with something other than movement value
    }
}
