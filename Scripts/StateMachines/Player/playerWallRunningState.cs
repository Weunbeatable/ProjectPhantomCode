using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerWallRunningState : PlayerBaseState
{
    private readonly int WallRunSpeedHash = Animator.StringToHash("WallRunD"); // storing hash for free look spee dparameter
    private readonly int WallRunMirrorSpeedHash = Animator.StringToHash("wallRunDMirror"); // storing hash for free look spee dparameter 

    private readonly int WallRunBlendTreeHash = Animator.StringToHash("FreeLookBlendTree");

    private const float CrossFadeDuration = 0.1f;
    public playerWallRunningState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        CheckForWalll();
        CheckForDoubleJump();
        CheckForAnimationOrientation(WallRunSpeedHash, WallRunMirrorSpeedHash, CrossFadeDuration);
        /*if(stateMachine.InputReader.MovementValue.x < 0)
        {
            stateMachine.anim
        }*/
        //TODO: REWORK FOR AIRWALKING, air walking will be limited to 
        if ((stateMachine.wallLeft || stateMachine.wallRight) && MinimumWalllRunHeight()) { stateMachine.InputReader.JumpEvent += OnWallJump; }

        stateMachine.InputReader.DodgeEvent += InitiateWallHang;
        stateMachine.ledgeDetector.onLedgeDetect += LedgeDetector_onLedgeDetect;

    }

   

    private void CheckForDoubleJump()
    {
        if (stateMachine.airJumpCounter >= 1)
        {
            stateMachine.airJumpCounter = 0;
            stateMachine.isAirJumpExhausted = false;
        }
    }
    public override void Tick(float deltaTime)
    {
       // Vector3 velocity = stateMachine.characterController.velocity;
        WallRunningMovement( deltaTime);
        //CheckForWalll();
        Vector3 movement = CalculateMovement();

      // Move(movement * stateMachine.wallRunSpeed, deltaTime);

        var normalizedTime = GetNormalizedTime(stateMachine.Animator, "wallRunning") < 1f;


        if (normalizedTime)
        {
            // If the animation is still running but the condition for a wall run isn't true anymore than just switch to faling state, no need to air run
            if (stateMachine.exitingWall)
                stateMachine.SwitchState(new PlayerFallingState(stateMachine));

            if (stateMachine.characterController.isGrounded)
            {
                stateMachine.SwitchState(new PlayerLandingState(stateMachine));
            }

        }
        else
        {
            stateMachine.SwitchState(new PlayerFallingState(stateMachine));
        }
        if (stateMachine.characterController.isGrounded)
        {
            stateMachine.SwitchState(new PlayerLandingState(stateMachine));
        }


        //Input check for dodge key
        //if dodge key pressed switch to wall hang state where you can hold on a wall and choose to jump off it
        // future option will be to use targeting skills 


    }
    public override void Exit()
    {
         stateMachine.InputReader.JumpEvent -= OnWallJump;
        stateMachine.InputReader.DodgeEvent -= InitiateWallHang;
        stateMachine.ledgeDetector.onLedgeDetect -= LedgeDetector_onLedgeDetect;
        stateMachine.exitingWall = false;
        //stateMachine.targeter.Cancel();

    }
    private Vector3 CalculateMovement()
    {
        Vector3 cameraForward = stateMachine.MainCameraTransform.forward;
        //Vector3 cameraRight = stateMachine.MainCameraTransform.right;
        cameraForward.z = 0;// don't need to tilt camera
       // cameraRight.y = 0;
        Vector3.Normalize(cameraForward); // want to ensure uniform speed in all directions
       // cameraRight.Normalize();
       if(stateMachine.InputReader.MovementValue.y < 0)
        {
            return cameraForward * stateMachine.InputReader.MovementValue.x;
        }

        return cameraForward * -stateMachine.InputReader.MovementValue.x;

    }
    private void OnWallJump()
    {
       // WallJump();
        stateMachine.SwitchState(new PlayerWallJumpState(stateMachine));
        return;
        
    }
    private void InitiateWallHang()
    {
        stateMachine.SwitchState(new PlayerWallHangState(stateMachine));
        return;
    }
    private void LedgeDetector_onLedgeDetect(Vector3 ledgeForward, Vector3 closestPoint)
    {
        stateMachine.SwitchState(new PlayerWallHangState(stateMachine, ledgeForward, closestPoint));
    }
}
