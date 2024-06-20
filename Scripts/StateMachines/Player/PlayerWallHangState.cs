using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallHangState : PlayerBaseState
{
    private readonly int wallHangLeftSideHash = Animator.StringToHash("Wall_LHold_Stop_Root"); 
    private readonly int wallHangRightSideHash = Animator.StringToHash("Wall_RHold_Stop");
    private Vector3 ledgeForward;
    private Vector3 closestPoint;
    private const float CrossFadeDuration = 0.1f;
    public PlayerWallHangState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public PlayerWallHangState(PlayerStateMachine stateMachine, Vector3 ledgeForward, Vector3 closestPoint) : this(stateMachine)
    {
        this.ledgeForward = ledgeForward;
        this.closestPoint = closestPoint;
    }

    public override void Enter()
    {
        CheckForWalll();
        CheckForAnimationOrientation(wallHangLeftSideHash, wallHangRightSideHash, CrossFadeDuration);

        if ((stateMachine.wallLeft || stateMachine.wallRight) && MinimumWalllRunHeight()) { stateMachine.InputReader.JumpEvent += OnWallJump; }
    }

    public override void Tick(float deltaTime)
    {
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "wallHang");


        if (normalizedTime < 1f)
        {     
            // If the animation is still running but the condition for a wall run isn't true anymore than just switch to faling state, no need to air run
            if (stateMachine.exitingWall)
                stateMachine.SwitchState(new PlayerFallingState(stateMachine));

            if (stateMachine.characterController.isGrounded)
            {
                stateMachine.SwitchState(new PlayerLandingState(stateMachine));
            }

        }

        if (normalizedTime > 1f)
        {
            stateMachine.characterController.transform.position = stateMachine.ledgeDetector.transform.position;
            stateMachine.forceReceiver.Reset();
            stateMachine.characterController.Move(Vector3.zero);
        }

        if (stateMachine.characterController.isGrounded)
        {
            stateMachine.SwitchState(new PlayerLandingState(stateMachine));
        }
    }
    public override void Exit()
    {
        stateMachine.InputReader.JumpEvent -= OnWallJump;
        stateMachine.exitingWall = false;
    }

    private void OnWallJump()
    {
        stateMachine.SwitchState(new PlayerWallJumpState(stateMachine));
        return;
    }

}
