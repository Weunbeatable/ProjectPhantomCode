using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpFallingState : EnemyBaseState
{

    private readonly int falling_HitHash = Animator.StringToHash("Hit_Falling");

    private Vector3 momentum;
    //TODO Create script to pass a string value of hit with its type and string value to differentiate assassin, great sword etc so i'm not manually passing all these values. 

    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;

    private float duration = 1.3f;
    public PopUpFallingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
       stateMachine.health.onTakeDamage += HandleTakeDamage;
        momentum = stateMachine.characterController.velocity;
        momentum.y = 0f;
        momentum.x = 0f;
        momentum.z = 0f;
        stateMachine.Animator.CrossFadeInFixedTime(falling_HitHash, CrossFadeDuration);

    }
   
        

    public override void Tick(float deltaTime)
    {
        //Debug.Log("current state is " + stateMachine.characterController.isGrounded);
         Move(momentum, deltaTime/1.454f);

        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "hitFalling");
        if (normalizedTime < 1f) // if greater than previous do something. if greater than 1 animation has finished, may remove the && for animation cancel
        {
            //stateMachine.characterController.excludeLayers = 6;
            if (stateMachine.characterController.isGrounded == true)
            {
                duration -= deltaTime;
                    stateMachine.SwitchState(new GetupState(stateMachine));             
            }
        }
        else
        {

            if (stateMachine.characterController.isGrounded == true)
            {
                duration -= deltaTime;
                if (duration <= 0f)
                {
                    // There should be a grounded knocked state that allows for transition between being knocked
                    // and getting up, that way extra logic can be allowed (like being able to get hit while getting up)
                    // or being invulnerable and proper animation transition
                    stateMachine.SwitchState(new GetupState(stateMachine));
                    return;
                }
            }
            else
            {
                stateMachine.SwitchState(new PopUpFallingState(stateMachine));
            }
        }


        /*       if(!stateMachine.characterController.isGrounded)
               {
                   stateMachine.SwitchState(new PopUpFallingState(stateMachine));
                   return;
               }*/

    }
    public override void Exit()
    {
      stateMachine.health.onTakeDamage -= HandleTakeDamage;
    }

    private void HandleTakeDamage()
    {
        if(stateMachine.characterController.isGrounded == false)
        {
            if(stateMachine.hitReaction == "knockdown")
            {
                stateMachine.SwitchState(new KnockDownState(stateMachine));
            }
            else
                stateMachine.SwitchState(new AirHitState(stateMachine));
        }
        else
        {
            stateMachine.SwitchState(new StunState(stateMachine));
        }
            
            
    }
}
