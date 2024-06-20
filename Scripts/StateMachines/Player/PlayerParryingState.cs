using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryingState : PlayerBaseState
{
    public static event Action onSpecialParryTriggered;

    private readonly int ParrySuccess = Animator.StringToHash("Succesful_Parry");

    private const float CrossFadeDuration = 0.1f;

    private bool specialParry;
    public PlayerParryingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public PlayerParryingState(PlayerStateMachine stateMachine, bool specialParry) : base(stateMachine)
    {
        this.specialParry = specialParry;
    }
    public override void Enter()
    {
        if(specialParry == true)
        {
            onSpecialParryTriggered?.Invoke();
        }
        stateMachine.Animator.CrossFadeInFixedTime(ParrySuccess, CrossFadeDuration);
        stateMachine.InputReader.TargetEvent += Ontarget;
        stateMachine.InputReader.JumpEvent += OnJump;
        stateMachine.InputReader.TauntEvent += OnTaunt;
        stateMachine.InputReader.DodgeEvent += OnDash;
        stateMachine.Animator.applyRootMotion = true;
    }

    public override void Tick(float deltaTime)
    {
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Parried");
        if (normalizedTime > 1f)
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            return;
        }
        if (normalizedTime > 0.5f)
        {
            CheckForCombat();
        }

            
    }
    public override void Exit()
    {
        if(specialParry == true) { specialParry = false; }
        stateMachine.InputReader.TargetEvent -= Ontarget; // Entering free look blend tree
        stateMachine.InputReader.JumpEvent -= OnJump;
        stateMachine.InputReader.TauntEvent -= OnTaunt;
        stateMachine.InputReader.DodgeEvent -= OnDash;
    }



    private void OnTaunt()
    {
        
        stateMachine.SwitchState(new PlayerTauntState(stateMachine));
        // making sure we odn't run anything else if this If statement runs
    }
    private void OnJump()
    {
        
        stateMachine.SwitchState(new PlayerJumpingState(stateMachine));
        // making sure we odn't run anything else if this If statement runs
    }
    private void Ontarget()
    {
        
        if (!stateMachine.targeter.SelectTarget()) { return; }
        stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
    }
    private void OnDash()
    {
        
        stateMachine.SwitchState(new PlayerDashingState(stateMachine, stateMachine.InputReader.MovementValue)); // flexibility if we wantto dodge with something other than movement value
    }

    private void CheckForCombat()
    {
        if (stateMachine.InputReader.isBasicAttack || stateMachine.InputReader.isHeavyAttack)
        {
            stateMachine.SwitchState(new PlayerCounterAttackState(stateMachine));
        }
        return;
    }
}
