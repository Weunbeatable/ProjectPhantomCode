using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    //TODO:
    // Ultimately this script needs to be largely refactored to accomadate a transition to persistent data using scriptable objects
    // For now an adjustment to the logic scripts need to be added. 
    // When respective enable object is called. It should trigger a settable boolean to being true, this will make it easier to handle logic for other systems like perfect dodging etc. (for perfect dodge PD one bool is to  check  if attacking the next w ill check if attacking and close to enemy)
    // This boolean will live on the respective state machines and be used to handle state notifications. 
    [SerializeField] private GameObject weaponLogic;
    [SerializeField] private GameObject kunaiLogic;
    [SerializeField] private GameObject greatSwordLogic;
    [SerializeField] private GameObject LeftHandLogic;
    [SerializeField] private GameObject RightHandLogic;
    [SerializeField] private GameObject LeftLegLogic;
    [SerializeField] private GameObject RightLegLogic;
    [SerializeField] private GameObject HeadLogic;
    [SerializeField] public SoundEffects sfx;
    [SerializeField] public VFXEffects vfx;

    [SerializeField] private GameObject greatSwordSpawnParticlePosition;
    public float ReiteratingKnockback { get; set; }

    public string animatorInfo;

    public bool isAttacking { get; set; }

    private void Start()
    {
       // sfx = GetComponent<SoundEffects>();
       // vfx = GetComponent<Effects>();
    }
    
    
    //Weapon enable
    public void EnableWeapon()
    {
        isAttacking = true;
        StanceInfo();
        weaponLogic.SetActive(true);
        sfx.weaponAudio();
        vfx.PlayAssasinHitEffect();
        
    }

    public void DisableWeapon()
    {
        isAttacking = false;
        weaponLogic.SetActive(false);
    }

    public void EnableGreatSword()
    {   
        isAttacking = true;
       // StanceInfo();
        greatSwordLogic.SetActive(true);
        sfx.weaponAudio();
        if(greatSwordSpawnParticlePosition != null)
        {
            vfx.PlayGreatSword(greatSwordSpawnParticlePosition.transform.position);
        }
        
    }

    public void DisableGreatSword()
    {
        isAttacking = false;
        greatSwordLogic.SetActive(false);
    }
    // Enable Hands
    public void EnableRightHand()
    {
        isAttacking = true;
        StanceInfo();
        RightHandLogic.SetActive(true);
        sfx.HeavyPunchAAudio();
        vfx.PlayFighterFistsRight();
    }

    public void DisableRighttHand()
    {
        isAttacking = false;
        RightHandLogic.SetActive(false);
    }


    public void EnableLeftHand()
    {
        isAttacking = true;
        StanceInfo();
        LeftHandLogic.SetActive(true);
        if(sfx != null)
        {
            sfx.LightPunchAAudio();
        }
        if(vfx != null)
        {
            vfx.PlayFighterFistsleft();
        }
        
    }
    public void DisableLeftHand()
    {
        isAttacking = false;
        LeftHandLogic.SetActive(false);
    }

    // Enable legs
    public void EnableRightLeg()
    {
        isAttacking = true;
        StanceInfo(); 
       this.RightLegLogic.SetActive(true);
        if(sfx != null)
        {
            sfx.HeavyKickAudio();
        }
        if(vfx != null)
        {
            vfx.PlayFighterKicksRight();
        }
        
    }
    public void DisableRightLeg()
    {
        isAttacking = false;
        RightLegLogic.SetActive(false);
    }

    public void EnableLeftLeg()
    {
        isAttacking = true;
        StanceInfo();
        LeftLegLogic.SetActive(true);
        if(sfx != null)
        {
            sfx.LightKickAAudio();
        }
        if(vfx != null)
        {
            vfx.PlayFighterKicksLeft();
        }
        
        
    }

    public void DisableLeftLeg()
    {
        isAttacking = false;
        LeftLegLogic.SetActive(false);
    }

    public void EnableLeHead()
    {
        isAttacking = true;
        StanceInfo();
        HeadLogic.SetActive(true);
    }

    public void DisableHead()
    {
        isAttacking = false;
        HeadLogic.SetActive(false);
    }
    public float SetKnockback()
    {
        float knockback;
        knockback = this.ReiteratingKnockback;
        return knockback;
    }

    public bool GetAttackingState() => isAttacking;

    private void StanceInfo()
    {
        if (TryGetComponent<PlayerStateMachine>(out PlayerStateMachine stateMachine))
        {
            animatorInfo = stateMachine.currentActiveStance;
        }
    }
}
