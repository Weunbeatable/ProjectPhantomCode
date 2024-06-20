using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerHoldAirHeavyAttack : PlayerBaseState
{
    private float previousFrameTime; // in case we get data from final frame of previous animation

    private bool alreadyAppliedForce;

    private Attack attack; // setting a private field of type attack
    private readonly int HoldTimeSpeedHash = Animator.StringToHash("HoldInput");
    int attackCounter;
    public PlayerHoldAirHeavyAttack(PlayerStateMachine stateMachine, int AttackIndex) : base(stateMachine) // adding attack ID into constructor so we know which attack to use
    {
        if (stateMachine.InputReader.isBasicHoldAttack)
        {
            attack = stateMachine.BasicHoldAttack[AttackIndex]; // assigning the attack value to the attack id when we pass this information into other states in statemachine
            FaceTarget();
            Debug.Log("Basic attack index" + attack.ComboStateIndex);
            Debug.Log("Attackname is " + attack.AnimationName);
        }

        /*if (stateMachine.InputReader.isBasicHoldAttack && AttackIndex > 0)
        {
            AttackIndexHeavy = attack.ComboStateIndex;

        }*/
    }
    public override void Enter()
    {

        stateMachine.isStartingComboCounter = false;
        attackCounter = 0;

        stateMachine.Weapon.SetAttack(attack.Damage, attack.Knockback);
       /* stateMachine.headDamage.SetAttack(attack.Damage, attack.Knockback);
        stateMachine.rightHandDamage.SetAttack(attack.Damage, attack.Knockback);
        stateMachine.leftHandDamage.SetAttack(attack.Damage, attack.Knockback);
        stateMachine.rightLegDamage.SetAttack(attack.Damage, attack.Knockback);
        stateMachine.leftLegDamage.SetAttack(attack.Damage, attack.Knockback);*/
        stateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, attack.TransitionDuration); // crossfade in fixed time is better than play so we get smootheranimations
        FaceTarget();
        MoveTowardsTarget();
        PhantomAttackHandler(attack);
        stateMachine.Animator.applyRootMotion = true;
        // Debug.Log("current animation controller is " + stateMachine.Animator.runtimeAnimatorController.name);

        //  playStoredAttacks();
    }

    public override void Tick(float deltaTime)
    {

        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Attack");
        // redundant normalizedTime >= previousFrameTime &&
        if (normalizedTime < 1f) // if greater than previous do something. if greater than 1 animation has finished, may remove the && for animation cancel
        {

            if (stateMachine.InputReader.isBasicHoldAttack)
            {

                attackCounter++;
                if (attackCounter > 1)
                {
                    stateMachine.isStartingComboCounter = true;
                }
                FaceTarget();

                TryComboAttack(normalizedTime);

                //Move(stateMachine.InputReader.MovementValue, deltaTime);
            }
            /* else if (stateMachine.InputReader.isBasicAttack)
             {
                 attackCounter++;
                 if (attackCounter > 1)
                 {
                     stateMachine.isStartingComboCounter = true;
                 }
                 FaceTarget();

                 TryTapComboAttack(normalizedTime);
             }*/
        }
        else
        {
            if (stateMachine.targeter.currentTarget != null)
            {
                attackCounter = 0;
                stateMachine.isStartingComboCounter = false;
                stateMachine.SwitchState(new PlayerTargetingState(stateMachine)); //switch to targeting state
            }
            else
            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine)); // free look state
            }
            stateMachine.Animator.SetFloat(HoldTimeSpeedHash, 0f);
        }

        if (normalizedTime > attack.ForceTime)
        {

            TryApplyForce();
        }
        previousFrameTime = normalizedTime;
    }


    public override void Exit()
    {


    }
    private void OnTarget()
    {
        stateMachine.targeter.Cancel();

        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
    }
    private void TryApplyForce()
    {
        if (alreadyAppliedForce) { return; }
        FaceTarget();
        stateMachine.forceReceiver.AddForce(stateMachine.transform.forward * attack.Force);

        alreadyAppliedForce = true;
    }

    private void TryComboAttack(float normalizedTime)
    {
        if (attack.ComboStateIndex == -1) { return; } // making sure we have a combo attack

        if (normalizedTime < attack.ComboAttackTime) { return; } // ensure we are far enough through the combo to do it
        stateMachine.SwitchState // If we are far enough through switch the state to attack.
            (
                new PlayerHoldAirHeavyAttack
                (
                    stateMachine,
                    attack.ComboStateIndex
                )
            );
    }
    private void TryTapComboAttack(float normalizedTime)
    {
        if (attack.ComboStateIndex == -1) { return; } // making sure we have a combo attack

        if (normalizedTime < attack.ComboAttackTime) { return; } // ensure we are far enough through the combo to do it

        stateMachine.SwitchState // If we are far enough through switch the state to attack.
        (
            new PlayerAttackingState
            (
                stateMachine,
                attack.ComboStateIndex
            )
        );

    }

    void MoveTowardsTarget()
    {
        Vector3 offset = new Vector3(1f, 0f, 1f);
        Vector3 controllerInput = new Vector3(stateMachine.InputReader.MovementValue.x, 0, stateMachine.InputReader.MovementValue.y);
        if (stateMachine.targeter.currentTarget == null && stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.characterController.transform.rotation = Quaternion.LookRotation(stateMachine.transform.forward);
            return;
        }
        else if (stateMachine.targeter.currentTarget == null && stateMachine.InputReader.MovementValue != Vector2.zero)
        {
            stateMachine.characterController.transform.rotation = Quaternion.LookRotation(CalculateMovement());
        }
        else
        {
            stateMachine.characterController.transform.DORotate(stateMachine.targeter.currentTarget.transform.position, .1f);
            stateMachine.characterController.transform.DOLookAt(stateMachine.targeter.currentTarget.transform.position, .1f);

            // stateMachine.characterController.transform.DOMove(stateMachine.targeter.currentTarget.transform.position - offset, .4f);
        }
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
