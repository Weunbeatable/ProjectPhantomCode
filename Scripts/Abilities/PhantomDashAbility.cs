using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomDashAbility : phantomAbilites
{
    /*
     * Need to keep count of how many dashes were done. Can't simply be a dash input because the input is being
     * used for other things as well. 
     * Need to keep track of when dash input has been activated. and record the last known location on exiting the state
     * This can be handled via events from the dash class, (alternatively we can have an interface) for contextual interactions
     * Each dash will get added to a Stack of dash inputs.
     * Tapping the button will return you back to the last spot, (more of a lerp, via coroutine) 
     * holding. Causes an "overcharge of dashes, holding and releasing will cause a charge for stacks,
     * The more stacks the more powerful your dash will become. 
     */
    public static bool isRecordingPossible { set; get; }
    int chargeStacks = 0;
    public override void Awake()
    {
        base.Awake();
        isRecordingPossible = true;
        // subscribe to statemachine event and pull state data there.
        
    }

 

    private void OnDestroy()
    {
        // if the skill is not active on the phantom anymore trigger an event to stop allowing recording. 
        isRecordingPossible = false;
        
    }
    public override void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        GetPlayerInputs().RightSpecialEvent += PhantomDashAbility_RightSpecialEvent;
        GetPlayerInputs().RightSpecialHoldEvent += PhantomDashAbility_RightSpecialHoldEvent;
     //   StateMachine.OnInformCurrentState += StateMachine_OnInformCurrentState;
        PlayerDashingState.OnDashHasEnded += PlayerDashingState_OnDashHasEnded;
    }

    

    private void OnDisable()
    {
        GetPlayerInputs().RightSpecialEvent -= PhantomDashAbility_RightSpecialEvent;
        GetPlayerInputs().RightSpecialHoldEvent -= PhantomDashAbility_RightSpecialHoldEvent;
        //   StateMachine.OnInformCurrentState -= StateMachine_OnInformCurrentState;
        PlayerDashingState.OnDashHasEnded += PlayerDashingState_OnDashHasEnded;
    }

    private void PhantomDashAbility_RightSpecialEvent()
    {
        // Check if stacks are not 0 
        if(chargeStacks > 1)
        {
            Debug.Log("gimmie dem stacks");
        }
    }

    private void PhantomDashAbility_RightSpecialHoldEvent()
    {
        throw new System.NotImplementedException();
    }

 /*   private void StateMachine_OnInformCurrentState(State state)
    {
        if (state is PlayerDashingState && chargeStacks <= 3)
        {
            Debug.Log("Yo did you just dash");
            chargeStacks++;
            return;
        }
    }*/

    private void PlayerDashingState_OnDashHasEnded()
    {
       if(chargeStacks <= 3)
        {
            chargeStacks++;
            Debug.Log("Yo did you just dash");
        }
    }

    public override string GetAbilityName()
    {
        return "Phantom Dash";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
