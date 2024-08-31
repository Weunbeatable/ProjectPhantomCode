using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpStartState : EnemyBaseState
{
    private readonly int EnemyStunHash = Animator.StringToHash("FighterHitPopupState");
    private readonly int EnemyImpactAssasinHash = Animator.StringToHash("FighterHitPopupState");
    private readonly int EnemyImpactMeleeHash = Animator.StringToHash("EnemyHitPopupState");
    private readonly int EnemyImpactHeavySwordHash = Animator.StringToHash("GreatSword_Hit_PopupState");

    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;

    private float duration = .4f;

    private Vector3 momentum;
    public PopUpStartState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.navMesh.enabled = false;
        stateMachine.health.onTakeDamage += HandleTakeDamage;
        momentum = stateMachine.characterController.velocity;
        //momentum.y = 0f;
        //facePlayer();
        WeaponTypeImpactAnimation(EnemyImpactMeleeHash, EnemyImpactHeavySwordHash, CrossFadeDuration);
        //StanceDependentImpactAnimation(EnemyImpactMeleeHash, EnemyImpactHeavySwordHash, EnemyImpactAssasinHash, EnemyStunHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "PopUP");
        //Debug.Log("current velocity is " + stateMachine.characterController.velocity);
        Move(momentum, deltaTime * 0.5f);
        // attack = GameObject.FindGameObjectWithTag("Player").GetComponent<Attack>();
        // duration = attack.hitStunDuration;
        duration -= deltaTime;
        // if(stateMachine.characterController.detectCollisions)


        if (normalizedTime < 1f)
        {
            if (stateMachine.characterController.isGrounded)
            {
                stateMachine.SwitchState(new GetupState(stateMachine));

                return;
            }

/*           else if (!stateMachine.characterController.isGrounded)
            {
                stateMachine.SwitchState(new PopUpFallingState(stateMachine));
            }*/
        }
        else
        {
            if (stateMachine.characterController.isGrounded)
            {
                stateMachine.SwitchState(new GetupState(stateMachine));


            }

            else
            {
                stateMachine.SwitchState(new PopUpFallingState(stateMachine));
            }


        }

        /* if (duration <= 0f)
         {


                 if (stateMachine.characterController.velocity.y <= 0)
                 {
                     stateMachine.SwitchState(new PopUpFallingState(stateMachine));

                     return;
                 }

         }*/
    }
    public override void Exit()
    {
        stateMachine.health.onTakeDamage -= HandleTakeDamage;
    }

    private void HandleTakeDamage()
    {

        if (stateMachine.characterController.isGrounded == false)
        {
            if (stateMachine.hitReaction == "knockdown")
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
