using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using System;

public class PlayerDashingState : PlayerBaseState
{
    public static event Action<bool> OnActivateMeshTrail;
    public static event Action OnDashHasEnded;
    private readonly int DashingBlendTreeHash = Animator.StringToHash("placeholderDash");

    private readonly int DashForwardHash = Animator.StringToHash("GroundDash");

    private readonly int AirealForwardHash = Animator.StringToHash("AirDash");

    private readonly int DashRightHash = Animator.StringToHash("DodgeRight");

    private const float CrossFadeDuration = 0.1f;

    private Vector3 dashingDirectionInput;

    private float remainingDodgeTime;
    private bool isCurrentlyDashing;
    private float dashForceMultiplier = 5f;
    Vector3 delayedForceToApply;
    float applyDashToMovementSpeed;
    
    public PlayerDashingState(PlayerStateMachine stateMachine, Vector3 dashingDirectionInput) : base(stateMachine)
    {
        this.dashingDirectionInput = dashingDirectionInput;
    }

    public override void Enter()
    {
        isCurrentlyDashing = true;
        OnActivateMeshTrail?.Invoke(isCurrentlyDashing);
        // remainingDodgeTime = stateMachine.DodgeDuration;
        InputDash();
        //  stateMachine.Animator.SetFloat(DashForwardHash, dashingDirectionInput.y);
        // stateMachine.Animator.SetFloat(DashRightHash, dashingDirectionInput.x);
        if (stateMachine.characterController.isGrounded)
        {
            stateMachine.Animator.CrossFadeInFixedTime(DashForwardHash, CrossFadeDuration);
        }
        else
        {
            stateMachine.Animator.CrossFadeInFixedTime(AirealForwardHash, CrossFadeDuration);
        }
        
        
    }

  
    public override void Tick(float deltaTime)
    {
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Dash");
        // Vector3 dash = new Vector3();
        /* Vector3 dashForce = new Vector3(dashingDirectionInput.x * stateMachine.DodgeLength / stateMachine.DodgeDuration * dashForceMultiplier,
                                         0f,
                                         dashingDirectionInput.y * stateMachine.DodgeLength / stateMachine.DodgeDuration * dashForceMultiplier);

         Move(dashForce, deltaTime); // dodge depending on movement

         FaceTarget(); //ensure we still face target 

         remainingDodgeTime -= deltaTime;*/

        /* if (remainingDodgeTime <= 0f)
         {*/
        if (normalizedTime > 0.7f)
        {
            CheckForCombat();
        }

            if (normalizedTime < 1f)
        {
            stateMachine.FreeLookMovementSpeed += 10f;
            Vector3 movement = new Vector3();
            if (stateMachine.characterController.isGrounded)
            {
                movement += stateMachine.orientation.forward * stateMachine.dashForce + stateMachine.orientation.up * stateMachine.dashUpwardForce;
            }
            else
            {
               float appliedForce = stateMachine.dashUpwardForce;
                appliedForce = 2.5f;
                movement += stateMachine.orientation.forward * stateMachine.dashForce + stateMachine.orientation.up * appliedForce;
            }

            Move(movement, deltaTime); // dodge depending on movement
            applyDashToMovementSpeed = Mathf.Sqrt(movement.magnitude); // taking the magnitude of the resultant of the movement vector will give me a value that is more in line
                                                                                 //with the regular movement speed of my character instead of some insane push force. 

           
            ResetDash(); // for end of our dash
            if ((stateMachine.wallLeft || stateMachine.wallRight) && MinimumWalllRunHeight() && stateMachine.InputReader.MovementValue.y > 0 && !stateMachine.exitingWall)
            {

                // Move(stateMachine.InputReader.MovementValue, deltaTime);
                stateMachine.SwitchState(new playerWallRunningState(stateMachine));

                return;
            }
            return;
        }
        isCurrentlyDashing = false;
        if (stateMachine.dashCoolDownTimer > 0)
            stateMachine.dashCoolDownTimer -= deltaTime;

        if (stateMachine.characterController.isGrounded == false && (stateMachine.wallLeft || stateMachine.wallRight) && MinimumWalllRunHeight() && stateMachine.InputReader.MovementValue.y > 0 && !stateMachine.exitingWall)
        {

           // Move(stateMachine.InputReader.MovementValue, deltaTime);
            stateMachine.SwitchState(new playerWallRunningState(stateMachine));

            return;
        }
        if (!stateMachine.characterController.isGrounded)
        {
            stateMachine.SwitchState(new PlayerFallingState(stateMachine));

            return;
        }
        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        
    }

    public override void Exit()
    {
        isCurrentlyDashing = false;
        OnActivateMeshTrail?.Invoke(isCurrentlyDashing);
        OnDashHasEnded?.Invoke();
        stateMachine.FreeLookMovementSpeed = applyDashToMovementSpeed * 4f;
        Debug.Log("freelook speed is " + stateMachine.FreeLookMovementSpeed);
    }

    public void Dash() // this function should be somewhere else
    {
        stateMachine.isDashing = true;
        if (stateMachine.dashCoolDownTimer > 0) { return; }
        else stateMachine.dashCoolDownTimer = stateMachine.dashCoolDown;
        Vector3 forceToApply = stateMachine.orientation.forward * stateMachine.dashForce + stateMachine.orientation.up * stateMachine.dashUpwardForce;

        // stateMachine.forceReceiver.DashForce(forceToApply, ForceMode.Impulse);
        delayedForceToApply = forceToApply;
        Move(forceToApply, Time.deltaTime);
       // DelayedDashForce();
        ResetDash(); // for end of our dash
    }

    public void InputDash()
    {
        stateMachine.isDashing = true;
        if (stateMachine.dashCoolDownTimer > 0) { return; }
        else stateMachine.dashCoolDownTimer = stateMachine.dashCoolDown;
        
        
    }

    public void DelayedDashForce()
    {
        stateMachine.forceReceiver.DashForce(delayedForceToApply, ForceMode.Impulse);
    }
    protected void ResetDash()
    {
        stateMachine.isDashing = false;
    }
    private void CheckForCombat()
    {
            if (stateMachine.InputReader.isBasicAttack ||
                stateMachine.InputReader.isHeavyAttack ||
                stateMachine.InputReader.isBasicHoldAttack ||
                stateMachine.InputReader.isHeavyHoldAttack)
            {
                stateMachine.SwitchState(new PlayerDashAttackState(stateMachine, CalculateMovement()));
            }
        
    }

    }
