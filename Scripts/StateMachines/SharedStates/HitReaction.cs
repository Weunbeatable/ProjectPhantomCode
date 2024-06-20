using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Script documentation. 
 * Purpose: Retrieve data passed from Attackers animation state and allow invocation of a corresponding impact animation/ effect.
 * Parameters: Animator, animation data, trigger.
 * Preconditions:Must be attached to receiver, Attacker must have an animator.
 * Returns: Data pertaining to attack/impact animation and by extension any trigger that would cause some form of impact. 
 * Side Effect: Management of state transition.
 */
public class HitReaction : MonoBehaviour
{
    public event Action onHitReaction;

    public void CorrespondingState(string recievedState)
    {
        onHitReaction?.Invoke();
    }

    public void StateDictionary()
    {

    }
}
