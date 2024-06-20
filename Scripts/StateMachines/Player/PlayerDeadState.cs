using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    private readonly int PlayerDeathHash = Animator.StringToHash("PlayerDeath");

    private const float CrossFadeDuration = 0.1f;
    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        
        stateMachine.Weapon.gameObject.SetActive(false);
        stateMachine.Animator.CrossFadeInFixedTime(PlayerDeathHash, CrossFadeDuration);
        stateMachine.ragdoll.ToggleRagdoll(true);
    }


    public override void Tick(float deltaTime)
    {
        
    }

    public override void Exit()
    {
       
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
