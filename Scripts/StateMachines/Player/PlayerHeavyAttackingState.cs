using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
public class PlayerHeavyAttackingState : PlayerBaseState
{

    private bool alreadyAppliedForce;

    private Attack attack;

    int attackCounter;
    public PlayerHeavyAttackingState(PlayerStateMachine stateMachine, int AttackIndex) : base(stateMachine) // adding attack ID into constructor so we know which attack to use
    {
       
        if (stateMachine.InputReader.isHeavyAttack)
        {
            if (stateMachine.Animator.runtimeAnimatorController != null)
            {
                AttackIndex = AssesIndexState(stateMachine, AttackIndex);
            }
        }
        if (stateMachine.InputReader.isBasicAttack)     { return; }
        if (stateMachine.InputReader.isBasicHoldAttack) { return; }
        if (stateMachine.InputReader.isHeavyHoldAttack) { return; }
    }
    private int AssesIndexState(PlayerStateMachine stateMachine, int AttackIndex)
    {
        if (stateMachine.Animator.runtimeAnimatorController == stateMachine.FightController)
        {
            if (stateMachine.currentCombatIndexValue < 0)
            {
                AttackIndex = 0;
                attack = stateMachine.stanceManager.FighterHeavyAttacks[AttackIndex];
            }
            else if(stateMachine.currentCombatIndexValue > stateMachine.stanceManager.FighterHeavyAttacks.Count())
            {
                attack = stateMachine.stanceManager.FighterHeavyAttacks[AttackIndex -2];
            }
            else
                attack = stateMachine.stanceManager.FighterHeavyAttacks[AttackIndex];
        }
        else if (stateMachine.Animator.runtimeAnimatorController == stateMachine.AssasinController)
        {
            if (stateMachine.currentCombatIndexValue >= stateMachine.stanceManager.AssasinHeavyAttacks.Length) // invalid value due to switching states
            {
                AttackIndex = 0;
                attack = stateMachine.stanceManager.AssasinHeavyAttacks[AttackIndex];
            }
            else if (stateMachine.currentCombatIndexValue < 0)
            {
                AttackIndex = 0;
                attack = stateMachine.HeavyAttacks[AttackIndex];
            }
            else
            {
                attack = stateMachine.stanceManager.AssasinHeavyAttacks[AttackIndex];
            }
        }
        else
        {
            if (stateMachine.currentCombatIndexValue >= stateMachine.HeavyAttacks.Length)
            {
                AttackIndex = 0;
                attack = stateMachine.HeavyAttacks[AttackIndex];
            }
            else if (stateMachine.currentCombatIndexValue < 0)
            {
                AttackIndex = 0;
                attack = stateMachine.HeavyAttacks[AttackIndex];
            }
            else
            {
                stateMachine.currentCombatIndexValue = AttackIndex;
                attack = stateMachine.HeavyAttacks[AttackIndex]; // assigning the attack value to the attack id when we pass this information into other states in statemachine
            }

        }

        Debug.Log("Attackname is " + attack.AnimationName);
        return AttackIndex;

    }

    public override void Enter()
    {
        
        stateMachine.UpdateHitState();
        FaceTarget();
        stateMachine.isStartingComboCounter = false;
        attackCounter = 0;
        stateMachine.Animator.applyRootMotion = true;
        TriggerLastHitExaggeration();
        stateMachine.SetUpAttacks(attack);
        TryApplyForce(alreadyAppliedForce, attack);
      //  MoveToEnemy();

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
            stateMachine.currentCombatIndexValue = attack.HeavyComboStateIndex;
        }
      

    }

    public override void Tick(float deltaTime)
    {
        HandleInputBuffer();
        stateMachine.SetUpAttacks(attack);
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Attack");
        
        if (normalizedTime < .7f) { FaceTarget(); }
        // if (normalizedTime < .3f) { MoveToEnemy(); }
         if(normalizedTime > 0.7f)
            {
                stateMachine.InputReader.FinishEvent += InputBufferForFinishers;
            }
        if (normalizedTime < 1f) // if greater than previous do something. if greater than 1 animation has finished, may remove the && for animation cancel
        {
           

                if (stateMachine.InputReader.isHeavyAttack)
                {
                    attackCounter++;
                    if (attackCounter > 1)
                    {
                        stateMachine.isStartingComboCounter = true;
                    }


                    TryComboAttack(normalizedTime);
                    //Move(stateMachine.InputReader.MovementValue, deltaTime);

                }
                if (stateMachine.InputReader.isBasicAttack)
                {
                    /*attackCounter++;
                    if (attackCounter > 1)
                    {
                        stateMachine.isStartingComboCounter = true;
                    }
                    FaceTarget();*/
                    TryLightComboAttack(normalizedTime);
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

            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine)); // free look state
            }
        }

        if (normalizedTime > attack.ForceTime)
        {

            TryApplyForce(alreadyAppliedForce, attack);
           // stateMachine.combatModifiers.shouldAdjustPositionFromEnemy = 0;
        }
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

        if (attack.HeavyComboStateIndex == -1)
        {
            stateMachine.currentCombatIndexValue = 0;
            return;
        }
        // making sure we have a combo attack
        if (normalizedTime < attack.ComboAttackTime) { return; } // ensure we are far enough through the combo to do it
        OnTarget();
        stateMachine.SwitchState // If we are far enough through switch the state to attack.
            (
                new PlayerHeavyAttackingState
                (
                    stateMachine,
                    attack.HeavyComboStateIndex
                )
            ); 

    }
    private void TryLightComboAttack(float normalizedTime)
    { // Current limitation is the inputs are evaluated with booleans. so when I cross reference the input buffer I have to evaluate what the input is,
        // Then evaluate if the input value is true, then and only then can i switch state. 
        // When Implementing this in unreal opt for just checking the raw value of the command (does it correlate with light attack, heavy etc). 
        if (attack.ComboStateIndex == -1) { return; } // making sure we have a combo attack
        if (normalizedTime < attack.ComboAttackTime) { return; } // ensure we are far enough through the combo to do it

        OnTarget();
        stateMachine.SwitchState // If we are far enough through switch the state to attack.
            (
                new PlayerAttackingState
                (
                    stateMachine,
                    attack.HeavyComboStateIndex
                )
            ); ;

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
    void PhantomAttackHandler()
    {
        /* if (stateMachine.fighter.playPhantomAttacks.Count <= 10)
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
 */
      /*  if (PhantomCombatMimicAbility.Instance.istapped == true)
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
        }*/
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

    private void PreventFurtherComboInputs()
    {
        if (attack.ComboStateIndex == -1)
        {
            StopAcceptingPlayerInputOnLastHit();
            return;
        }
    }
}
