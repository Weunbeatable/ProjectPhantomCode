using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PFF.UI;
public class SoundEffects : MonoBehaviour
{
    public AudioClip fighterMode;
    public AudioClip gunmode;
    public AudioClip greatSwordmode;
    public AudioClip assasinmode;
    public AudioSource playerSource;
    public AudioSource rightHandSource;
    public AudioSource leftHandSource;
    public AudioSource rightLegSource;
    public AudioSource leftLegSource;
    public AudioSource weaponSource;
    public AudioClip DashEnded;
    private float pitchRange;
    [Header("punching Audio")]
    public AudioClip lightPunchA;
    public AudioClip lightPunchB;
    public AudioClip heavyPunchA;
    public AudioClip heavyPunchB;

    [Header("kicking Audio")]
    public AudioClip lightKickA;
    public AudioClip heavyKickA;

    [Header("grunt Audio")]
    public AudioClip AttackEffortA;
    public AudioClip PunchEffort;

    [Header("weaponAudio")]
    public AudioClip PhantomWeapon;
    [SerializeField] private AudioClip parryAudio;

    [Header("Special effects sfx")]
    public AudioClip clockActivated;
    [SerializeField] private AudioClip heavyBass;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip specialDodge;

    //TODO: MOVE FOOTSTEP CODE TO FOOTSTEP TEMP AND FOOTSTEP MANAGER WHILE FINISHING ASSEMBLY SETUP
    [Header("FootStepAudio")]
    [SerializeField] private AudioClip jogAudio;
    [SerializeField] private AudioClip jumpAudio;
    [SerializeField] private AudioClip landAudio;
    // Start is called before the first frame update
    void Start()
    {
        playerSource.GetComponent<AudioSource>();
        /*rightHandSource.GetComponent<AudioSource>();*/
    }
    private void OnEnable()
    {
        WeaponDamage.onParried += WeaponDamage_onParried;
        PlayerWorldUI.onPlayDaHorn += PlayerWorldUI_onPlayDaHorn;
        PlayerBaseState.onFilledPhantomMimicCommandMeter += PlayerBaseState_onFilledPhantomMimicCommandMeter;
        PlayerJumpingState.onJumped += PlayerJumpingState_onJumped;
        PlayerLandingState.onlanded += PlayerLandingState_onlanded;
    }


    private void OnDisable()
    {
        WeaponDamage.onParried -= WeaponDamage_onParried;
        PlayerWorldUI.onPlayDaHorn -= PlayerWorldUI_onPlayDaHorn;
        PlayerBaseState.onFilledPhantomMimicCommandMeter += PlayerBaseState_onFilledPhantomMimicCommandMeter;
        PlayerJumpingState.onJumped -= PlayerJumpingState_onJumped;
        PlayerLandingState.onlanded -= PlayerLandingState_onlanded;
    }

    private void PlayerBaseState_onFilledPhantomMimicCommandMeter()
    {
       if(clockActivated != null)
        {
            playerSource.PlayOneShot(clockActivated);
        }
    }
    private void PlayerWorldUI_onPlayDaHorn(object sender, EventArgs e)
    {
        playerSource.PlayOneShot(DashEnded);
    }
    private void PlayerJumpingState_onJumped()
    {
        if(jumpAudio != null)
        {
            playerSource.PlayOneShot(jumpAudio);
        }
        
    }
    private void PlayerLandingState_onlanded()
    {
        if(landAudio != null)
        {
            playerSource.PlayOneShot(landAudio);
        }
        
    } 
    public void weaponAudio()
    {
        pitchRange = UnityEngine.Random.Range(0.98f, 1.02f);
        playerSource.pitch =  pitchRange;
        playerSource.PlayOneShot(PhantomWeapon);
    }
    public void LightPunchAAudio()
    {
        pitchRange = UnityEngine.Random.Range(0.98f, 1.02f);
        playerSource.pitch = pitchRange;
        playerSource.PlayOneShot(lightPunchA);
    }
    public void LightPunchBAudio()
    {
        pitchRange = UnityEngine.Random.Range(0.98f, 1.02f);
        playerSource.pitch = pitchRange;
        playerSource.PlayOneShot(lightPunchB);
    }

    public void HeavyPunchAAudio()
    {
        pitchRange = UnityEngine.Random.Range(0.98f, 1.02f);
        playerSource.pitch = pitchRange;
        playerSource.PlayOneShot(heavyPunchA);
    }
    public void HeavyPunchBAudio()
    {
        pitchRange = UnityEngine.Random.Range(0.98f, 1.02f);
        playerSource.pitch = pitchRange;
        playerSource.PlayOneShot(heavyPunchB);
    }

    public void LightKickAAudio()
    {
        pitchRange = UnityEngine.Random.Range(0.98f, 1.02f);
        playerSource.pitch = pitchRange;
        playerSource.PlayOneShot(lightKickA);
    }
    public void HeavyKickAudio()
    {
        pitchRange = UnityEngine.Random.Range(0.98f, 1.02f);
        playerSource.pitch = pitchRange;
        playerSource.PlayOneShot(heavyKickA);
    }

    public void Grunt1()
    {
        pitchRange = UnityEngine.Random.Range(0.98f, 1.02f);
        playerSource.pitch = pitchRange;
        playerSource.PlayOneShot(AttackEffortA);
    }
    public void PunchingEffort()
    {
        pitchRange = UnityEngine.Random.Range(0.98f, 1.02f);
        playerSource.pitch = pitchRange;
        playerSource.PlayOneShot(PunchEffort);
    }

    private void WeaponDamage_onParried(object sender, EventArgs e)
    {
        pitchRange = UnityEngine.Random.Range(0.98f, 1.02f);
        playerSource.pitch = pitchRange;
        playerSource.PlayOneShot(parryAudio);
    }

    public void HeavyBass()
    {
        if(heavyBass != null)
        {
            pitchRange = UnityEngine.Random.Range(0.98f, 1.02f);
            playerSource.pitch = pitchRange;
            playerSource.PlayOneShot(heavyBass);
        }
    }

    public void JumpSound()
    {
        if (jumpSound != null)
        {
            playerSource.PlayOneShot(jumpSound);
        }
    }

    public void SpecialDodge()
    {
        if(specialDodge != null)
        {
            playerSource.PlayOneShot(specialDodge);
        }
    }
    //
}
