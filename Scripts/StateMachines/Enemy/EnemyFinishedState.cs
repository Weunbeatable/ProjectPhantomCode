using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFinishedState : EnemyBaseState
{
    public static EventHandler onCleanupFinisherUI;
    private string finisherName;
    int updated_Health;
    public EnemyFinishedState(EnemyStateMachine stateMachine, string FinisherName) : base(stateMachine)
    {
        this.finisherName = FinisherName;
    }

    public override void Enter()
    {
        FacePlayer();
        stateMachine.health.SetHealth(0);
        onCleanupFinisherUI?.Invoke(this, EventArgs.Empty);
        stateMachine.Weapon.gameObject.SetActive(false);
        stateMachine.isTargetOfFinish = false;
        Vector3 startingSpot = stateMachine.transform.forward - stateMachine.target.transform.position;
        Move(startingSpot, Time.deltaTime);
        stateMachine.Animator.applyRootMotion = true;
        GameObject.Destroy(stateMachine.target); // after i'm satisfied with camera testing i'll turn this back on
        stateMachine.Animator.Play(finisherName);



    }

    public override void Tick(float deltaTime)
    {
       
    }
    public override void Exit()
    {
       // GameObject.Destroy(stateMachine.target); // remove target component to stop enemy from being targeted
       // stateMachine.Animator.CrossFadeInFixedTime(EnemyDeathHash, CrossFadeDuration);
        stateMachine.ragdoll.ToggleRagdoll(true);
    }

}
