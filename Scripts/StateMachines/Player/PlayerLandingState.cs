using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandingState : PlayerBaseState
{
    public static event Action onlanded;

    private readonly int PlayerNoInputLand = Animator.StringToHash("Land_Spawn_Wait");
    private readonly int playerMovingLowVelocityLand = Animator.StringToHash("Land_Base_Move_Root");
    private readonly int playerMovingMidVelocityLand = Animator.StringToHash("Land_Roll_Move");
    private readonly int playerMovingHighVelocityInputLand = Animator.StringToHash("Land_Stumble_Move_Root");

    private Vector3 momentum;

    private const float CrossFadeDuration = 0.1f;
    private bool noInputLanding = false;
    private bool InputLanding = false;
    private bool lowMovement = false;

    Vector3 velocity;
    public PlayerLandingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public PlayerLandingState(PlayerStateMachine stateMachine, State PreviousState) : base(stateMachine)
    {
        this.previousState = PreviousState;

    }


    public override void Enter()
    {
        ShouldTargetingStillOccur();
        // reading player velocity 
        velocity = stateMachine.characterController.velocity;

        if (stateMachine.characterController.isGrounded)
        {
            onlanded?.Invoke();
            if (stateMachine.InputReader.MovementValue == Vector2.zero)
            {
                noInputLanding = true;
                stateMachine.Animator.CrossFadeInFixedTime(PlayerNoInputLand, CrossFadeDuration);

            }
            else 
            {
                
                if(Mathf.Abs( velocity.z) <= 5.5f && Mathf.Abs(velocity.z) > 0f)
                {
                    InputLanding = true;
                    lowMovement = false;
                    stateMachine.Animator.CrossFadeInFixedTime(playerMovingLowVelocityLand, CrossFadeDuration);
                }
                else if(Mathf.Abs(velocity.z) <= 19 && Mathf.Abs(velocity.z) > 5.5f)
                {
                    InputLanding = true;
                    lowMovement = false;
                    stateMachine.Animator.CrossFadeInFixedTime(playerMovingMidVelocityLand, CrossFadeDuration);
                }
                // add another animation  if velocity is way  too high then default to high velocity, another layer should be added though so if not too fast roll happens and if slow just a regular run happens.
                else
                {
                    lowMovement = true;
                    InputLanding = true;
                    stateMachine.Animator.CrossFadeInFixedTime(playerMovingHighVelocityInputLand, CrossFadeDuration);
                }
                

            }   
        }
       
    }


    public override void Tick(float deltaTime)
    {
        Vector3 move = CalculateMovement();
        var normalizedTime = GetNormalizedTime(stateMachine.Animator, "Landing") < 1f;
        if (GetNormalizedTime(stateMachine.Animator, "Landing") < 1f)
        {
            if (noInputLanding)
            {
                FaceMovementDirection(move, deltaTime);
            }
            if (InputLanding)
            {
                if (!lowMovement)
                {
                    MoveWithDIrectionFace(move * stateMachine.FreeLookMovementSpeed, deltaTime);
                    //Move(move  * stateMachine.FreeLookMovementSpeed, deltaTime);
                    //FaceMovementDirection(move, deltaTime);

                }
                else
                {
                    MoveWithDIrectionFace(move * stateMachine.FreeLookMovementSpeed, deltaTime);
                    //Move(move  * stateMachine.FreeLookMovementSpeed / 0.5f, deltaTime);
                    //FaceMovementDirection(move, deltaTime);

                }
             
            }
           
            
        }
        else
        {
            
            ReturnToLocomotion();
        }
    }

    public override void Exit()
    {
        lowMovement = false;
        noInputLanding = false;
        InputLanding = false;
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
