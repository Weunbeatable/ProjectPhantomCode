using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// This script will act as a hub for triggering screen wide effects
/// The whole purpse will be methdos that will be called to trigger events, 
/// Seperate scripts in the scene that recieve the events will trigger causing screen effects to play.
/// </summary>
public class TriggerScreenEffects : MonoBehaviour
{
    public static EventHandler onCallShockwave;
    public void TriggerShockWave()
    {
        onCallShockwave?.Invoke(this, EventArgs.Empty);
    }
}
