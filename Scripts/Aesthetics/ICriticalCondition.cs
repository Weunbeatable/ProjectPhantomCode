using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// purpose of this is to trigger actions depending on the state of the player. 
/// depending on the health status or state of the player this condition will trigger 
/// it will be called in multiple classes affecting movement animations audio, and possibly even speed, effects and range and speed of abilites. 
/// </summary>
public interface ICriticalCondition // may rework into an interface instead
{
   virtual void CriticalCondition() { }
}
