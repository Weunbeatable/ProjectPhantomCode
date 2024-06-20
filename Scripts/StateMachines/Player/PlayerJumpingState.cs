using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    public static event Action onJumped;


    private readonly int PlayerJumpHash = Animator.StringToHash("BaseJump");
    private readonly int initiatePlayerJumpHash = Animator.StringToHash("StartJump");
    private const float CrossFadeDuration = 0.1f;
    private Vector3 momentum;
    

    public PlayerJumpingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }
    public PlayerJumpingState(PlayerStateMachine stateMachine, State previousState) : base(stateMachine)
    {
        this.previousState = previousState;
        
    }
    public override void Enter()
    {

        stateMachine.ledgeDetector.onLedgeDetect += HandleLedgeDetection;
        stateMachine.InputReader.JumpEvent += OnAirJump;
        stateMachine.InputReader.DodgeEvent += OnDash;
        stateMachine.forceReceiver.Jump(stateMachine.JumpForce);

        momentum = stateMachine.characterController.velocity;
        momentum.y = 0f;

        onJumped?.Invoke();
        stateMachine.Animator.applyRootMotion = false;
        stateMachine.Animator.CrossFadeInFixedTime(PlayerJumpHash, CrossFadeDuration);
        //ShouldTargetingStillOccur();
    }

    public override void Tick(float deltaTime)
    {
        momentum = ApplyInputToMomentum(momentum, deltaTime);
        Move(momentum, deltaTime); // move based on momentum every frame
        CheckForWalll();

        // cooldown for jumping test
        if (stateMachine.dashCoolDownTimer > 0)
            stateMachine.dashCoolDownTimer -= deltaTime;

        if (!stateMachine.characterController.isGrounded && stateMachine.InputReader.isBasicAttack || stateMachine.InputReader.isHeavyAttack) // go into attacking state if true
        {
            stateMachine.SwitchState(new PlayerAirAttackingState(stateMachine, 0, 0));
            return;
        }

        if (stateMachine.characterController.velocity.y <= 0)
        {
            stateMachine.SwitchState(new PlayerFallingState(stateMachine, previousState));

            return;
        }

        GroundedStateCheck();
        Vector3 move = CalculateMovement();

        Move(move * stateMachine.FreeLookMovementSpeed, deltaTime);
        FaceMovementDirection(move, deltaTime);

        if ((stateMachine.wallLeft || stateMachine.wallRight) && MinimumWalllRunHeight() && stateMachine.InputReader.MovementValue.y > 0 && !stateMachine.exitingWall)
        {

            // Move(stateMachine.InputReader.MovementValue, deltaTime);
            stateMachine.SwitchState(new playerWallRunningState(stateMachine));

            return;
        }
        else if (stateMachine.exitingWall)
        {
            stateMachine.SwitchState(new PlayerFallingState(stateMachine));

            if (stateMachine.exitWallTimer > 0)
            {
                stateMachine.exitWallTimer -= Time.deltaTime;

                if (stateMachine.exitWallTimer <= 0)
                {
                    stateMachine.exitingWall = false;
                }
            }
        }
        //FaceTarget();
    }

   

    public override void Exit()
    {
        stateMachine.ledgeDetector.onLedgeDetect -= HandleLedgeDetection;
        stateMachine.InputReader.JumpEvent -= OnAirJump;
        stateMachine.InputReader.DodgeEvent -= OnDash;
    }

    private void OnDash()
    {
        
        CheckToTeleportOrDash();
    }
    private void HandleLedgeDetection(Vector3 ledgeForward, Vector3 closestPoint)
    {      
            stateMachine.SwitchState(new PlayerHangingState(stateMachine, ledgeForward, closestPoint));
    }

    private void OnAirJump()
    {
        if (stateMachine.isAirJumpExhausted == false)
        {
            if(previousState != null)
            {
                if (previousState is PlayerTargetingState)
                {
                    stateMachine.SwitchState(new PlayerAirJumpState(stateMachine, previousState));
                    return;
                }
            }
            else
            {
                stateMachine.SwitchState(new PlayerAirJumpState(stateMachine));
                return;
            } 
        }
        else { return; }
        // making sure we odn't run anything else if this If statement runs
    }
    private Vector3 CalculateMovement()
    {
        Vector3 cameraForward = stateMachine.MainCameraTransform.forward;
        Vector3 cameraRight = stateMachine.MainCameraTransform.right;
        cameraForward.y = 0;// don't need to tilt camera
        cameraRight.y = 0;
        Vector3.Normalize(cameraForward); // want to ensure uniform speed in all directions
        cameraRight.Normalize();

        return cameraForward * stateMachine.InputReader.MovementValue.y + cameraRight * stateMachine.InputReader.MovementValue.x;

    }

    private void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
        stateMachine.transform.rotation,
        Quaternion.LookRotation(movement),
        deltaTime * stateMachine.RotationDamping);
    }

    private void GroundedStateCheck()
    {
        if (stateMachine.characterController.isGrounded)
        {
            if (previousState != null)
            {
                if (previousState is PlayerTargetingState)
                {
                    stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
                }
                else
                {
                    stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
                }
            }
            else
            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            }
        }
    }
}
