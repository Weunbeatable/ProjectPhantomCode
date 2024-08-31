using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingState : PlayerBaseState
{
    public static event Action OnTriggerConfusion;
    public static event Action<Transform> OnTriggerLockOnIcon;

    private readonly int TargetingBlendTreeHash = Animator.StringToHash("TargetingBlendTree");

    private readonly int TargetingForwardHash = Animator.StringToHash("TargetingForward");

    private readonly int TargetingRightHash = Animator.StringToHash("TargetingRight");

    private const float AnimatorDampTime = 0.1f;

    private const float CrossFadeDuration = 0.1f;

    // Special dodge parameters
    private float specialDodgeDistance = 1.2f;

    private bool isInPhantomDodgeTime;

    private float teleportPositionDecisionTimer = 2f;

    bool checkLeft, checkRight;
    public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        if (stateMachine.airJumpCounter >= 1)
        {
            stateMachine.airJumpCounter = 0;
            stateMachine.isAirJumpExhausted = false;
        }
        stateMachine.InputReader.TargetEvent += OnTarget;
        stateMachine.InputReader.DodgeEvent += OnDodge;
        stateMachine.InputReader.JumpEvent += OnJump;
        stateMachine.InputReader.TauntEvent += OnTaunt;
        stateMachine.InputReader.FinishEvent += OnFinish;


        stateMachine.Animator.CrossFadeInFixedTime(TargetingBlendTreeHash, CrossFadeDuration);
        stateMachine.Animator.applyRootMotion = true;
        if(stateMachine.targeter.currentTarget != null)
        {
            OnTriggerLockOnIcon?.Invoke(stateMachine.targeter.currentTarget.transform);
        }
    }

    public override void Tick(float deltaTime)
    {
        /*Debug.Log("input value " + stateMachine.InputReader.MovementValue.x);
        Debug.Log("input value Y " + stateMachine.InputReader.MovementValue.y);*/
        if (isInPhantomDodgeTime == true)
        {
            stateMachine.playerEffects.SwapToDissolve();
            stateMachine.health.setInVulnerable(true);
            Time.timeScale = 0.25f;
            float normalizedTime = GetNormalizedTime(stateMachine.dodgeObjects[0].GetComponent<Animator>(), "Dodge");
            if (normalizedTime > 1f) // upon finishing dodge animation allow choosing a place to teleport to. 
            {
                teleportPositionDecisionTimer -= Time.fixedDeltaTime;
                // Debug.Log("Decision time is " + teleportPositionDecisionTimer);
                stateMachine.playerEffects.PlayDissolveEffect();
                if (teleportPositionDecisionTimer > 0f)
                {

                    DirectionToTeleportTo();
                }
                else if (teleportPositionDecisionTimer <= 0f) // if you don't choose a place to teleport to. 
                {
                    Time.timeScale = 1f;
                    // Handle time returning to usual
                    stateMachine.health.setInVulnerable(false);
                    stateMachine.playerEffects.SwapBackToOriginal();
                    stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
                }

            }
        }
        else
        {
            if (stateMachine.InputReader.LookValue.x > 0f)
            {
                stateMachine.targeter.CheckForTargetOnRight();
                return;
            }
            if ( stateMachine.InputReader.LookValue.x < 0f)
            {
                stateMachine.targeter.CheckForTargetOnLeft();
                return;
            }


            
            TargetingAttackingCheck();

            if (stateMachine.InputReader.isBlocking) // go into attacking state if true
            {
                stateMachine.SwitchState(new PlayerBlockingState(stateMachine));
                return;
            }

            if (stateMachine.targeter.currentTarget == null)
            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
                return; // making sure we odn't run anything else if this If statement runs
            }

            Vector3 movement = CalculateMovement(deltaTime);
            Move(movement * stateMachine.TargetingMovementSpeed, deltaTime);

            UpdateAnimator(deltaTime);

            FaceTarget();
        }

    }

    public override void Exit()
    {       
        foreach (GameObject dodge in stateMachine.dodgeObjects)
        {
            dodge.SetActive(false);

        }
       // stateMachine.playerEffects.SwapBackToOriginal();
        stateMachine.InputReader.TargetEvent -= OnTarget;
        stateMachine.InputReader.DodgeEvent -= OnDodge;
        stateMachine.InputReader.JumpEvent -= OnJump;
        stateMachine.InputReader.TauntEvent -= OnTaunt;
        stateMachine.InputReader.FinishEvent -= OnFinish;
    }

    private void OnTaunt()
    {
        stateMachine.SwitchState(new PlayerTauntState(stateMachine));
        // making sure we odn't run anything else if this If statement runs
    }
    private void OnJump()
    {
        stateMachine.SwitchState(new PlayerJumpingState(stateMachine, this));
         // making sure we odn't run anything else if this If statement runs
    }
    private void OnDodge()
    {
        //Evaluate what kind of dodge is being done. 
        if (stateMachine.targeter.currentTarget.GetComponent<EnemyStateMachine>().isInAttackingRange == true
            && stateMachine.targeter.currentTarget.GetComponent<EnemyStateMachine>().handler.GetAttackingState() == true)
        {
            stateMachine.sfx.SpecialDodge();
            Vector3 distance = (stateMachine.characterController.transform.position - stateMachine.targeter.currentTarget.transform.position);
            float distanceCheck = distance.sqrMagnitude;
            if (distanceCheck < specialDodgeDistance)
            {
                isInPhantomDodgeTime = true;
                int DodgingBlendTreeHash = Animator.StringToHash("EvasionBlendTree");

                int DodgeForwardHash = Animator.StringToHash("DodgeForward");

                int DodgeRightHash = Animator.StringToHash("DodgeRight");
                stateMachine.Animator.applyRootMotion = false;
                stateMachine.Animator.CrossFadeInFixedTime(DodgingBlendTreeHash, 0.05f);
                PhantomDodge();
            }
        }
        else
        {
            stateMachine.SwitchState(new PlayerDodgingState(stateMachine, stateMachine.InputReader.MovementValue)); // flexibility if we wantto dodge with something other than movement value
        }
    }
    private void OnTarget()
    {
        stateMachine.targeter.Cancel();

        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
    }
    private void OnFinish()
    {
        if (stateMachine.targeter.currentTarget != null)
        {
            float distanceToAllowFinisher = Vector3.Distance(stateMachine.characterController.transform.position,
                   stateMachine.targeter.currentTarget.transform.position);
            if (distanceToAllowFinisher < 2f)
            {
                if (stateMachine.targeter.GetFinishState() == false)
                {
                    return;
                }
                else if (stateMachine.targeter.GetFinishState() == true)
                {
                    Debug.Log("Finish Him");
                    stateMachine.SwitchState(new PlayerFinisherState(stateMachine));
                    return;
                }
            }
        }
    }
    private Vector3 CalculateMovement(float deltaTime)
    {
        Vector3 movement = new Vector3(); // holding right we move clockwise around target, left is anti-clockwises

            movement += stateMachine.transform.right * stateMachine.InputReader.MovementValue.x;
            movement += stateMachine.transform.forward * stateMachine.InputReader.MovementValue.y;

        return movement;
    }

    private void UpdateAnimator(float deltaTime)
    {
         if(stateMachine.InputReader.MovementValue.y == 0)
        {
            stateMachine.Animator.SetFloat(TargetingForwardHash, 0, AnimatorDampTime, deltaTime);
        }
        else
        {
            float value = stateMachine.InputReader.MovementValue.y > 0f ? 1f : -1f; // one liner if else for non monobehavior scripts checks input reader value, if greater than 0 set to 1f if less than 0 set to -1f
            stateMachine.Animator.SetFloat(TargetingForwardHash, value, AnimatorDampTime, deltaTime);
        }

        if (stateMachine.InputReader.MovementValue.x == 0)
        {
            stateMachine.Animator.SetFloat(TargetingRightHash, 0, AnimatorDampTime, deltaTime);
        }
        else
        {
            float value = stateMachine.InputReader.MovementValue.x > 0f ? 1f : -1f; // one liner if else for non monobehavior scripts checks input reader value, if greater than 0 set to 1f if less than 0 set to -1f
            stateMachine.Animator.SetFloat(TargetingRightHash, value, AnimatorDampTime, deltaTime);
        }

    }

    private void TargetingAttackingCheck()
    {
        if (stateMachine.InputReader.isBasicAttack)
        {

            stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
            return;
        }
        if (stateMachine.InputReader.isHeavyAttack)
        {

            stateMachine.SwitchState(new PlayerHeavyAttackingState(stateMachine, 0));
            return;
        }
        if (stateMachine.InputReader.isBasicHoldAttack)
        {

            stateMachine.SwitchState(new PlayerHoldBasicAttack(stateMachine, 0));
            return;
        }
        if (stateMachine.InputReader.isHeavyHoldAttack)
        {

            stateMachine.SwitchState(new PlayerHoldHeavyAttack(stateMachine, 0));
            return;
        }
    }

    private void DirectionToTeleportTo()
    {

        // Y Values
        if (stateMachine.InputReader.MovementValue.y > 0.85)
        {
            ChosenPosition(stateMachine.dodgeObjects[0].transform.position);
            stateMachine.health.setInVulnerable(false);
            Time.timeScale = 1f;
            OnTriggerConfusion?.Invoke();
            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
            foreach (GameObject dodge in stateMachine.dodgeObjects)
            {
                dodge.SetActive(false);

            }
            stateMachine.playerEffects.SwapBackToOriginal();
        }

        if (stateMachine.InputReader.MovementValue.y < -0.85)
        {
            ChosenPosition(stateMachine.dodgeObjects[1].transform.position);
            stateMachine.health.setInVulnerable(false);
            Time.timeScale = 1f;
            OnTriggerConfusion?.Invoke();
            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
            foreach (GameObject dodge in stateMachine.dodgeObjects)
            {
                dodge.SetActive(false);

            }
            stateMachine.playerEffects.SwapBackToOriginal();
        }

        // X Values
        if (stateMachine.InputReader.MovementValue.x > 0.85)
        {
            ChosenPosition(stateMachine.dodgeObjects[2].transform.position);
            stateMachine.health.setInVulnerable(false);
            Time.timeScale = 1f;
            OnTriggerConfusion?.Invoke();
            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
            foreach (GameObject dodge in stateMachine.dodgeObjects)
            {
                dodge.SetActive(false);

            }
            stateMachine.playerEffects.SwapBackToOriginal();
        }

        if (stateMachine.InputReader.MovementValue.x < -0.85)
        {
            ChosenPosition(stateMachine.dodgeObjects[3].transform.position);
            stateMachine.health.setInVulnerable(false);
            Time.timeScale = 1f;
            OnTriggerConfusion?.Invoke();
            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
            foreach (GameObject dodge in stateMachine.dodgeObjects)
            {
                dodge.SetActive(false);

            }
            stateMachine.playerEffects.SwapBackToOriginal();
        }
    }

    private void ChosenPosition(Vector3 PhantomPosition)
    {
        stateMachine.characterController.enabled = false;
        stateMachine.transform.position = PhantomPosition;
        //stateMachine.transform.rotation = stateMachine.lastKnownRotation; Might Handle rotation after testing
        stateMachine.characterController.enabled = true;
    }


    private void PhantomDodge()
    {
        foreach (GameObject dodge in stateMachine.dodgeObjects)
        {
            dodge.GetComponent<Animator>().runtimeAnimatorController = stateMachine.Animator.runtimeAnimatorController;
            dodge.SetActive(true);

        }
        SetPositions(stateMachine.dodgeObjects[0], 0, 1);
        SetPositions(stateMachine.dodgeObjects[1], 0, -1);
        SetPositions(stateMachine.dodgeObjects[2], 1, 0);
        SetPositions(stateMachine.dodgeObjects[3], -1, 0);
    }
    public void SetPositions(GameObject dodgetemplate, float dodgevalue, float dodgeRightValue)
    {
        dodgetemplate.transform.position = stateMachine.transform.position;
        int DodgingBlendTreeHash = Animator.StringToHash("EvasionBlendTree");

        int DodgeForwardHash = Animator.StringToHash("DodgeForward");

        int DodgeRightHash = Animator.StringToHash("DodgeRight");

        dodgetemplate.GetComponent<Animator>().SetFloat(DodgeForwardHash, dodgevalue);
        dodgetemplate.GetComponent<Animator>().SetFloat(DodgeRightHash, dodgeRightValue);

        dodgetemplate.GetComponent<Animator>().applyRootMotion = true;
        dodgetemplate.GetComponent<Animator>().Play(DodgingBlendTreeHash);

    }

}
