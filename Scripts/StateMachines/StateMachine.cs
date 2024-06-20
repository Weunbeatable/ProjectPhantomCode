using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State currentState;
    protected State previousState;
    public void SwitchState(State newState)
    {
        currentState?.Exit(); // checking if null just to be safe
        currentState = newState;
        currentState.Enter();
        
    }
    void Update()
    {   
            currentState?.Tick(Time.deltaTime); // one liner If statement null conditional operater (such wow much great)           
    }

    // caching previous state as state
    public void CheckSwitchState(State prevState)
    {
        currentState?.Exit(); // checking if null just to be safe
        previousState = currentState;

    }

    // caching previous state as string
    public void CheckSwitchState(string prevStateName)
    {
        currentState?.Exit(); // checking if null just to be safe
        prevStateName = previousState.ToString();
    }
    public State GetCurrentState()
    {
        return currentState;
    }
}
