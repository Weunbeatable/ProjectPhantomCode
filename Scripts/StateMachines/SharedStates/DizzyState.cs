using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DizzyState : EnemyBaseState
{
    // melee Stun
    private readonly int noWeaponstunStart = Animator.StringToHash("No_Weapon_Damage_Stun02_start 1");
    private readonly int noWeaponStunLoop = Animator.StringToHash("No_Weapon_Damage_Stun02_loop");
    private readonly int noWeaponStunEnd = Animator.StringToHash("Damage_Stun02_end");
    // weapon Stun
    private readonly int weaponStunStart = Animator.StringToHash("Weapon_Damage_Stun04_start");
    private readonly int weaponStunLoop = Animator.StringToHash("Weapon_Damage_Stun04_loop 1");
    private readonly int weaponStunEnd = Animator.StringToHash("Weapon_Damage_Stun04_end");

    private bool characterControllerInAirStatus;
    private float duration = 1.8f;
    private float transition_Duration = 0.1f;

    private bool isHitByMelee;
    private bool isHitByWeapon;
    public DizzyState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }


    public override void Enter()
    {

        FacePlayer();
        DetermineWeapon();
        characterControllerInAirStatus = stateMachine.characterController.isGrounded;
        WeaponTypeImpactAnimation(noWeaponstunStart, weaponStunStart, 0.1f);
        /* if (characterControllerInAirStatus == true)
         {
             WeaponTypeImpactAnimation(noWeaponstunStart, weaponStunStart, 0.1f);

         }
         else
         {
             stateMachine.SwitchState(new AirHitState(stateMachine));
             stateMachine.Animator.applyRootMotion = false;
         }
         */
    }

    

    public override void Tick(float deltaTime)
    {

        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Dizzy");
        if (normalizedTime > 1f)
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
        }
    }

    private void DetermineWeapon()
    {
        if (stateMachine.weaponType == "Melee")
        {
            isHitByMelee = true;
            isHitByWeapon = false;
        }
        if (stateMachine.weaponType == "Weapon")
        {
            isHitByWeapon = true;
            isHitByMelee = false;
        }
    }
    private void HandleWeaponStun(float deltaTime, float normalizedTime)
    {
        if (normalizedTime > 1f)
        {
            if (isHitByWeapon == true)
            {
                stateMachine.Animator.CrossFadeInFixedTime(weaponStunLoop, transition_Duration);
                duration = 3.2f;
                duration -= deltaTime;
                if (duration <= 0f)
                {
                    stateMachine.Animator.CrossFadeInFixedTime(weaponStunEnd, transition_Duration);
                    if (normalizedTime > 1f)
                    {
                        stateMachine.SwitchState(new EnemyIdleState(stateMachine));
                    }
                }

            }
        }
        if (normalizedTime > 1f)
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
        }
    }

    private void HandleMeleeStun(float deltaTime, float normalizedTime)
    {
        if (isHitByMelee == true)
        {
            if (normalizedTime > 1f)
            {
                stateMachine.Animator.CrossFadeInFixedTime(noWeaponStunLoop, transition_Duration);
                duration -= deltaTime;
                if (duration <= 0f)
                {
                    float lastNormalizedTime = GetNormalizedTime(stateMachine.Animator, "DizzyEnd");
                    stateMachine.Animator.CrossFadeInFixedTime(noWeaponStunEnd, transition_Duration);
                    if (lastNormalizedTime > 1f)
                    {
                        stateMachine.SwitchState(new EnemyIdleState(stateMachine));
                    }
                }

            }
        }
    }

    public override void Exit()
    {
      
    }

}
