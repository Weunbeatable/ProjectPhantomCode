using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyBackState :EnemyBaseState
{
    private readonly int EnemyStunHash = Animator.StringToHash("FighterHitPopupState");
    private readonly int EnemyImpactAssasinHash = Animator.StringToHash("FighterHitPopupState");
    private readonly int EnemyImpactMeleeHash = Animator.StringToHash("Flyback_Melee");
    private readonly int EnemyImpactWeaponHash = Animator.StringToHash("Flyback_Weapon");

    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;

    private float duration = 1.6f;
    public FlyBackState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.health.onTakeDamage += HandleTakeDamage;
        FacePlayer();
        WeaponTypeImpactAnimation(EnemyImpactMeleeHash, EnemyImpactWeaponHash, CrossFadeDuration);
      
    }

    public override void Tick(float deltaTime)
    {
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Flyback");
        Move(deltaTime);
        // attack = GameObject.FindGameObjectWithTag("Player").GetComponent<Attack>();
        // duration = attack.hitStunDuration;
        duration -= deltaTime;

      /*  if (duration <= 0f)
        {*/
      if(normalizedTime < 1f)
        {
            if(stateMachine.characterController.isGrounded )
            {
                stateMachine.SwitchState(new GetupState(stateMachine));
                return;
            }
            else
            {
                return;
            }
        }
        if (stateMachine.characterController.isGrounded && duration <= 0f)
        {
            stateMachine.SwitchState(new GetupState(stateMachine));
            return;
        }
        else
        {
            stateMachine.SwitchState(new PopUpFallingState(stateMachine));
        }
        
       
        
    }
    public override void Exit()
    {
        stateMachine.health.onTakeDamage -= HandleTakeDamage;
    }

    private void HandleTakeDamage()
    {
        if (!stateMachine.characterController.isGrounded)
        {
            stateMachine.SwitchState(new PopUpStartState(stateMachine));
        }
        else
        {
            stateMachine.LoadStates();
        }
    }

}
