using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Attack 
{
    public List<ComboInput> comboInputs;
    [field: SerializeField] public string AnimationName { get; private set; }
    [field: SerializeField] public float TransitionDuration { get; private set; } 
    [field: SerializeField] public int ComboStateIndex { get;  set; } = -1; // setting the index to -1 is a little more freeform, it allows us to have combo mixups
    [field: SerializeField] public int HeavyComboStateIndex { get; set; } = -1; // setting the index to -1 is a little more freeform, it allows us to have combo mixups
    [field: SerializeField] public float ComboAttackTime { get; private set; } // How far through an attack before we can do the next attack, (we are getting closer to animation cancelling)
    [field: SerializeField] public float ForceTime { get; private set; }
    [field: SerializeField] public float Force { get; private set; }
    [field: SerializeField] public float hitStunDuration { get; private set; }
    [field: SerializeField] public float Knockback { get;  set; } // turned off private set for knockback and damage. may change later
    [field: SerializeField] public int Damage { get; set; }

    public Attack(string AnimationName, float TransitionDuration, float ComboAttackTime)
    {
        this.AnimationName = AnimationName;
        this.TransitionDuration = TransitionDuration;
        this.ComboAttackTime = ComboAttackTime;
    }

}
