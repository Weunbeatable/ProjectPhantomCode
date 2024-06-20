using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class PlayerFreeLookState : PlayerBaseState, ICriticalCondition
{
    public static event Action<bool> OnSwitchToWeaponEquip; // If I haven't switched to another state... stay enabled. 
    public static event Action<bool, bool> OnTriggerFootsteps;

    private bool shouldFade;

    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed"); // storing hash for free look spee dparameter
    private readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLookBlendTree");
    private readonly int FreeLookWeaponEquippedBlendTreeHash = Animator.StringToHash("FreeLookWeaponActiveBlendTree");
    private readonly int FreeLookCriticalConditionBlendTreeHash = Animator.StringToHash("FreeLookCriticalStateBlendTree");
    private  int BlendTreeHandle;
    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;

    private bool isStillInPlayerFreelookState;
    private float originalFreelookMovementSpeed;
    private bool isSwapping;
    private bool iswalking, isrunning;
    
    CinemachineStateDrivenCamera cinemachine;
    private State state;

    public PlayerFreeLookState(PlayerStateMachine stateMachine, bool shouldFade = true) : base(stateMachine) // bool is an optional parameter so we dont have to add this everywhere, these are added after required params
    {
        this.shouldFade = shouldFade;
    }

    public PlayerFreeLookState(PlayerStateMachine stateMachine, State previousState) : base(stateMachine)
    {
        this.state = previousState;
        // TODO State check, if coming from freelook state, play the ready anim from that particular state. 
    }



    // private float timeInState = 0.0f;
    public override void Enter()
    {

        stateMachine.targeter.Cancel();
        Debug.Log("Current stat is the " + stateMachine + " State");
       // originalFreelookMovementSpeed = MathF.Abs(stateMachine.characterController.velocity.z);
        stateMachine.combatModifiers.modifiedKnockBack = 0f;
        stateMachine.InputReader.TargetEvent += Ontarget;
        stateMachine.InputReader.JumpEvent += OnJump;
        stateMachine.InputReader.TauntEvent += OnTaunt;
        stateMachine.InputReader.DodgeEvent += OnDash;
        stateMachine.InputReader.FinishEvent += OnFinish;
        PlayerCombatTimers.OnCallToSwapBlendTrees += SwapTree;
        SwapTree(isSwapping);
        if (stateMachine.airJumpCounter >= 1)
        {
            stateMachine.airJumpCounter = 0;
            stateMachine.isAirJumpExhausted = false;
        }


        stateMachine.Animator.applyRootMotion = false;

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0);
        }
        else if(stateMachine.FreeLookMovementSpeed < 19)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0.75f);
        }
        else
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 2f);
        }
            

        if (shouldFade)
        {
            stateMachine.Animator.CrossFadeInFixedTime(BlendTreeHandle, CrossFadeDuration);
        }
        else
        {
            stateMachine.Animator.Play(BlendTreeHandle);
        }
        isStillInPlayerFreelookState = true;
        OnSwitchToWeaponEquip?.Invoke(isStillInPlayerFreelookState);
        
    }

    public override void Tick(float deltaTime)
    {
        /*Debug.Log("x Value is " + stateMachine.InputReader.LookValue.x +
                  " y Value is " + stateMachine.InputReader.LookValue.y);*/
        CheckForCombat();
        SwapTree(isSwapping);
            if (stateMachine.parkour.lasthit != null)
        {
            if (stateMachine.parkour.GetObjectParkourStatus() == 11 && stateMachine.InputReader.MovementValue.y > 0
                || stateMachine.parkour.GetObjectParkourStatus() == 12 && stateMachine.InputReader.MovementValue.y > 0
                || stateMachine.parkour.GetObjectParkourStatus() == 13 && stateMachine.InputReader.MovementValue.y > 0)
            {
                stateMachine.SwitchState(new PlayerParkourState(stateMachine, stateMachine.InputReader.MovementValue));
                return;
            }
        }

        if (stateMachine.dashCoolDownTimer > 0)
            stateMachine.dashCoolDownTimer -= deltaTime;
        Vector3 movement = CalculateMovement();
       // Debug.Log($"Current x value is is sitting at {stateMachine.InputReader.MovementValue.x} and current Y value is at {stateMachine.InputReader.MovementValue.y}");

        Move(movement * stateMachine.FreeLookMovementSpeed, deltaTime);

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
           // stateMachine.FreeLookMovementSpeed = 13f;
           if(stateMachine.FreeLookMovementSpeed > 10f)
            {
                stateMachine.FreeLookMovementSpeed = 10f;
            }
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            stateMachine.isRunning = false;
            return;
        } // no calculations if not moving
        else
        {
            OnTriggerFootsteps?.Invoke(iswalking, isrunning);
        }
        if(stateMachine.health.CriticalHealthPercentage() == false)
        {
            MoveTransitions(deltaTime);
            stateMachine.Animator.SetBool("Critical", false);
        }
        else
        {
            CriticalCondition(deltaTime);
            stateMachine.Animator.SetBool("Critical", true);
        }


        FaceMovementDirection(movement, deltaTime);
    }

 
    public override void Exit()
    {
        isStillInPlayerFreelookState = false;
        stateMachine.InputReader.TargetEvent -= Ontarget; // Entering free look blend tree
        stateMachine.InputReader.JumpEvent -= OnJump;
        stateMachine.InputReader.TauntEvent -= OnTaunt;
        stateMachine.InputReader.DodgeEvent -= OnDash;
        stateMachine.InputReader.FinishEvent -= OnFinish;
    }
    private void SwapTree(bool SwapBlendTree)
    {
        isSwapping = SwapBlendTree;
        if(stateMachine.Animator != null)
        {
            if (SwapBlendTree == false) // Set to unequipped blend tree
            {
                BlendTreeHandle = FreeLookBlendTreeHash;
                stateMachine.Animator.SetBool("WeaponBlendTree", false);
                // Debug.Log("Unequipped freelooking");
                return;
            }
            else
            {
                if (stateMachine.health.CriticalHealthPercentage() == false)
                {
                    BlendTreeHandle = FreeLookWeaponEquippedBlendTreeHash;
                    stateMachine.Animator.SetBool("WeaponBlendTree", true);
                    return;
                }
                else
                {
                    BlendTreeHandle = FreeLookCriticalConditionBlendTreeHash;
                    stateMachine.Animator.SetBool("WeaponBlendTree", false);
                    return;
                }
            }
        }
       
    }
    private void RunTransitionValueCheck(float deltaTime)
    {
        if (stateMachine.health.CriticalHealthPercentage() == false)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 1, AnimatorDampTime, deltaTime); // setting free look hash
            stateMachine.FreeLookMovementSpeed += 1.05f * deltaTime; // will remove hard coded value after testing. 
            if (stateMachine.FreeLookMovementSpeed >= 27)
            {
                stateMachine.FreeLookMovementSpeed -= 1.5f * deltaTime;
            }
        }
        else
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 1, AnimatorDampTime, deltaTime); // setting free look hash
            stateMachine.FreeLookMovementSpeed += 1.84f * deltaTime; // will remove hard coded value after testing. 
            if (stateMachine.FreeLookMovementSpeed >= 16f)
            {
                stateMachine.FreeLookMovementSpeed -= 3f * deltaTime;
            }
        }
    }

    private void JogTransitionValueCheck(float deltaTime)
    {
        if (stateMachine.health.CriticalHealthPercentage() == false)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0.75f, AnimatorDampTime, deltaTime); // setting free look hash
            stateMachine.FreeLookMovementSpeed += 0.85f * deltaTime;
        }
        else
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0.75f, AnimatorDampTime, deltaTime); // setting free look hash
            stateMachine.FreeLookMovementSpeed += 0.35f * deltaTime;
        }
    }

    private void FastWalkTransitionValueCheck(float deltaTime)
    {
        if (stateMachine.health.CriticalHealthPercentage() == false)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0.5f, AnimatorDampTime, deltaTime); // setting free look hash
            stateMachine.FreeLookMovementSpeed += 2.3f * deltaTime; // will remove hard coded value after testing. 
        }
        else
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0.5f, AnimatorDampTime, deltaTime);
            stateMachine.FreeLookMovementSpeed += 0.73f * deltaTime;

        }
    }
    private void WalkTransitionValueCheck(float deltaTime)
    {
        stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0.25f, AnimatorDampTime, deltaTime); // setting free look hash
    }
    private void MoveTransitions(float deltaTime)
    {
        if (stateMachine.InputReader.MovementValue.x > 0f && stateMachine.InputReader.MovementValue.x < 0.10f && stateMachine.FreeLookMovementSpeed <= 10f ||
             stateMachine.InputReader.MovementValue.x < 0f && stateMachine.InputReader.MovementValue.x > -0.10f && stateMachine.FreeLookMovementSpeed <= 10f ||
             stateMachine.InputReader.MovementValue.y > 0f && stateMachine.InputReader.MovementValue.y < 0.10f && stateMachine.FreeLookMovementSpeed <= 10f ||
             stateMachine.InputReader.MovementValue.y < 0f && stateMachine.InputReader.MovementValue.y > -0.10f && stateMachine.FreeLookMovementSpeed <= 10f)
        {
            iswalking = true;
            isrunning = false;
            WalkTransitionValueCheck(deltaTime);
        }
        else if (stateMachine.InputReader.MovementValue.x > 0.10f ||
             stateMachine.InputReader.MovementValue.x < -0.10f ||
             stateMachine.InputReader.MovementValue.y > 0.10f ||
             stateMachine.InputReader.MovementValue.y < -0.10f)
        {
            if (stateMachine.FreeLookMovementSpeed < 13f)
            {
                FastWalkTransitionValueCheck(deltaTime);
                iswalking = true;
                isrunning = false;
            }
            else if (stateMachine.FreeLookMovementSpeed < 16f)
            {
                JogTransitionValueCheck(deltaTime);
                iswalking = false;
                isrunning = true;
            }
            else
            {
                iswalking = false;
                isrunning = true;
                RunTransitionValueCheck(deltaTime);
            }
               

        }
    }


    private void OnTaunt()
    {
        isStillInPlayerFreelookState = false;
        stateMachine.SwitchState(new PlayerTauntState(stateMachine));
        // making sure we odn't run anything else if this If statement runs
    }
    private void OnJump()
    {
        isStillInPlayerFreelookState = false;
        stateMachine.SwitchState(new PlayerJumpingState(stateMachine));
         // making sure we odn't run anything else if this If statement runs
    }
    private void Ontarget()
    {
        isStillInPlayerFreelookState = false;
        if (!stateMachine.targeter.SelectTarget()) { return; }
        stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
    }
    private void OnDash()
    {
        isStillInPlayerFreelookState = false;
        CheckToTeleportOrDash();
        
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

    private void CheckForCombat()
    {

        if(stateMachine.FreeLookMovementSpeed > 16.5f)
        {
            if (stateMachine.InputReader.isBasicAttack ||  
                stateMachine.InputReader.isHeavyAttack ||
                stateMachine.InputReader.isBasicHoldAttack ||
                stateMachine.InputReader.isHeavyHoldAttack)
            {
                stateMachine.SwitchState(new PlayerDashAttackState(stateMachine,CalculateMovement()));
            }
        }
        else
        {
            if(stateMachine.InputReader.isBasicAttack) // go into attacking state if true
        {
                isStillInPlayerFreelookState = false;
                stateMachine.targeter.SelectTarget();
                stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
                return;
            }
            if (stateMachine.InputReader.isHeavyAttack)
            {
                isStillInPlayerFreelookState = false;
                stateMachine.targeter.SelectTarget();
                stateMachine.SwitchState(new PlayerHeavyAttackingState(stateMachine, 0));
                return;
            }

            if (stateMachine.InputReader.isBasicHoldAttack)
            {
                isStillInPlayerFreelookState = false;
                stateMachine.targeter.SelectTarget();
                stateMachine.SwitchState(new PlayerHoldBasicAttack(stateMachine, 0));
                return;
            }
            if (stateMachine.InputReader.isHeavyAttack)
            {
                isStillInPlayerFreelookState = false;
                stateMachine.targeter.SelectTarget();
                stateMachine.SwitchState(new PlayerHeavyAttackingState(stateMachine, 0));
                return;
            }
        }
    }
    private void OnFinish()
    {
        stateMachine.targeter.SelectTarget();
        if (stateMachine.targeter.currentTarget != null)
        {
            float distanceToAllowFinisher = Vector3.Distance(stateMachine.characterController.transform.position,
                   stateMachine.targeter.currentTarget.transform.position);
            if (distanceToAllowFinisher < 2f && stateMachine.targeter.GetFinishState() == true)
            {
                Debug.Log("Finish Him");
                stateMachine.targeter.SetFinishState(false);
                stateMachine.SwitchState(new PlayerFinisherState(stateMachine));
                return;
            }
            else if (stateMachine.targeter.GetFinishState() == false)
            {
                return;
            }
        }
        
    }

    public virtual void CriticalCondition(float deltaTime)
    {
        stateMachine.Animator.SetBool("Critical", true);
        BlendTreeHandle = FreeLookCriticalConditionBlendTreeHash;
            stateMachine.Animator.SetBool("WeaponBlendTree", false);

        if (stateMachine.InputReader.MovementValue.x > 0f && stateMachine.InputReader.MovementValue.x < 0.15f && stateMachine.FreeLookMovementSpeed <= 10f ||
             stateMachine.InputReader.MovementValue.x < 0f && stateMachine.InputReader.MovementValue.x > -0.15f && stateMachine.FreeLookMovementSpeed <= 10f ||
             stateMachine.InputReader.MovementValue.y > 0f && stateMachine.InputReader.MovementValue.y < 0.15f && stateMachine.FreeLookMovementSpeed <= 10f ||
             stateMachine.InputReader.MovementValue.y < 0f && stateMachine.InputReader.MovementValue.y > -0.15f && stateMachine.FreeLookMovementSpeed <= 10f)
        {
            iswalking = true;
            isrunning = false;
            WalkTransitionValueCheck(deltaTime);
        }
        else if (stateMachine.InputReader.MovementValue.x > 0.15f ||
             stateMachine.InputReader.MovementValue.x < -0.15f ||
             stateMachine.InputReader.MovementValue.y > 0.15f ||
             stateMachine.InputReader.MovementValue.y < -0.15f)
        {
            if (stateMachine.FreeLookMovementSpeed < 14f)
            {
                iswalking = true;
                isrunning = false;
                FastWalkTransitionValueCheck(deltaTime);
            }
            else if (stateMachine.FreeLookMovementSpeed < 16f)
            {
                iswalking = false;
                isrunning = true;
                JogTransitionValueCheck(deltaTime);
            }
        }
        return;
    }
}
