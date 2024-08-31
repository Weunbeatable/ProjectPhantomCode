using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallEjectState : PlayerBaseState
{
    private readonly int PlayerEjectHash = Animator.StringToHash("WallEject");

    private readonly Vector3 Eject = new Vector3(0f, 0f, -90f);

    private const float CrossFadeDuration = 0.1f;
    private Vector3 lookValue;
    public PlayerWallEjectState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.applyRootMotion = true;
        stateMachine.Animator.CrossFadeInFixedTime(PlayerEjectHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        lookValue = stateMachine.characterController.transform.forward;
        if (GetNormalizedTime(stateMachine.Animator, "Climbing") < 1f) { return; }

        /*    stateMachine.characterController.enabled = false; // setting it to false will allow for the translation, afterwards we set it back on. 
            stateMachine.transform.Translate(offset, Space.Self); // these offset values that have  been made a variable so they aren't magic numbers require trial and error in editor to get it right, also it is relative to self.
            stateMachine.characterController.enabled = true;*/

        /* if (stateMachine.characterController.velocity.y <= 0)
         {*/

        if (GetNormalizedTime(stateMachine.Animator, "Climbing") > 1f)
            stateMachine.transform.rotation = Quaternion.LookRotation(lookValue * -1f, Vector3.up);
            stateMachine.SwitchState(new PlayerFallingState(stateMachine));

            return;
        
    }

    public override void Exit()
    {
     
    }

    public void CalltoEject()
    {
        stateMachine.forceReceiver.WallJumpForce(Eject, ForceMode.Impulse);
    }
}
