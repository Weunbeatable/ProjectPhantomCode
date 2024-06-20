/*    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerBaseGroundedAttackingState : PlayerBaseState
{
  
        private float previousFrameTime; // in case we get data from final frame of previous animation

        private bool alreadyAppliedForce;

        private Attack attack; // setting a private field of type attack
        private readonly int HoldTimeSpeedHash = Animator.StringToHash("HoldInput");
        int attackCounter;
        float chargeCounter; // make this part of player base so it won't get reset whenever re entring this state.
        public  PlayerBaseGroundedAttackingState(PlayerStateMachine stateMachine, int AttackIndex) : base(stateMachine) // adding attack ID into constructor so we know which attack to use
        {
            if (stateMachine.InputReader.isHeavyHoldAttack)
            {
                attack = stateMachine.HeavyHoldAttack[AttackIndex]; // assigning the attack value to the attack id when we pass this information into other states in statemachine
                FaceTarget();
                *//* Debug.Log("Basic attack index" + attack.ComboStateIndex);
                 Debug.Log("Attackname is " + attack.AnimationName);*//*
            }

            *//*if (stateMachine.InputReader.isBasicHoldAttack && AttackIndex > 0)
            {
                AttackIndexHeavy = attack.ComboStateIndex;

            }*//*
        }
        public override void Enter()
        {
            MoveToEnemy();
            FaceTarget();
            stateMachine.UpdateHitState();
           *//* if (attack.ComboStateIndex == -1)
            {
                stateMachine.combatTimers.isChargingHeavy = true;
            }*//*
            stateMachine.isStartingComboCounter = false;
            attackCounter = 0;
            TriggerLastHitExaggeration();
            stateMachine.SetUpAttacks(attack);

            stateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, attack.TransitionDuration); // crossfade in fixed time is better than play so we get smootheranimations


            PhantomAttackHandler(attack);
            stateMachine.Animator.applyRootMotion = true;
            //  playStoredAttacks();
        }

        public override void Tick(float deltaTime)
        {

            stateMachine.SetUpAttacks(attack);

            float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Attack");


           // FaceTarget();
            if (normalizedTime < 1f)
            {
            if (normalizedTime > 0.7f)
            {
                stateMachine.InputReader.FinishEvent += InputBufferForFinishers;
            }
            if (attack.ComboStateIndex == -1) { return; }
                else
                {
                    if (stateMachine.InputReader.isHeavyHoldAttack)
                    {

                        attackCounter++;
                        if (attackCounter > 1)
                        {
                            stateMachine.isStartingComboCounter = true;
                        }
                        //  FaceTarget();

                        TryComboAttack(normalizedTime);
                    }
                    if (stateMachine.InputReader.isBasicHoldAttack)
                    {
                        stateMachine.SwitchState(new PlayerHoldBasicAttack(stateMachine, 0));
                    }
                }
            }
            else
            {
                if (stateMachine.targeter.currentTarget != null)
                {
                    attackCounter = 0;
                    stateMachine.isStartingComboCounter = false;
                    stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
                    //   stateMachine.SwitchState(new PlayerTargetingState(stateMachine)); //switch to targeting state
                }
                else
                {
                    stateMachine.SwitchState(new PlayerFreeLookState(stateMachine)); // free look state
                }
                //  stateMachine.Animator.SetFloat(HoldTimeSpeedHash, 0f);
            }

            if (normalizedTime > attack.ForceTime)
            {

            TryApplyForce(alreadyAppliedForce, attack);
        }
            previousFrameTime = normalizedTime;
        }


        public override void Exit()
        {
        stateMachine.InputReader.FinishEvent -= InputBufferForFinishers;
        stateMachine.combatModifiers.modifiedKnockBack = 0f;
        }

    private void OnTarget()
    {
        if (attack.ComboStateIndex == -1)
        {
            stateMachine.targeter.RemoveMultiTargets();
        }
    }

    private void TryComboAttack(float normalizedTime)
    {
        OnTarget();
        if (attack.ComboStateIndex == -1) { return; } // making sure we have a combo attack

        if (normalizedTime < attack.ComboAttackTime) { return; } // ensure we are far enough through the combo to do it
        
        stateMachine.SwitchState // If we are far enough through switch the state to attack.
                (
                    new PlayerHoldHeavyAttack
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

    private void TriggerLastHitExaggeration()
    {
        if (attack.ComboStateIndex == -1)
        {
            stateMachine.isLastHit = true;
        }
        else
        {
            stateMachine.isLastHit = false;
        }
    }
}


*/