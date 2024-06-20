using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose of this is to drive most UI that is based on distance
/// if player or a target is within some sort of priximity it will trigger some UI to play
/// This function can then be modified in whatever script extends itself with this Interface for better functionality and to fit its specific need. 
/// </summary>
public interface IProximity 
{
    public void InProximity();
}
