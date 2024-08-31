using PFF.Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

//using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

namespace PFF.UI
{
    public class AbilitiesMenuUI : MonoBehaviour
    {
        // abilites menu should take an object of type base ability system.
        // It should access its string name,
        // access its image
        // also check to make sure its of the correct type
        // further expantion can also check here to see if it is a hold or multi tap ability.
        // Also have to account for phantom abilites vs regular player abilites. 
        public static event Action updateChosenSkillUI;
        [SerializeField] GameObject abilityEntryPrefab;
        List<BaseAbilitySystem> abilities;
        public enum triggerChoice { leftTrigger = 0, rightTrigger = 1 };
        public triggerChoice selectedTrigger;

        [SerializeField] BaseAbilitySystem abilityLeft;
        [SerializeField] BaseAbilitySystem abilityRight;
        // skill lists
        [SerializeField] private GameObject SelectableSkills; // might turn selectable skill and skill slots back to lists. 
        [SerializeField] private GameObject skillSlotslist;
        [SerializeField] private Sprite testSprite;
        [SerializeField] private phantomAbilites[] phantomSkill;
        #region oldCodeIdeas
        // due to the small scope of this project i'll just leave the abilites all in one place (no shop system at the moment)
        /*    [SerializeField] PhantomStealAbility stealAbility;
              [SerializeField] PhantomCombatMimicAbility mimicAbility;
           // [SerializeField] private GameObject[] skillSlots;*/
        #endregion
        public PlayerStateMachine machine;



        private void Awake()
        {
            foreach (Transform skill in skillSlotslist.transform)
            {
                //if (skill == null) continue;
                skill.gameObject.GetComponent<SkillSlots>().SetSprite(null);
            }

            
        }
        private void OnEnable()
        {
            SkillSelection.Initiate_Action += SkillSelection_initiate_Action;
            SkillSelection.AbilityToSet += SkillSelection_abilityToSet;
        }


        private void SkillSelection_initiate_Action(Sprite icon)
        {
            foreach (Transform skill in skillSlotslist.transform)
            {
                // if the item  value already exists, return
                /*  if (skill.gameObject.GetComponent<SkillSlots>().skillSprite.sprite == skill.gameObject.GetComponent<SkillSlots>().GetSprite())
                  { break; }*/
                if (icon == skill.gameObject.GetComponent<SkillSlots>().skillSprite.sprite) { return; }
                if (skill.gameObject.GetComponent<SkillSlots>().skillSprite.sprite == null)
                {
                    skill.gameObject.GetComponent<SkillSlots>().SetSprite(icon);
                    break;
                }
                else { continue; }

            }
            updateChosenSkillUI?.Invoke();
        }

        private void SkillSelection_abilityToSet(string name)
        {
            for (int i = 0; i < skillSlotslist.transform.childCount; i++)
            {
                //if (name == skillSlotslist.transform.GetChild(i).GetComponent<SkillSlots>().skillName) { return; }
                if(skillSlotslist.transform.GetChild(i).GetComponent<SkillSlots>().skillName == "")
                {
                    skillSlotslist.transform.GetChild(i).GetComponent<SkillSlots>().SetName(name);
                    break;
                }
                

                else { continue;}
               // continue;
                //break;
           /*     if (name == skillSlotslist.transform.GetChild(i).GetComponent<SkillSlots>().skillName) { return; }
                if (skillSlotslist.transform.GetChild(i).GetComponent<SkillSlots>().skillName == null) 
                {
                    skillSlotslist.transform.GetChild(i).GetComponent<SkillSlots>().SetName(name);
                    break;
                }
                else { continue; };*/
            }
/*
                foreach (Transform skill in skillSlotslist.transform)
            {
                
                // if the item  value already exists, return
                *//*  if (skill.gameObject.GetComponent<SkillSlots>().skillSprite.sprite == skill.gameObject.GetComponent<SkillSlots>().GetSprite())
                  { break; }*//*
                if (name == skill.gameObject.GetComponent<SkillSlots>().skillName) { return; }
                if (skill.gameObject.GetComponent<SkillSlots>().skillName == null)
                {
                    skill.gameObject.GetComponent<SkillSlots>().SetName(name);
                    break;
                }
                else { continue; }

            }*/
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
            /* if (machine.InputReader.isLeftSpecial == true)
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
             abilityRight.SetHoldStatus(machine.InputReader.isRightSpecial);*/
        }

        public void AddAbilityEntry(string abilityName, Sprite abilityIcon, BaseAbilitySystem.IAbilitesDelegate useAbility)
        {
            GameObject entry = Instantiate(abilityEntryPrefab, transform);

            BaseAbilitySystem abs = entry.GetComponent<BaseAbilitySystem>();
            //  abs.GetAbilityName();


        }

      /*  public void OpenMenu()
        {
            //menuIsOpen = true;
            print("Opened abilites menu");
            if (abilities.Count == 0)
            {
                for (int i = 0; i < 1; i++)
                {
                    AddAbilityEntry(phantomSkill[i].GetAbilityName(), phantomSkill[i].GetAbilityImageIcon(), stealAbilityDelegate); // will add another boolean or option so when a skill is selected it flips a switch on the script
                                                                                                                                    // this "switch" will let the script know its currently the one selected so that other features can work properly like the gaurd system
                    *//* AddAbilityEntry(mimicAbility.GetAbilityName(), mimicAbility.GetAbilityImageIcon(), CombatMimicAbilityDelegate);*//*

                }
            }
        }*/

        void stealAbilityDelegate(IAbilites ability)
        {
            int j = 1;
            phantomSkill[j].UseAbility();
            //INSTEAD OF STEALABILITY it will be an ability of type PHANTOM ABILITY .useability
        }

        /* void CombatMimicAbilityDelegate(IAbilites ability)
         {
             mimicAbility.UseAbility();
         }*/

        private void OnDisable()
        {
            SkillSelection.Initiate_Action -= SkillSelection_initiate_Action;
            SkillSelection.AbilityToSet -= SkillSelection_abilityToSet;
        }
    }
}

