using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : EnemyBaseState
{
    private readonly int EnemyStunHash = Animator.StringToHash("Stagger");
    private readonly int EnemyImpactAssasinHash = Animator.StringToHash("Stagger");
    private readonly int EnemyImpactMeleeHash = Animator.StringToHash("MeleeStun");
    private readonly int EnemyImpactHeavySwordHash = Animator.StringToHash("WeaponStagger");

    private readonly int EnemyImpactWeaponHash = Animator.StringToHash("Damage_HeavySword");
    //TODO Create script to pass a string value of hit with its type and string value to differentiate assassin, great sword etc so i'm not manually passing all these values. 

    // Old Animations 
    //private readonly int EnemyImpactMeleeHash = Animator.StringToHash("Stagger");
    //private readonly int EnemyImpactWeaponHash = Animator.StringToHash("WeaponStagger");
    //***********************
    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;

    private float duration = 1.2f;

    public StunState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.navMesh.enabled = false;
        stateMachine.health.onTakeDamage += HandleTakeDamage;
        FacePlayer();
        WeaponTypeImpactAnimation(EnemyImpactMeleeHash, EnemyImpactWeaponHash, CrossFadeDuration);
        stateMachine.Animator.applyRootMotion = true;
        //StanceDependentImpactAnimation(EnemyImpactMeleeHash, EnemyImpactHeavySwordHash, EnemyImpactAssasinHash, EnemyStunHash, CrossFadeDuration);
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
       // stateMachine.Animator.applyRootMotion = false;
        stateMachine.health.onTakeDamage -= HandleTakeDamage;
    }

    private void HandleTakeDamage()
    {
        //stateMachine.Animator.applyRootMotion = false;
        stateMachine.LoadStates();
    }
}
