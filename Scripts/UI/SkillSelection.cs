using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The purpose of this script is to provide a visual update to skills that are selected while at the same time updating
// the players Skill slot
// The UI on the skills tab will update to show the order of available skills. 
// Next a callback will be made to update the skills available to the player for use. 
// by default value should be null 
namespace PFF.UI
{
    public class SkillSelection : MonoBehaviour
    {
        
        public static event Action<Sprite> Initiate_Action;
        public static event Action<string> AbilityToSet;
        public Image skillSprite;

        [SerializeField] public string abilityName;

        private void Awake()
        {
        }
        public void InitiateCallback()
        {
           
            Initiate_Action?.Invoke(skillSprite.sprite);
            AbilityToSet?.Invoke(GetSkillName());
        }

        public string GetSkillName() { return abilityName; }
        public Sprite GetSprite() { return skillSprite.sprite; }
    }
}

