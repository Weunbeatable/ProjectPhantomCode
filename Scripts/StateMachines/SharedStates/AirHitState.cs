using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirHitState : EnemyBaseState
{
    private readonly int EnemyStunHash = Animator.StringToHash("FighterHitPopupState");
    private readonly int EnemyImpactAssasinHash = Animator.StringToHash("FighterHitPopupState");
    private readonly int EnemyImpactMeleeHash = Animator.StringToHash("AirHitMelee");
    private readonly int EnemyImpactHeavySwordHash = Animator.StringToHash("GreatSword_Hit_PopupState");

    private readonly int EnemyImpactWeaponHash = Animator.StringToHash("AirHitWeapon");

    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;

    private float duration = .7f;
    private Vector3 momentum;
    private Vector3 pos;
    public AirHitState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.health.onTakeDamage += HandleTakeDamage;
        momentum = stateMachine.characterController.velocity;
        momentum = Vector3.zero;

        FacePlayer();
        WeaponTypeImpactAnimation(EnemyImpactMeleeHash, EnemyImpactWeaponHash, CrossFadeDuration);
        //StanceDependentImpactAnimation(EnemyImpactMeleeHash, EnemyImpactHeavySwordHash, EnemyImpactAssasinHash, EnemyStunHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
       
        
        //Move(deltaTime/2);
        // attack = GameObject.FindGameObjectWithTag("Player").GetComponent<Attack>();
        // duration = attack.hitStunDuration;
        duration -= deltaTime;
        Move(momentum, deltaTime / 8);
        /* if (duration <= 0f)
         {
             Move(deltaTime / 2);

         }*/
        var normalizedTime = GetNormalizedTime(stateMachine.Animator, "AirHit");
        if (normalizedTime < 1f)
        {

            return;

        }
        else
        {
            if (stateMachine.characterController.isGrounded)
            {
                stateMachine.SwitchState(new GetupState(stateMachine));
                return;
            }
            else
            {
                stateMachine.SwitchState(new PopUpFallingState(stateMachine));
                return;
            }
        }
        
            
        
    }
    public override void Exit()
    {
        stateMachine.health.onTakeDamage -= HandleTakeDamage;
    }

    private void HandleTakeDamage()
    {
        /* if(!stateMachine.characterController.isGrounded)
         {
             stateMachine.SwitchState(new AirHitState(stateMachine));
         }
         else
         {
             stateMachine.LoadStates();
         }*/
        CheckforReset();
    }

    private void CheckforReset()
    {
        if (!Physics.Raycast(stateMachine.characterController.transform.position, Vector3.down, out var hit)) return;

        if (hit.distance >= 1.05f)
        {
            stateMachine.SwitchState(new AirHitState(stateMachine));
        }
        else
        {
            stateMachine.LoadStates();
        }
    }
}
