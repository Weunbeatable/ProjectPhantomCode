using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime;


public class PlayerTauntState : PlayerBaseState
{
    //Custom Taunt state, 
    // Currently whenever context is pressed a taunt animation will occur
    // Debuffing properties attached to taunt command
    // invokes a rage state in enemies, lowering their defense but increasing their attack
    // taunt may increase speed but have some damaage offsset, or could link attack and defense increase decrease properties. 

    private readonly int PlayerTauntHash = Animator.StringToHash("PhoneTaunt");
    private readonly int PlayerBreakDanceTauntHash = Animator.StringToHash("1980BreakDanceTaunt");
    private readonly int PlayerFreezeTauntHash = Animator.StringToHash("FreezeTaunt");
   


    private const float CrossFadeDuration = 0.1f;
    private float duration = 17f;
    Taunt taunt;
    PropHandler currentProp;

    public PlayerTauntState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        var pickATaunt = UnityEngine.Random.Range(0, 30);
        
        if(pickATaunt <= 10)
        stateMachine.Animator.CrossFadeInFixedTime(PlayerTauntHash, CrossFadeDuration); // crossfade in fixed time is better than play so we get smootheranimations
        else if(pickATaunt <= 20 )
        stateMachine.Animator.CrossFadeInFixedTime(PlayerBreakDanceTauntHash, CrossFadeDuration);
        else 
        stateMachine.Animator.CrossFadeInFixedTime(PlayerFreezeTauntHash, CrossFadeDuration);

        pickATaunt = UnityEngine.Random.Range(0, 30);
    }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);
        duration -= deltaTime;

        if (duration <= 0f || stateMachine.InputReader.MovementValue.magnitude != 0)
        {
            
            ReturnToLocomotion();
        }
        Vector3 fwd = stateMachine.characterController.transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(stateMachine.characterController.transform.position, fwd, 10))
            Debug.Log("There is something in front of the object!");
    }

  

    public override void Exit()
    {
       
    }


    private void TryTaunt(float normalizedTime)
    {
      /*  stateMachine.SwitchState // If we are far enough through switch the state to attack.
                (
                    new PlayerTargetingState
                    (stateMachine)
                );*/
    }


    private float GetNormalizedTime() // purpose here is to check how far through the animation and if we pass a threshold so if they player is still attacking or holding attack , go to the next animation.
    {
        // want to know the data for current and next state to figure out which one we are in whenever blending
        AnimatorStateInfo currentInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(0); // gettinig animation layer info and storing in variables
        AnimatorStateInfo nextInfo = stateMachine.Animator.GetNextAnimatorStateInfo(0);

        if (stateMachine.Animator.IsInTransition(0) && nextInfo.IsTag("Taunt")) // already know we are in layer 0, so if we are in layer 0 and transitioning to an attack we want to get the data for the next state.

        {
            return nextInfo.normalizedTime;
        }
        else if (!stateMachine.Animator.IsInTransition(0) && currentInfo.IsTag("Taunt")) // if not in an attack animation case
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f; // just in case none of this is true;s
        }
    }
}
