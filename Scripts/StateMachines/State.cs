using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State 
{
    public abstract void Enter();

    public abstract void Tick(float deltaTime); // avoid weird movement issues regardless of framerate, going to pass in deltaTime.

    public abstract void Exit();

    protected float GetNormalizedTime(Animator animator, string tag) // purpose here is to check how far through the animation and if we pass a threshold so if they player is still attacking or holding attack , go to the next animation.
    {
        // want to know the data for current and next state to figure out which one we are in whenever blending
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0); // gettinig animation layer info and storing in variables
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

        if (animator.IsInTransition(0) && nextInfo.IsTag(tag)) // already know we are in layer 0, so if we are in layer 0 and transitioning to an attack we want to get the data for the next state.

        {
            return nextInfo.normalizedTime;
        }
        else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag)) // if not in an attack animation case
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f; // just in case none of this is true;s
        }
    }
}
