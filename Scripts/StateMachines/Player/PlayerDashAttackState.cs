using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashAttackState : PlayerBaseState
{
    private readonly int DashAttackA = Animator.StringToHash("DashA");
    private readonly int DashAttackB = Animator.StringToHash("DashB");
    private Vector3 vector3;
    private const float CrossFadeDuration = 0.1f;
    private float decreaseSpeedValue = 4;
    public PlayerDashAttackState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public PlayerDashAttackState(PlayerStateMachine stateMachine, Vector3 vector3) : this(stateMachine)
    {
        this.vector3 = vector3;
    }
    bool continueMomentum;
    public override void Enter()
    {
        /*  System.Random rnd = new System.Random(); Not sure if I want to keep the idea of it being random instead of it being an input
           int value = rnd.Next(0, 5);*/
        FaceTarget();
        stateMachine.Animator.applyRootMotion = true;
        if (stateMachine.InputReader.isBasicAttack || stateMachine.InputReader.isBasicHoldAttack)
        {
            stateMachine.Animator.CrossFadeInFixedTime(DashAttackA, CrossFadeDuration);
            continueMomentum = false;
        }
        else if(stateMachine.InputReader.isHeavyAttack || stateMachine.InputReader.isHeavyHoldAttack)
        {
            stateMachine.Animator.CrossFadeInFixedTime(DashAttackB, CrossFadeDuration);
            continueMomentum = true;
        }
        
    }



    public override void Tick(float deltaTime)
    {
        
            float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Dash_Attack");
        /*if (normalizedTime < 1f)
        {
            Vector3 movement = vector3;
            Move(movement * stateMachine.FreeLookMovementSpeed/ decreaseSpeedValue, deltaTime);
        }*/
        if (normalizedTime > 1f)
        {
            if(continueMomentum == false)
            {
                stateMachine.forceReceiver.Reset();
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine, this));
            }
            else
            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine, this));
            }
            
        }
    }

    public override void Exit()
    {
   
    }


    private void CheckForCombat()
    {
        if (stateMachine.InputReader.isBasicAttack) // go into attacking state if true
        {
            stateMachine.targeter.SelectTarget();
            stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
            return;
        }
        if (stateMachine.InputReader.isHeavyAttack)
        {
            stateMachine.targeter.SelectTarget();
            stateMachine.SwitchState(new PlayerHeavyAttackingState(stateMachine, 0));
            return;
        }

        if (stateMachine.InputReader.isBasicHoldAttack)
        {
            stateMachine.targeter.SelectTarget();
            stateMachine.SwitchState(new PlayerHoldBasicAttack(stateMachine, 0));
            return;
        }
        if (stateMachine.InputReader.isHeavyAttack)
        {
            stateMachine.targeter.SelectTarget();
            stateMachine.SwitchState(new PlayerHeavyAttackingState(stateMachine, 0));
            return;
        }
    }
}
