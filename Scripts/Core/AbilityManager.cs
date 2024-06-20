using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    //The whole purpose of this script is to act as a management dump for abilites
    // specifically attributes that don't handle to well being loaded in through scripting
    // and another way to avoid using gameobject.find to squeeze out more performance
    // will be added to a gameobject that will be persistent across scenes
    // whether the ability is currently on the player or not it should be readily loaded to 
    // avoid unwanted behavior,
    // will be attached to an empty gameobject in the scene and will be used as a reference
    // to load attributes 


    [Header("Animtator Controllers")]
    //TODO: apply getters for animatiors
    [SerializeField] private RuntimeAnimatorController FightModeController;
    [SerializeField] private RuntimeAnimatorController GunModeController;
    [SerializeField] private RuntimeAnimatorController GreatSwordModeController;
    [SerializeField] private RuntimeAnimatorController AssasinModeController;
    [SerializeField] private RuntimeAnimatorController PhantomController;
    [SerializeField] private CombatModifiers playerModifier;

    public static AbilityManager Instance;

    private void Awake()
    {
        //Singleton
        if (Instance != null)
        {
            Debug.Log("a copy of this instance already exists, " + Instance.name + "transform " + transform);
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public RuntimeAnimatorController GetFighterController() => FightModeController;
    public RuntimeAnimatorController GetGunslingerController() => GunModeController;
    public RuntimeAnimatorController GetGreatSwordController() => GreatSwordModeController;
    public RuntimeAnimatorController GetAssassinController() => AssasinModeController;

    public RuntimeAnimatorController GetPhantomController() => PhantomController;

    public CombatModifiers GetCombatModifierOnPhantom() => playerModifier;
}
