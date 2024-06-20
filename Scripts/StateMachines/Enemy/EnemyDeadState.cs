using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    public static event EventHandler OnDead;
    private readonly int EnemyDeathHash = Animator.StringToHash("Death");

   
    private const float CrossFadeDuration = 0.1f;
    public EnemyDeadState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        
        stateMachine.Weapon.gameObject.SetActive(false);
        GameObject.Destroy(stateMachine.target); // remove target component to stop enemy from being targeted
        stateMachine.Animator.CrossFadeInFixedTime(EnemyDeathHash, CrossFadeDuration);
        stateMachine.ragdoll.ToggleRagdoll(true);
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public override void Tick(float deltaTime)
    {
        
    }

    public override void Exit()
    {
     
    }
}
