using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAirAttackingState : PlayerBaseState
{
    private float previousFrameTime; // in case we get data from final frame of previous animation

    private bool alreadyAppliedForce;

    private Attack attack, heavyAttack; // setting a private field of type attack

    int attackCounter;
    public PlayerAirAttackingState(PlayerStateMachine stateMachine, int LightAttackIndex, int AttackIndexHeavy) : base(stateMachine)
    {
        if (stateMachine.InputReader.isBasicAttack)
        {
            attack = stateMachine.LightAirAttacks[LightAttackIndex]; // assigning the attack value to the attack id when we pass this information into other states in statemachine
            FaceTarget();
            //Debug.Log("Basic attack index" + attack.ComboStateIndex);
        }
        if (stateMachine.InputReader.isHeavyAttack)
        {
            attack = stateMachine.HeavyAirAttacks[AttackIndexHeavy];
            FaceTarget();
            //Debug.Log("heavy attack index" + AttackIndexHeavy);
        }

        if (stateMachine.InputReader.isBasicAttack && LightAttackIndex > 0)
        {
            AttackIndexHeavy = attack.ComboStateIndex;
        }
        if (stateMachine.InputReader.isHeavyAttack && AttackIndexHeavy > 0)
        {
            LightAttackIndex = attack.ComboStateIndex;
        }
    }
   


    public override void Enter()
    {
        stateMachine.InputReader.JumpEvent += OnJump;
        stateMachine.UpdateHitState();
        MoveToEnemy();
        stateMachine.isStartingComboCounter = false;
        attackCounter = 0;
        FaceTarget();
        stateMachine.SetUpAttacks(attack);
        stateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, attack.TransitionDuration); // crossfade in fixed time is better than play so we get smootheranimations
        PhantomAttackHandler(attack);
       // MoveTowardsTarget();
        stateMachine.Animator.applyRootMotion = true;
        
    }

    public override void Tick(float deltaTime)
    {
        PreventFurtherComboInputs();

        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Attack");
        // redundant normalizedTime >= previousFrameTime &&
        if (normalizedTime < 1f) // if greater than previous do something. if greater than 1 animation has finished, may remove the && for animation cancel
        {
          /*  Vector3 movement = CalculateMovement();
            Move(movement * stateMachine.FreeLookMovementSpeed / 8f, deltaTime);*/
            stateMachine.SetUpAttacks(attack);
            // PushBack();
           // stateMachine.Weapon.SetAttack(attack.Damage, attack.Knockback);
            if (stateMachine.InputReader.isBasicAttack || stateMachine.InputReader.isHeavyAttack)
            {
                attackCounter++;
                if (attackCounter > 1)
                {
                    stateMachine.isStartingComboCounter = true;
                }
                FaceTarget();
               // MoveTowardsTarget();
                TryComboAttack(normalizedTime);
                //Move(stateMachine.InputReader.MovementValue, deltaTime);

            }
        }
        else
        {
            {
                stateMachine.SwitchState(new PlayerFallingState(stateMachine)); // free look state
            }
        }

        if (normalizedTime > attack.ForceTime)
        {

            TryApplyForce(alreadyAppliedForce, attack);
        }
        previousFrameTime = normalizedTime;
    }


    public override void Exit()
    {
        stateMachine.InputReader.JumpEvent -= OnJump;
    }

    private void TryComboAttack(float normalizedTime)
    {
        if (attack.ComboStateIndex == -1) { return; } // making sure we have a combo attack

        if (normalizedTime < attack.ComboAttackTime) { return; } // ensure we are far enough through the combo to do it

        stateMachine.SwitchState // If we are far enough through switch the state to attack.
            (
                new PlayerAirAttackingState
                (
                    stateMachine,
                    attack.ComboStateIndex,
                    attack.HeavyComboStateIndex
                )
            );

    }
    protected override void PhantomAttackHandler(Attack attack)
    {
        base.PhantomAttackHandler(attack);
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

    private Vector3 IdleMovement()
    {
        Vector3 cameraForward = stateMachine.MainCameraTransform.forward;
        Vector3 cameraRight = stateMachine.MainCameraTransform.right;
        cameraForward.y = 0;// don't need to tilt camera
        cameraRight.y = 0;

        // stateMachine.characterController.transform.DOLookAt( stateMachine.transform.position, .1f);   
        return cameraForward + cameraRight;
    }
    private void OnJump()
    {
        stateMachine.SwitchState(new PlayerAirJumpState(stateMachine));
        // making sure we odn't run anything else if this If statement runs
    }
    private void PreventFurtherComboInputs()
    {
        if (attack.ComboStateIndex == -1)
        {
            StopAcceptingPlayerInputOnLastHit();
            return;
        }
    }
}
