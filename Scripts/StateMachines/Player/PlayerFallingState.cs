using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingState : PlayerBaseState
{
    private readonly int PlayerFallHash = Animator.StringToHash("BaseFall");
    private readonly int PlayerNoInputLand = Animator.StringToHash("Land_Spawn_Wait");
    private readonly int playerMovingInputLand = Animator.StringToHash("Land_Stumble_Move_Root");

    private Vector3 momentum;
    private State previousState; // I'm keeping this because falling is like a substate between jumping and grounded so to avoid overwritting the targeting state 
    //with the actual previous state, i'll keep this here. 
    private const float CrossFadeDuration = 0.1f;
    public PlayerFallingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public PlayerFallingState(PlayerStateMachine stateMachine, State PreviousState) : base(stateMachine)
    {
        this.previousState = PreviousState;

    }

    public override void Enter()
    {
        stateMachine.ledgeDetector.onLedgeDetect += HandleLedgeDetection;
        stateMachine.InputReader.JumpEvent += OnAirJump;
        stateMachine.InputReader.DodgeEvent += OnDash;
        momentum = stateMachine.characterController.velocity;
        momentum.y = 0f;
        stateMachine.Animator.CrossFadeInFixedTime(PlayerFallHash, CrossFadeDuration);

        ShouldTargetingStillOccur();

    }

    public override void Tick(float deltaTime)
    {
        momentum = momentum = ApplyInputToMomentum(momentum, deltaTime);
        Move(momentum, deltaTime);
        CheckForWalll();

        if (stateMachine.dashCoolDownTimer > 0)
            stateMachine.dashCoolDownTimer -= deltaTime;

        if (!stateMachine.characterController.isGrounded && stateMachine.InputReader.isBasicAttack || stateMachine.InputReader.isHeavyAttack) // go into attacking state if true
        {
            stateMachine.SwitchState(new PlayerAirAttackingState(stateMachine, 0, 0));
            return;
        }


    //    ShouldTargetingStillOccur();

        if (stateMachine.characterController.isGrounded)
        {
            if (previousState != null)
            {
                if (previousState is PlayerTargetingState)
                {
                    stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
                }
            }

            else
            {
                stateMachine.SwitchState(new PlayerLandingState(stateMachine, previousState));
                return;
            }
           
        }

        Vector3 move = CalculateMovement();

        Move(move * stateMachine.FreeLookMovementSpeed, deltaTime);
        FaceMovementDirection(move, deltaTime);

        if ((stateMachine.wallLeft || stateMachine.wallRight) && MinimumWalllRunHeight() && stateMachine.InputReader.MovementValue.y > 0 && !stateMachine.exitingWall)
        {
            
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
        
       // FaceTarget();
    }

    public override void Exit()
    {
        stateMachine.ledgeDetector.onLedgeDetect -= HandleLedgeDetection;
        stateMachine.InputReader.JumpEvent -= OnAirJump;
        stateMachine.InputReader.DodgeEvent -= OnDash;
    }

    private void HandleLedgeDetection( Vector3 ledgeForward, Vector3 closestPoint)
    {
        stateMachine.SwitchState(new PlayerHangingState(stateMachine, ledgeForward, closestPoint));
    }
   
    private void OnAirJump()
    {
        if (stateMachine.isAirJumpExhausted == false)
        {
            stateMachine.forceReceiver.Reset();
            stateMachine.SwitchState(new PlayerAirJumpState(stateMachine));
            return;
        }
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

    private void OnDash()
    {
        CheckToTeleportOrDash();

    }
}
