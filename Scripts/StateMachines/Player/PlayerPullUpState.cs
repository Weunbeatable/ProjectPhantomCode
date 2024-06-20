using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPullUpState : PlayerBaseState
{
    private readonly int PlayerHangHash = Animator.StringToHash("LedgeClimb");

    private readonly Vector3 offset = new Vector3 (0f, 2.325f, 0.65f);

    private const float CrossFadeDuration = 0.1f;
    public PlayerPullUpState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(PlayerHangHash, CrossFadeDuration);
        stateMachine.ledgeDetector.onLedgeDetect += HandleLedgeDetection;
    }

    public override void Tick(float deltaTime)
    {
    if(GetNormalizedTime(stateMachine.Animator, "Climbing") < 1f) { return; }

        stateMachine.characterController.enabled = false; // setting it to false will allow for the translation, afterwards we set it back on. 
        stateMachine.transform.Translate(offset, Space.Self); // these offset values that have  been made a variable so they aren't magic numbers require trial and error in editor to get it right, also it is relative to self.
        stateMachine.characterController.enabled = true ;

        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine, false));
    }
    public override void Exit()
    {
        stateMachine.characterController.Move(Vector3.zero);
        stateMachine.forceReceiver.Reset(); // reset our forces so we don't plummet to the ground over time
        stateMachine.ledgeDetector.onLedgeDetect -= HandleLedgeDetection;
    }

    private void HandleLedgeDetection(Vector3 ledgeForward, Vector3 closestPoint)
    {
        stateMachine.SwitchState(new PlayerHangingState(stateMachine, ledgeForward, closestPoint));
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
