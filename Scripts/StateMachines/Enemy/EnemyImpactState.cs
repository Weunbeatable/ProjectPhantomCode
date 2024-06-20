using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EnemyImpactState : EnemyBaseState
{
    
    private readonly int EnemyImpactHash = Animator.StringToHash("Sword And Shield Impact");
    private readonly int EnemyImpactAssasinHash = Animator.StringToHash("Damage_Assasin_Stun");
    private readonly int EnemyImpactMeleeHash = Animator.StringToHash("MeleeStun");
    private readonly int EnemyImpactHeavySwordHash = Animator.StringToHash("Damage_HeavySword");
    //TODO Create script to pass a string value of hit with its type and string value to differentiate assassin, great sword etc so i'm not manually passing all these values. 

    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;

    private float duration = 1.6f;

    Attack attack;
    
    public EnemyImpactState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    
    public override void Enter()
    {
        FacePlayer();
        StanceDependentImpactAnimation(EnemyImpactMeleeHash, EnemyImpactHeavySwordHash, EnemyImpactAssasinHash, EnemyImpactHash, CrossFadeDuration);
        //Vector3 offset = new Vector3(-1.5f, 0f, -1.5f);
        if (stateMachine.target != null)
        {
            stateMachine.transform.position = Vector3.Lerp(stateMachine.transform.position, stateMachine.target.transform.forward, Time.deltaTime * .5f);         
        }

    }

   

    public override void Tick(float deltaTime)
    {


        Move(deltaTime);
        // attack = GameObject.FindGameObjectWithTag("Player").GetComponent<Attack>();
        // duration = attack.hitStunDuration;
        duration -= deltaTime;

        if (duration <= 0f)
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
        }

    }

    public override void Exit()
    {
       
    }
   /* private void StanceDependentImpactAnimation() //TO DO: Refactor and move to enemy Base state so this takes 4 string values instead so I can just call the function and hot swap values instead of manually adding and changing each time. 
    {
        if (stateMachine.playerResponse.isRespondingToFighter == true)
        {
            stateMachine.Animator.CrossFadeInFixedTime(EnemyImpactMeleeHash, CrossFadeDuration);
        }
        else if (stateMachine.playerResponse.isRespondingToGreatSword == true)
        {
            stateMachine.Animator.CrossFadeInFixedTime(EnemyImpactHeavySwordHash, CrossFadeDuration);
        }
        else if (stateMachine.playerResponse.isRespondingToAssassin == true)
        {
            stateMachine.Animator.CrossFadeInFixedTime(EnemyImpactAssasinHash, CrossFadeDuration);
        }
        else
        {
            stateMachine.Animator.CrossFadeInFixedTime(EnemyImpactHash, CrossFadeDuration);
        }
    }
    private void HandleTakeDamage()
    {
        stateMachine.SwitchState(new EnemyImpactState(stateMachine));
    }*/
}
