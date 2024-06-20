using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;

public class PlayerSpecialBlockingState : PlayerBaseState
{
    public static event Action<bool> onParrying; // Might need to shift this action or add an action to other attacking states where parrying is possible on attack and some attacks aren't parryable. 
    public static event Action onTriggerSpecialParry;
    private readonly int PlayerBlockHash = Animator.StringToHash("SpecialBlock");

    private const float CrossFadeDuration = 0.1f;
    private float parryWindow = 0f;
    private float counterTimeCap = 0.25f;
    bool SpecialBlockTriggered;
    public PlayerSpecialBlockingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public PlayerSpecialBlockingState(PlayerStateMachine stateMachine, bool SpecialBlockTriggered) : base(stateMachine)
    {
        this.SpecialBlockTriggered = SpecialBlockTriggered;
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
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "SpecialBlock");

        if (normalizedTime < 1f)
        {
            if (parryWindow > counterTimeCap)
            {
                stateMachine.combatTimers.isInParryState = false;
                Move(deltaTime);
            }
        }
        else
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
        if (this.SpecialBlockTriggered == true)
        {
            this.SpecialBlockTriggered = false;
        }
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
            stateMachine.SwitchState(new PlayerParryingState(stateMachine, true));
            onTriggerSpecialParry?.Invoke();
    }
}
