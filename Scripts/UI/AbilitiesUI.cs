using PFF.Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class AbilitiesMenuUI : MonoBehaviour
{
    // abilites menu should take an object of type base ability system.
    // It should access its string name,
    // access its image
    // also check to make sure its of the correct type
    // further expantion can also check here to see if it is a hold or multi tap ability.
    // Also have to account for phantom abilites vs regular player abilites. 

    [SerializeField] GameObject abilityEntryPrefab;
    List<BaseAbilitySystem> abilities;
    public enum triggerChoice { leftTrigger = 0,  rightTrigger = 1 };
    public triggerChoice selectedTrigger;

    [SerializeField] BaseAbilitySystem abilityLeft;
    [SerializeField] BaseAbilitySystem abilityRight;
    // due to the small scope of this project i'll just leave the abilites all in one place (no shop system at the moment)
   [SerializeField] PhantomStealAbility stealAbility;
   [SerializeField] PhantomCombatMimicAbility mimicAbility;
    public PlayerStateMachine machine;

    private void Awake()
    {
        machine = GameObject.FindWithTag("Player").GetComponent<PlayerStateMachine>();
    }
    void Start()
    {
        abilities = new List<BaseAbilitySystem>();
    }
    /*
     * void FirePerformed(InputAction.CallbackContext context)
{
    // If SlowTap interaction was performed, perform a charged
    // firing. Otherwise, fire normally.
    if (context.interaction is SlowTapInteraction)
        FireChargedProjectile();
    else
        FireNormalProjectile();
}
    */
    // Update is called once per frame
    void Update()
    {
        if (machine.InputReader.isLeftSpecial == true)
        {
            
         //   abilityLeft.UseAbility();
        }
        if(machine.InputReader.isLeftSpecialHold == true)
        {
            
           // abilityLeft.UseAbility();
        }

        if (machine.InputReader.isRightSpecial == true)
        {
           
            //abilityRight.UseAbility();
        }
        if (machine.InputReader.isRightSpecialHold == true)
        {
            

                //  abilityRight.UseAbility();
            
        }
        abilityLeft.SetTapStatus(machine.InputReader.isLeftSpecial);
        abilityLeft.SetHoldStatus(machine.InputReader.isLeftSpecial);
        abilityRight.SetTapStatus(machine.InputReader.isRightSpecial);
        abilityRight.SetHoldStatus(machine.InputReader.isRightSpecial);
    }

    public void AddAbilityEntry(string abilityName, Sprite abilityIcon, BaseAbilitySystem.IAbilitesDelegate useAbility)
    {
        GameObject entry = Instantiate(abilityEntryPrefab, transform);

        BaseAbilitySystem abs = entry.GetComponent<BaseAbilitySystem>();
        abs.GetAbilityName();
        
    } 

    public void OpenMenu()
    {
        //menuIsOpen = true;
        print("Opened abilites menu");
        if(abilities.Count == 0)
        {
            for(int i = 0; i < 1; i++)
            {
                AddAbilityEntry(stealAbility.GetAbilityName(), stealAbility.GetAbilityImageIcon(), stealAbilityDelegate); // will add another boolean or option so when a skill is selected it flips a switch on the script
                // this "switch" will let the script know its currently the one selected so that other features can work properly like the gaurd system
                AddAbilityEntry(mimicAbility.GetAbilityName(), mimicAbility.GetAbilityImageIcon(), CombatMimicAbilityDelegate);

            }
        }
    }

    void stealAbilityDelegate(IAbilites ability)
    {
        stealAbility.UseAbility();
    }

    void CombatMimicAbilityDelegate(IAbilites ability)
    {
        mimicAbility.UseAbility();
    }
}
