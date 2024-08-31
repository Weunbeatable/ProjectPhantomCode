using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaggerState :EnemyBaseState
{
    private readonly int EnemyStunHash = Animator.StringToHash("Fighter_Hit_Stun");
    private readonly int EnemyImpactAssasinHash = Animator.StringToHash("Damage_Assasin_Stun");
    private readonly int EnemyImpactMeleeHash = Animator.StringToHash("Stagger");
    private readonly int EnemyImpactHeavySwordHash = Animator.StringToHash("Damage_HeavySword");

    private readonly int EnemyImpactWeaponHash = Animator.StringToHash("WeaponStagger");

    //old animation
    //private readonly int EnemyImpactWeaponHash = Animator.StringToHash("Damage_HeavySword");
    //private readonly int EnemyImpactMeleeHash = Animator.StringToHash("Melee_Impact");

    private const float AnimatorDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;

    private float duration = 1.6f;
    public StaggerState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.navMesh.enabled = false;
        stateMachine.health.onTakeDamage += HandleTakeDamage;
        FacePlayer();
        WeaponTypeImpactAnimation(EnemyImpactMeleeHash, EnemyImpactWeaponHash, CrossFadeDuration);
        stateMachine.Animator.applyRootMotion = true;
      //  stateMachine.Animator.CrossFadeInFixedTime(EnemyStunHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
      //  Move(deltaTime);
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
      //  stateMachine.Animator.applyRootMotion = false;
    }
    private void HandleTakeDamage()
    {
        //stateMachine.Animator.applyRootMotion = false;
        stateMachine.LoadStates();
    }
}
