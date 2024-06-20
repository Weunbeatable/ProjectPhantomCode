using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class PlayerHoldBasicAttack : PlayerBaseState
{
    private float previousFrameTime; // in case we get data from final frame of previous animation

    private bool alreadyAppliedForce;

    private Attack attack; // setting a private field of type attack
    private readonly int HoldTimeSpeedHash = Animator.StringToHash("HoldInput");
    int attackCounter;
    float chargeCounter; // make this part of player base so it won't get reset whenever re entring this state.
    public PlayerHoldBasicAttack(PlayerStateMachine stateMachine, int AttackIndex) : base(stateMachine) // adding attack ID into constructor so we know which attack to use
    {
        if (stateMachine.InputReader.isBasicHoldAttack)
        {
            if (stateMachine.Animator.runtimeAnimatorController != null)
            {
                AttackIndex = AssesIndexState(stateMachine, AttackIndex);
            }
          //  attack = stateMachine.BasicHoldAttack[AttackIndex]; // assigning the attack value to the attack id when we pass this information into other states in statemachine
        }
    
        /*if (stateMachine.InputReader.isBasicHoldAttack && AttackIndex > 0)
        {
            AttackIndexHeavy = attack.ComboStateIndex;

        }*/
    }

    private int AssesIndexState(PlayerStateMachine stateMachine, int AttackIndex)
    {
        if (stateMachine.Animator.runtimeAnimatorController == stateMachine.FightController)
        {
            if (stateMachine.currentCombatIndexValue < 0)
            {
                AttackIndex = 0;
                attack = stateMachine.stanceManager.FighterBasicHoldAttack[AttackIndex];
            }
            else
            attack = stateMachine.stanceManager.FighterBasicHoldAttack[AttackIndex];
        }
        else if (stateMachine.Animator.runtimeAnimatorController == stateMachine.AssasinController)
        {
            if (stateMachine.currentCombatIndexValue >= stateMachine.stanceManager.AssasinBasicHoldAttack.Length) // invalid value due to switching states
            {
                AttackIndex = 0;
                attack = stateMachine.stanceManager.AssasinBasicHoldAttack[AttackIndex];
            }
            else if (stateMachine.currentCombatIndexValue < 0)
            {
                AttackIndex = 0;
                attack = stateMachine.stanceManager.AssasinBasicHoldAttack[AttackIndex];
            }
            else
            {
                Debug.Log("Index for assasin stance is " + AttackIndex);
                attack = stateMachine.stanceManager.AssasinBasicHoldAttack[AttackIndex];
            }
        }
        else
        {
            if (stateMachine.currentCombatIndexValue >= stateMachine.BasicHoldAttack.Length)
            {
                AttackIndex = 0;
                attack = stateMachine.BasicHoldAttack[AttackIndex];
            }
            else if (stateMachine.currentCombatIndexValue < 0)
            {
                AttackIndex = 0;
                attack = stateMachine.Attacks[AttackIndex];
            }
            else
            {
                Debug.Log("Index for general stances is " + AttackIndex);
                stateMachine.currentCombatIndexValue = AttackIndex;
                attack = stateMachine.BasicHoldAttack[AttackIndex]; // assigning the attack value to the attack id when we pass this information into other states in statemachine
            }

        }

      
        return AttackIndex;

    }
    public override void Enter()
    {
        MoveToEnemy();
        stateMachine.UpdateHitState();
        FaceTarget();
        
        stateMachine.isStartingComboCounter = false;
        attackCounter = 0;
        stateMachine.Animator.applyRootMotion = true;
        TriggerLastHitExaggeration();
        //stateMachine.SetUpAttacks(attack);
        int animationNameTest = Animator.StringToHash(attack.AnimationName);
        if (!stateMachine.Animator.HasState(0, animationNameTest))
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine)); // free look state
        }
        else
        {
            stateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, attack.TransitionDuration); // crossfade in fixed time is better than play so we get smootheranimations
            PhantomAttackHandler(attack);
            MoveTowardsTarget();

            //OnTarget();
            // Debug.Log("current animation controller is " + stateMachine.Animator.runtimeAnimatorController.name);

            //  playStoredAttacks();
            stateMachine.currentCombatIndexValue = attack.ComboStateIndex;
        }
        
        // MODIFY THIS LINE so if attack is starting and you are right on the enemy, shift position away a little bit. (probably within plus or minus , so pushback btu with a margin of error)

        // Debug.Log("current animation controller is " + stateMachine.Animator.runtimeAnimatorController.name);

        //  playStoredAttacks();

    }

 

    public override void Tick(float deltaTime)
    {
       
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Attack");
        
       // FaceTarget();
        
        // redundant normalizedTime >= previousFrameTime &&

        if (normalizedTime < .65f) { FaceTarget(); }
        if (normalizedTime < 1f) // if greater than previous do something. if greater than 1 animation has finished, may remove the && for animation cancel
        {
            if (normalizedTime > 0.7f)
            {
                stateMachine.InputReader.FinishEvent += InputBufferForFinishers;
            }
            stateMachine.SetUpAttacks(attack);
                if (attack.ComboStateIndex == -1) { return; }
                else
                {
                    if (stateMachine.InputReader.isBasicHoldAttack)
                    {
                        chargeCounter += deltaTime;
                        
                    }
                    if (stateMachine.InputReader.isBasicHoldAttack)
                    {

                        attackCounter++;
                        if (attackCounter > 1)
                        {
                            stateMachine.isStartingComboCounter = true;
                        }

                        TryComboAttack(normalizedTime);

                        //Move(stateMachine.InputReader.MovementValue, deltaTime);
                    }
                    if (stateMachine.InputReader.isHeavyHoldAttack)
                    {
                        stateMachine.SwitchState(new PlayerHoldHeavyAttack(stateMachine, 0));
                    }

                }
        }
        else
        {
            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine)); // free look state
            }
            //stateMachine.Animator.SetFloat(HoldTimeSpeedHash, 0f);
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
        //   stateMachine.AddedKnockbackValue = 0f;
    }
    private void OnTarget()
    {
        if (attack.ComboStateIndex == -1)
        {
            stateMachine.targeter.RemoveMultiTargets();
        }
    }
    protected override void PhantomAttackHandler(Attack attack)
    {
        base.PhantomAttackHandler(attack);
    }
    private void TryComboAttack(float normalizedTime)
    {
        OnTarget();
        if (attack.ComboStateIndex == -1)
        {
            stateMachine.currentCombatIndexValue = 0;
            return;
        }

        if (normalizedTime < attack.ComboAttackTime) { return; } // ensure we are far enough through the combo to do it
        stateMachine.SwitchState // If we are far enough through switch the state to attack.
                (
                    new PlayerHoldBasicAttack
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
