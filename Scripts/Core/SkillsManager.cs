using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PFF.core
{
    public class SkillsManager : MonoBehaviour
    {
        public static SkillsManager Instance { get; private set; } // we only need 1 field for the class so its static
        Dictionary<string, Action> addSkillsDictionary = new Dictionary<string, Action>();
        // having a remove dictionary is convoluted and overcomplicated. 
      //  Dictionary<string, Action> removeSkillsDictionary = new Dictionary<string, Action>();
        [SerializeField] private string phantomSteal = "PhantomSteal";
        [SerializeField] private string PhantomMimic = "PhantomMimic";
        [SerializeField] private string PhantomDash = "PhantomDash";
        [SerializeField] private string PhantomCounter = "PhantomCounter";
        public GameObject player;

        private void Awake()
        {
            player = GameObject.FindWithTag("Phantom");
            Debug.Log("Player name is " + player.name);

            addSkillsDictionary.Add(phantomSteal, AddPhantomSteal);
            addSkillsDictionary.Add(PhantomMimic, AddPhantomMimic);
            addSkillsDictionary.Add(PhantomDash, AddPhantomDash);
            addSkillsDictionary.Add(PhantomCounter, AddPhantomCounter);

/*            removeSkillsDictionary.Add(phantomSteal, RemovePhantomSteal);
            removeSkillsDictionary.Add(PhantomMimic , RemovePhantomMimic);
            removeSkillsDictionary.Add(PhantomDash , RemovePhantomDash);
            removeSkillsDictionary.Add (PhantomCounter, RemovePhantomCounter);*/

            // error check for more than one instance, hence singleton, we'll delete the extra
            if (Instance != null)
            {
                Debug.Log("There's more than one SkillsManager! " + transform + "_" + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
 /*       private void Start()
        {
            Action trend = null;
            if(skillsDictionary.TryGetValue(phantomSteal, out trend))
            {
                trend();
                // Have a delegate call here to invoke the mehtod. 
            }
           
        }*/
        public Dictionary<string, Action> GetAddSkillsDictionary() => addSkillsDictionary;
       // public Dictionary<string, Action> GetRemoveSkillsDictionary() => removeSkillsDictionary;
        public string GetPhantomStealKey() => phantomSteal;
        public string GetPhantomMimicKey() => PhantomMimic;
        public string GetPhantomDashKey() => PhantomDash;
        public string GetPhantomCounterKey() => PhantomCounter;

        public void AddPhantomSteal()
        {
            player.AddComponent<PhantomStealAbility>();
        }
        public void AddPhantomMimic()
        {
            player.AddComponent<PhantomCombatMimicAbility>();
        }
        public void AddPhantomDash()
        {
            player.AddComponent<PhantomDashAbility>();
        }
        public void AddPhantomCounter()
        {
            player.AddComponent<PhantomCombatMimicAbility>();
        }

        public void RemovePhantomSteal() {
                Destroy(GetComponent<PhantomStealAbility>());
        }
        public void RemovePhantomMimic() {
                Destroy(GetComponent<PhantomCombatMimicAbility>()); 
        }
        public void RemovePhantomDash() {
                Destroy(GetComponent<PhantomDashAbility>()); 
        }
        public void RemovePhantomCounter() { 
                Destroy(GetComponent<PhantomCombatMimicAbility>());
        }

        public void RemoveAnyPhantomAbilities()
        {
            phantomAbilites CheckAbility = player.gameObject.GetComponent<phantomAbilites>();   
            if ( CheckAbility) // ability is there
            {
                Destroy(CheckAbility);
            }
        }
    }
}

