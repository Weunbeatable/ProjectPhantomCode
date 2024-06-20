using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgingState : PlayerBaseState
{
    private readonly int DodgingBlendTreeHash = Animator.StringToHash("EvasionBlendTree");

    private readonly int DodgeForwardHash = Animator.StringToHash("DodgeForward");

    private readonly int DodgeRightHash = Animator.StringToHash("DodgeRight");

    private const float CrossFadeDuration = 0.1f;

    private Vector3 dodgingDirectionInput;

    private float remainingDodgeTime;


    public PlayerDodgingState(PlayerStateMachine stateMachine, Vector3 dodgingDirectionInput) : base(stateMachine)
    {
        this.dodgingDirectionInput = dodgingDirectionInput;
    }

    public override void Enter()
    {
        
        //  stateMachine.InputReader.DodgeEvent += OnDodge;
        remainingDodgeTime = stateMachine.DodgeDuration;

            stateMachine.Animator.SetFloat(DodgeForwardHash, dodgingDirectionInput.y);
            stateMachine.Animator.SetFloat(DodgeRightHash, dodgingDirectionInput.x);
            stateMachine.Animator.CrossFadeInFixedTime(DodgingBlendTreeHash, CrossFadeDuration);
            stateMachine.health.setInVulnerable(true);
            stateMachine.Animator.applyRootMotion = true;
    }



    public override void Tick(float deltaTime)
    {
 

            Vector3 movement = new Vector3();

            movement += stateMachine.transform.right * dodgingDirectionInput.x * stateMachine.DodgeLength / stateMachine.DodgeDuration;
            movement += stateMachine.transform.forward * dodgingDirectionInput.y * stateMachine.DodgeLength / stateMachine.DodgeDuration;
            // movement vector is transform.right * input gets direction, length divided by duration gets us the distance
            // same thing for forwad
            Move(movement, deltaTime); // dodge depending on movement

            FaceTarget(); //ensure we still face target 

            remainingDodgeTime -= deltaTime;

            if (remainingDodgeTime <= 0f)
            {
                stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
            }

    }

    public override void Exit()
    {
        stateMachine.health.setInVulnerable(false);
        /* foreach (GameObject dodge in stateMachine.dodgeObjects)
         {
             dodge.SetActive(false);

         }*/
    }

}
