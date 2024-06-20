using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PFF.Combat
{
    //An assumption to be made is every class will have kicks available, the only remaining factor is if the weapon is dual wielding or left/right handed
    // May create an enum for specifics of a class e.g. has a blocking property like shields (Sword and shield class S&S) or movement abilites - if they do we will reference the appropriate script.
    [CreateAssetMenu(fileName = "Weapon", menuName = "PFFWeapons/MakeNewWeapon", order = 0)]
    public class WeaponSO : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] GameObject SecondaryHandItem = null;
        [SerializeField] GameObject props = null;
        [SerializeField] AudioClip weaponAttackSound = null;
        [SerializeField] AudioClip weaponSwingSound = null;
        [SerializeField] WeaponDamage weaponDamageController = null;

        [SerializeField] bool isRightHanded;
        [SerializeField] bool isLeftHanded;
        [SerializeField] bool isDualWielding;
        [SerializeField] float damageValue;
        [SerializeField] float weaponRange;
        [SerializeField] bool hasHitstun;

        [SerializeField] public enum SpecialProperties { BlockingTrait = 0, ExtensionTrait = 1, noHitstunTrait = 2 };
        public void Spawn(Transform rightHand, Transform leftHand, Transform dualWield, Animator animator)
        {
            
            if(weaponOverride != null)
            {
                Transform handTransform;
                if (isRightHanded )
                {
                    handTransform = rightHand;
                    isLeftHanded = false;
                    isDualWielding = false;
                    Instantiate(weaponPrefab, handTransform);
                }
                else if (isLeftHanded)
                {
                    handTransform = leftHand;
                    isRightHanded = false;
                    isDualWielding  = false;
                    Instantiate(weaponPrefab, leftHand);
                }
                else
                {
                    isDualWielding = true;
                    isLeftHanded = false;
                    isRightHanded = false;
                    // grab the first child assign it as right hand
                    // grab second child assign it as left hand
                    // Assumption: That prefab is structured in a way that the parent is empty and nested is a dual wielded weapon.
                    // We also assume that the order goes right hand first then left hand. right -> left.
                    Instantiate(weaponPrefab.transform.GetChild(0), rightHand);
                    Instantiate(weaponPrefab.transform.GetChild(1), leftHand);
                }
                
            }
            
            if(weaponOverride != null)
            {
                animator.runtimeAnimatorController = weaponOverride;
            }
            

        }

        public float GetDamage() => damageValue;
        public float GetRange() => weaponRange;

        public bool GetIsDualWeilding() => isDualWielding;
        
        public bool GetIsRightHanded() => isRightHanded;

        public bool GetIsLeftHanded() => isLeftHanded;

        public GameObject GetProps() => props;

        public AnimatorOverrideController GetOverrideController () => weaponOverride;

    }
}

