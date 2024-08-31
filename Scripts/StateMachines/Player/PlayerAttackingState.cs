using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAttackingState : PlayerBaseState
{
    private float previousFrameTime; // in case we get data from final frame of previous animation

    private bool alreadyAppliedForce;

    private Attack attack,heavyAttack; // setting a private field of type attack

    int attackCounter;
    
    public PlayerAttackingState(PlayerStateMachine stateMachine, int AttackIndex) : base(stateMachine) // adding attack ID into constructor so we know which attack to use
    {
       //TODO: 
       // ADD a check for array length, if the previous stance was a stance that had more attacks we can only know by comparing array sizes, if array size of previous is larger and we are further along then we need to default to
       // being the last attack of the array of the current stance e.g. fighter has 8 attacks and greatsword has 4 if i switch on the 6th attack of figther to greatsword I need to default to the 4th attack of greatsword so that I don't 
       // get an out of bounds error on my array index. 

        // Keep track of index value in a set ; get variable in state machine so I can do a comparison check when switching to an attacking state
        // then when I do an if check passing in that value in a new attacking state I don't get an empty object. 
        if (stateMachine.InputReader.isBasicAttack)
        {
            if(stateMachine.Animator.runtimeAnimatorController != null)
            {
                AttackIndex = AssesIndexState(stateMachine, AttackIndex);
                // FaceTarget();
                //Debug.Log("Basic attack index" + attack.ComboStateIndex);
                //Debug.Log("Attackname is " + attack.AnimationName);
            }
        }
        if (stateMachine.InputReader.isHeavyAttack)
        {
            return;
           /* attack = stateMachine.HeavyAttacks[AttackIndex];
            FaceTarget();*/
        }

        if (stateMachine.InputReader.isBasicHoldAttack) { return; }
        if (stateMachine.InputReader.isHeavyHoldAttack) { return; }
       

    }

    //TODO: MOVE THIS FUNCTION INTO PLAYERBASE STATE SO I'M NOT NEEDLESSLY EDITING THE SAME FUNCTION IN MULTIPLE PLACES JUST
    // ADD SOME PARAMETERS
    private int AssesIndexState(PlayerStateMachine stateMachine, int AttackIndex)
    {
        if(AttackIndex < 0) { AttackIndex++; }
        if (stateMachine.Animator.runtimeAnimatorController == stateMachine.FightController)
        {
            if (stateMachine.currentCombatIndexValue < 0)
            {
                AttackIndex = 0;
                attack = stateMachine.stanceManager.FighterAttacks[AttackIndex];
            }
            else
                attack = stateMachine.stanceManager.FighterAttacks[AttackIndex];
        }
        else if (stateMachine.Animator.runtimeAnimatorController == stateMachine.AssasinController)
        {
            if (stateMachine.currentCombatIndexValue >= stateMachine.stanceManager.AssassinAttacks.Length) // invalid value due to switching states
            {
                AttackIndex = 0;
                attack = stateMachine.stanceManager.AssassinAttacks[AttackIndex];
            }
            else if (stateMachine.currentCombatIndexValue < 0)
            {
                AttackIndex = 0;
                attack = stateMachine.stanceManager.AssassinAttacks[AttackIndex];
            }
            else
            {
                stateMachine.currentCombatIndexValue = AttackIndex;
                attack = stateMachine.stanceManager.AssassinAttacks[AttackIndex];
            }
        }
        else
        {
            if (stateMachine.currentCombatIndexValue >= stateMachine.Attacks.Length )
            {
                AttackIndex = 0;
                attack = stateMachine.Attacks[AttackIndex];
            }
            else if (stateMachine.currentCombatIndexValue < 0)
            {
                AttackIndex = 0;
                attack = stateMachine.Attacks[AttackIndex];
            }
            else
            {
                stateMachine.currentCombatIndexValue = AttackIndex;
                attack = stateMachine.Attacks[AttackIndex]; // assigning the attack value to the attack id when we pass this information into other states in statemachine
            }

        }

      //  Debug.Log("Attackname is " + attack.AnimationName);
        return AttackIndex;
         
    }

    public override void Enter()
    {
       // MoveToEnemy();
        stateMachine.UpdateHitState();
        stateMachine.isStartingComboCounter = false;
        attackCounter = 0;
        FaceTarget();
       
        stateMachine.Animator.applyRootMotion = true;
        /*   stateMachine.Weapon.SetAttack(attack.Damage, attack.Knockback);
           stateMachine.headDamage.SetAttack(attack.Damage, attack.Knockback);
           stateMachine.rightHandDamage.SetAttack(attack.Damage, attack.Knockback);
           stateMachine.leftHandDamage.SetAttack(attack.Damage, attack.Knockback);
           stateMachine.rightLegDamage.SetAttack(attack.Damage, attack.Knockback);
           stateMachine.leftLegDamage.SetAttack(attack.Damage, attack.Knockback);*/
        TriggerLastHitExaggeration(); // event to subscribe to
        stateMachine.SetUpAttacks(attack);
        TryApplyForce(alreadyAppliedForce, attack);
       // MoveTowardsTarget();
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

       
     //   Debug.Log("Current Combo index is " + stateMachine.currentCombatIndexValue);
    }

    public override void Tick(float deltaTime)
    {
    /*    if(GetInput() == 0 || GetInput() == 1)
        {
            HandleInputBuffer();
            HandleInputBuffer();
            ResetBufferActive();
        }*/
       
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Attack");
        if (normalizedTime > 0.75f) // Really interesting results. Effectively 3/4 of the inputs are read before allowing for hold attacks. heavy attacks allow full 4 attacks. very cool...
        {
            if (stateMachine.InputReader.isBasicHoldAttack)
            {
                stateMachine.SwitchState(new PlayerHoldBasicAttack(stateMachine, 0));
            }
            if (stateMachine.InputReader.isHeavyHoldAttack)
            {
                stateMachine.SwitchState(new PlayerHoldHeavyAttack(stateMachine, 0));
            }
        }
        if (normalizedTime < .6f) { FaceTarget(); }
        if (normalizedTime > 0.7f)
        {
            stateMachine.InputReader.FinishEvent += InputBufferForFinishers;
        }
        if ( normalizedTime < 1f) // if greater than previous do something. if greater than 1 animation has finished, may remove the && for animation cancel
        {
           
            stateMachine.SetUpAttacks(attack);

            if (stateMachine.InputReader.isBasicAttack)
            {
                attackCounter++;
                if(attackCounter > 1)
                {
                    stateMachine.isStartingComboCounter = true;
                }
                
                
                TryComboAttack(normalizedTime);
                //Move(stateMachine.InputReader.MovementValue, deltaTime);

            }
            if (stateMachine.InputReader.isHeavyAttack)
            {
             /*   attackCounter++;
                if (attackCounter > 1)
                {
                    stateMachine.isStartingComboCounter = true;
                }*/
                TryHeavyComboAttack(normalizedTime);
                //Move(stateMachine.InputReader.MovementValue, deltaTime);

            }
            if (stateMachine.InputReader.isBasicHoldAttack)
            {
                stateMachine.SwitchState(new PlayerHoldBasicAttack(stateMachine, 0));
            }
            if (stateMachine.InputReader.isHeavyHoldAttack)
            {
                stateMachine.SwitchState(new PlayerHoldHeavyAttack(stateMachine, 0));
            }
        }
        else
        {
          /*  if(stateMachine.targeter.currentTarget != null) // will not just automatically switch to targeting when inputting attack. Leaving here in attacking state
           *  in case i change my mind
            {
                attackCounter = 0;
                stateMachine.isStartingComboCounter = false;
                stateMachine.SwitchState (new PlayerTargetingState(stateMachine)); //switch to targeting state
            }*//*
            else*/
            {
                stateMachine.SwitchState (new PlayerFreeLookState(stateMachine)); // free look state
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
        
        if (attack.ComboStateIndex == -1)
        {
            stateMachine.currentCombatIndexValue = 0;
            return;
        }
        // making sure we have a combo attack
        if (normalizedTime < attack.ComboAttackTime) { return; } // ensure we are far enough through the combo to do it
        OnTarget();

        stateMachine.SwitchState // If we are far enough through switch the state to attack.
            (
                new PlayerAttackingState
                (
                    stateMachine,
                    attack.ComboStateIndex
                )
            ); ;
        
    }
    private void TryHeavyComboAttack(float normalizedTime)
    {
        // if (attack.ComboStateIndex == -1) { return; } // making sure we have a combo attack
        if (attack.ComboStateIndex == -1)
        {
            stateMachine.currentCombatIndexValue = 0;
            return;
        }
        if (normalizedTime < attack.ComboAttackTime) { return; } // ensure we are far enough through the combo to do it

        OnTarget();
        stateMachine.SwitchState // If we are far enough through switch the state to attack.
            (
                new PlayerHeavyAttackingState
                (
                    stateMachine,
                    attack.ComboStateIndex
                )
            ); 

    }

    void MoveTowardsTarget()
    {
        if (stateMachine.targeter.currentTarget == null && stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.characterController.transform.rotation = Quaternion.LookRotation(stateMachine.characterController.transform.forward);
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



    #region old code
    // old code implementation 

    /* if(stateMachine.InputReader.isBasicAttack && AttackIndex > 0 ) 
     {
           AttackIndexHeavy = attack.ComboStateIndex;

     }*/
    /*if(stateMachine.InputReader.isHeavyAttack && AttackIndexHeavy > 0)
    {
          AttackIndex = attack.ComboStateIndex;
    }*/



    /*    void PhantomAttackHandler() Reference to the old format, this was on all combat scripts, has been moved to bae class
    {
        if (PhantomCombatMimicAbility.Instance.istapped == true)
        {
            if (stateMachine.fighter.playPhantomAttacks.Count <= 10)
            {
                //stateMachine.TestDataCollection.Add(attack.AnimationName);
                stateMachine.fighter.SwithchPhantomController.Add(stateMachine.Animator.runtimeAnimatorController.name);
                stateMachine.fighter.playPhantomAttacks.Add(attack.AnimationName);
                stateMachine.effects.PlayMultipleStoringEffect();
            }
            if (stateMachine.fighter.playPhantomAttacks.Count == 10)
            {
                stateMachine.effects.PlayPhantomCastingTimeEffect();
            }
            if (stateMachine.fighter.playPhantomAttacks.Count > 10)
            {
                stateMachine.fighter.SwithchPhantomController.Clear();
                stateMachine.fighter.playPhantomAttacks.Clear();
                // stateMachine.TestDataCollection.Clear();
            }
            // Debug.Log("Size of list is now " + stateMachine.TestDataCollection.Count);
            // Debug.Log("Switchables new name is" + stateMachine.fighter.switchableAnim);
        }
    }*/

    // redundant normalizedTime >= previousFrameTime &&
    //FaceTarget();
    //       if(normalizedTime < .3f) { MoveToEnemy(); }
    private void PreventFurtherComboInputs()
    {
        if (attack.ComboStateIndex == -1)
        {
            StopAcceptingPlayerInputOnLastHit();
            return;
        }
    }
    #endregion

}



