using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockingState : PlayerBaseState
{
    public static event Action<bool> onParrying; // Might need to shift this action or add an action to other attacking states where parrying is possible on attack and some attacks aren't parryable. 
    public static event Action onTriggerSpecialParry;
    private readonly int PlayerBlockHash = Animator.StringToHash("Block");

    private const float CrossFadeDuration = 0.1f;
    private float parryWindow = 0f;
    private float counterTimeCap = 0.2f; // old valuie 0.25
    bool SpecialBlockTriggered;
    public PlayerBlockingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void Enter()
    {
        stateMachine.combatTimers.isInParryState = true;
        WeaponDamage.onParried += WeaponDamage_onParried;
        stateMachine.Animator.CrossFadeInFixedTime(PlayerBlockHash, CrossFadeDuration);
        stateMachine.health.setInVulnerable(true);
        stateMachine.Animator.applyRootMotion = false;
    }



    public override void Tick(float deltaTime)
    {

        parryWindow += deltaTime;
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Block");
        if (parryWindow > counterTimeCap)
        {
            stateMachine.combatTimers.isInParryState = false;
            Move(deltaTime);
        }
        
        if (!stateMachine.InputReader.isBlocking ) // when not blocking go back to targeting state
        {
            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
            return;
        }
        if(stateMachine.targeter.currentTarget == null)  // in case the enemy dies while targeting or gets destoryed we want to be able to go back into free look state
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            return;
        }
    }


    public override void Exit()
    {
        parryWindow = 0f;
        stateMachine.combatTimers.isInParryState = false;
        WeaponDamage.onParried -= WeaponDamage_onParried;
        stateMachine.health.setInVulnerable(false);// currently a simple way of blocking damage, will expand on in the future
    }

    // Parry Trannsition PseudoCode 
    // If player is struck while in blockinng state do a check, If struck before a certain time frame within the animation (lets say 0.3
    // once parrying has been accomplished allow the player a variety of options in slow motion time
    // dodging input
    // parry attack
    // jumping
    // nothing (no inputs)
    // Extra: phantom ability, phantom counter
    private void WeaponDamage_onParried(object sender, EventArgs e)
    {
        stateMachine.combatTimers.isInParryState = false;
            stateMachine.SwitchState(new PlayerParryingState(stateMachine));
        return;
    }
}
