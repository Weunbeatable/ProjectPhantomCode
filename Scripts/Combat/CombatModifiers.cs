using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatModifiers : MonoBehaviour
{
    [field: SerializeField] public float modifiedKnockBack { get; set; }
    [field: SerializeField] public string updatedHitStatus { get; set; }

    [field: SerializeField] public List<WeaponDamage> characterWeapons = new List<WeaponDamage>();

    [SerializeField] public RuntimeAnimatorController stolenAnimationController;
    [SerializeField] public int combatClipHash;

    [field: SerializeField] public AnimationClip CombatClip { get; set; }

    // public int shouldAdjustPositionFromEnemy { get; set; } // values should only ever be 0 or 1, 0 = false 1 = true

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    public float SetKnockback(float Knockback)
    {
        Knockback = this.modifiedKnockBack;
        return Knockback;
    }

    public string SetAttackStatus(string hitState)
    {
        hitState = this.updatedHitStatus;
        return updatedHitStatus;
    }


    //Setters here

    public void SetStunState()
    {
        foreach(WeaponDamage status in characterWeapons)
        {
            status.triggeredHitStateData = "stun";
        }
    }
    public void SetStaggerState()
    {
        foreach (WeaponDamage status in characterWeapons)
        {
            status.triggeredHitStateData = "stagger";
        }
    }
    public void SetFlybackState()
    {
        foreach (WeaponDamage status in characterWeapons)
        {
            status.triggeredHitStateData = "flyback";
        }
    }
    public void SetLauncherState()
    {
        foreach (WeaponDamage status in characterWeapons)
        {
            status.triggeredHitStateData = "launcher";
        }
    }
    public void SetDizzyState()
    {
        foreach (WeaponDamage status in characterWeapons)
        {
            status.triggeredHitStateData = "dizzy";
        }
    }
    public void SetKnockdownState()
    {
        foreach (WeaponDamage status in characterWeapons)
        {
            status.triggeredHitStateData = "knockdown";
        }
    }

}
