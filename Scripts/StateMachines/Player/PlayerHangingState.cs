using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHangingState : PlayerBaseState
{
    
    private Vector3 ledgeForward;
    private Vector3 closestPoint;

    private readonly int PlayerHangHash = Animator.StringToHash("HangIdle");

    private const float CrossFadeDuration = 0.1f;


    public PlayerHangingState(PlayerStateMachine stateMachine, Vector3 ledgeForward, Vector3 closestPoint) : base(stateMachine) // switching to this states requires the vector 3 variahles to be passed into the constructor
    {
      
        this.ledgeForward = ledgeForward;
        this.closestPoint = closestPoint;
    }

    public override void Enter()
    {
        stateMachine.transform.rotation = Quaternion.LookRotation(ledgeForward, Vector3.up); // in order to face leldge;
        stateMachine.characterController.enabled = false;
        stateMachine.transform.position = closestPoint - (stateMachine.ledgeDetector.transform.position - stateMachine.transform.position); // position of hands - positon of player
        stateMachine.characterController.enabled = true;



        stateMachine.Animator.CrossFadeInFixedTime(PlayerHangHash, CrossFadeDuration);
        stateMachine.InputReader.JumpEvent += OnJump;
    }

    public override void Tick(float deltaTime)
    {
       if(stateMachine.InputReader.MovementValue.y < 0f)
        {
            stateMachine.characterController.Move(Vector3.zero);
            stateMachine.forceReceiver.Reset(); // reset our forces so we don't plummet to the ground over time
            stateMachine.SwitchState(new PlayerFallingState(stateMachine)); 
        }
        if (stateMachine.InputReader.MovementValue.y > 0f)
        {
           
            stateMachine.SwitchState(new PlayerPullUpState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.InputReader.JumpEvent -= OnJump;
    }
    private void OnJump()
    {
        stateMachine.SwitchState(new PlayerWallEjectState(stateMachine));
        return;
        // making sure we odn't run anything else if this If statement runs
    }
}
