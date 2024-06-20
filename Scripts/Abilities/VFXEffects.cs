using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class VFXEffects : MonoBehaviour
{
    #region ParticleSystem Effects
    [SerializeField] public ParticleSystem fightModeDustParticle;
    [SerializeField] public ParticleSystem gunModeDustParticle;
    [SerializeField] public ParticleSystem greatSwordParticle;
    [SerializeField] public ParticleSystem assasinDustParticle;
    [SerializeField] public GameObject storingTimeEffect;
    [SerializeField] public GameObject playTimeEffect;
    [SerializeField] public ParticleSystem fighterFistRightHitEffect;
    [SerializeField] public ParticleSystem fighterFistLeftHitEffect;
    [SerializeField] public ParticleSystem fighterKickLeftsHitEffect;
    [SerializeField] public ParticleSystem fighterKicksRightEffect;
    [SerializeField] public ParticleSystem gunnerHitEffect;
    [SerializeField] public ParticleSystem greatSwordHitEffect;
    [SerializeField] public ParticleSystem assasinHitEffect;
    [SerializeField] public GameObject ClockSystem;
    [SerializeField] private GameObject parrySuccess;
    #endregion
    #region MeshRenderers
    [SerializeField] private List<SkinnedMeshRenderer> skinMeshMaterials;
    [SerializeField] private List<Material> originalMaterialColors;

    [SerializeField] private Material flashMaterial;
    #endregion
    //public PlayerStateMachine weapon;
    [field: Header("Non Combat effects")]
    [SerializeField] public ParticleSystem windTunnel;




    float pause = 0.5f; // wait half a second before switching back
    bool shouldTriggerFlash;
    private void Awake()
    {
        windTunnel = GetComponentInChildren<ParticleSystem>();
        InitalizeSkinMeshLists();
    }
    private void Start()
    {
        PlayerCombatTimers.ReachedHighVelocity += PlayerStateMachine_ReachedHighVelocity;
       // InitalizeSkinMeshLists();

    }

    

    private void OnEnable()
    {
        WeaponDamage.onParried += WeaponDamage_onParried;
        PlayerBaseState.onFilledPhantomMimicCommandMeter += PlayerBaseState_onFilledPhantomMimicCommandMeter;
        PlayerCombatTimers.OnTriggerChargeFlash += PlayerCombatTimers_OnTriggerChargeFlash;
    }

   

    private void OnDisable()
    {
        WeaponDamage.onParried -= WeaponDamage_onParried;
        PlayerBaseState.onFilledPhantomMimicCommandMeter -= PlayerBaseState_onFilledPhantomMimicCommandMeter;
        PlayerCombatTimers.OnTriggerChargeFlash -= PlayerCombatTimers_OnTriggerChargeFlash;
    }


    private void PlayerCombatTimers_OnTriggerChargeFlash(bool obj)
    {
        shouldTriggerFlash = obj;
        if (shouldTriggerFlash == true)
        {
            StartFlash();
            return;
        }
        
    }
    private void PlayerBaseState_onFilledPhantomMimicCommandMeter()
    {
        if (ClockSystem != null)
        {
            foreach (Transform child in ClockSystem.transform)
            {
                if (child.TryGetComponent<ParticleSystem>(out ParticleSystem system))
                {
                    system.Play();
                }
            }
        }
    }
    private void PlayerStateMachine_ReachedHighVelocity()
    {
        windTunnel.Play();
    }

    private void Update()
    {
        if (shouldTriggerFlash  == true)
        {
            pause =  .5f; // wait half a second before switching back    
                    
        }
        if(pause > 0)
        {
            pause -= Time.deltaTime;
            shouldTriggerFlash = false;
        }
        if (pause <= 0)
        {
            pause = 0;
            endFlash();
        }
        //
    }
    public void PlayMultipleStoringEffect()
    {
        storingTimeEffect.GetComponentInChildren<ParticleSystem>().Play();
    }
    public void PlayPhantomCastingTimeEffect()
    {
        playTimeEffect.GetComponentInChildren<ParticleSystem>().Play();
    }
    public void PlayFighterFistsRight()
    {
        if(fighterFistRightHitEffect != null)
        fighterFistRightHitEffect.GetComponentInChildren<ParticleSystem>().Play();
    }
    public void PlayFighterFistsleft()
    {
        if(fighterFistLeftHitEffect != null)
        {
            fighterFistLeftHitEffect.GetComponentInChildren<ParticleSystem>().Play();
        }
       
    }

    public void PlayFighterKicksRight()
    {
        if(fighterKicksRightEffect != null)
        {
            fighterKicksRightEffect.GetComponentInChildren<ParticleSystem>().Play();
        }
        
    }
    public void PlayFighterKicksLeft()
    {
        if(fighterFistLeftHitEffect == null)
        fighterKickLeftsHitEffect.GetComponentInChildren<ParticleSystem>().Play();
    }

    public void PlayAssasinHitEffect()
    {
        if(assasinHitEffect != null)
        {
            assasinHitEffect.Play();
        }
        
    }
    public void PlayGreatSword(Vector3 spawnPosition)
    {
        if(greatSwordHitEffect != null)
        {
            greatSwordHitEffect.GetComponent<ParticleSystem>().transform.position = spawnPosition;
            greatSwordHitEffect.GetComponent<ParticleSystem>().Play();
        }
       
    }
    private void InitalizeSkinMeshLists()
    {
        if (skinMeshMaterials != null) // in case another object has this script but not these properties. 
        {
            for (int i = 0; i < skinMeshMaterials.Count; i++)
            {
                if (flashMaterial != null)
                {
                    originalMaterialColors.Add(skinMeshMaterials[i].material); // set color of each material at the start to be considered the default color value. 
                }
               
            }
        }
    }
    private void StartFlash()
    {
        if (skinMeshMaterials != null) // in case another object has this script but not these properties. 
        {
            foreach (SkinnedMeshRenderer item in skinMeshMaterials)
            {
                item.material = flashMaterial;
            }

        }
        shouldTriggerFlash = true;            
    }
    private void endFlash()
    {
        if (skinMeshMaterials != null) // in case another object has this script but not these properties. 
        {
            for (int i = 0; i < skinMeshMaterials.Count; i++)
            {
                skinMeshMaterials[i].material = originalMaterialColors[i]; // set color of each material at the start to be considered the default color value. 
            }
        }
    }
    private void OnDestroy()
    {
        PlayerCombatTimers.ReachedHighVelocity -= PlayerStateMachine_ReachedHighVelocity;
    }

    private void WeaponDamage_onParried(object sender, EventArgs e)
    {
        parrySuccess.GetComponent<ParticleSystem>(). Play();
        //Instantiate(parrySuccess, weapon.ClosestImpactPoint, Quaternion.identity);
    }


}
