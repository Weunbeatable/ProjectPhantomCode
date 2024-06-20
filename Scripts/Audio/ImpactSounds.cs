using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSounds : MonoBehaviour
{
  
    public AudioSource genericAudioSource;

    [Header("Impact Audio")]
    public AudioClip meleeImpact;
    public AudioClip weaponImpact;

    //TODO split this up so that it's sepearate for weapon and bodypart. tag changes needed. 
    void Start()
    {
        genericAudioSource.GetComponent<AudioSource>();
    }

    public void PlayMeleeAAudio()
    {
        genericAudioSource.PlayOneShot(meleeImpact);
    }
    public void PlayWeaponAudio()
    {
        genericAudioSource.PlayOneShot(weaponImpact);
    }
}
