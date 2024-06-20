using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirJumpState : PlayerBaseState
{
    
    private readonly int PlayerAirJumpHash = Animator.StringToHash("DoubleJump");
    private const float CrossFadeDuration = 0.1f;
    private Vector3 momentum;
    private Vector3 airControl;
    public PlayerAirJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {

    }

    public PlayerAirJumpState(PlayerStateMachine stateMachine, State PreviousState) : base(stateMachine)
    {
        this.previousState = PreviousState;

    }

    public override void Enter()
    {
        Debug.Log("previous state post double jump is " + previousState);
        airControl = new Vector3(stateMachine.InputReader.MovementValue.x, 0f, stateMachine.InputReader.MovementValue.y);
        if(stateMachine.airJumpCounter >= 1) 
        {
            stateMachine.isAirJumpExhausted = true;
            stateMachine.SwitchState(new PlayerFallingState(stateMachine, previousState));
            return; 
        }
        else
        {
            stateMachine.airJumpCounter++;
            stateMachine.isAirJumpExhausted = false;

            stateMachine.ledgeDetector.onLedgeDetect += HandleLedgeDetection;
            stateMachine.InputReader.DodgeEvent += OnDash;
            stateMachine.forceReceiver.Jump(stateMachine.JumpForce);

            momentum = stateMachine.characterController.velocity + airControl;
            momentum.y = 0f;

            stateMachine.Animator.CrossFadeInFixedTime(PlayerAirJumpHash, CrossFadeDuration);
            
        }
    }

    public override void Tick(float deltaTime)
    {
        momentum = ApplyInputToMomentum(momentum, deltaTime);
        Move(momentum, deltaTime); // move based on momentum every frame
        CheckForWalll();
  
        if (stateMachine.characterController.velocity.y <= 0)
        {
            stateMachine.SwitchState(new PlayerFallingState(stateMachine));

            return;
        }
       
        Vector3 movement = CalculateMovement();
        Move(movement * stateMachine.FreeLookMovementSpeed, deltaTime);
        FaceMovementDirection(movement, deltaTime);
        if ((stateMachine.wallLeft || stateMachine.wallRight) && MinimumWalllRunHeight() && stateMachine.InputReader.MovementValue.y > 0 && !stateMachine.exitingWall)
        {

          //  Move(stateMachine.InputReader.MovementValue, deltaTime);
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
    }

    public override void Exit()
    {
        stateMachine.ledgeDetector.onLedgeDetect -= HandleLedgeDetection;
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
    private void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
        stateMachine.transform.rotation,
        Quaternion.LookRotation(movement),
        deltaTime * stateMachine.RotationDamping);
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
}
