using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PFF.Combat
{
    //An assumption to be made is every class will have kicks available, the only remaining factor is if the weapon is dual wielding or left/right handed
    // May create an enum for specifics of a class e.g. has a blocking property like shields (Sword and shield class S&S) or movement abilites - if they do we will reference
    // the appropriate script.
    // Legs will always be active.  
    [CreateAssetMenu(fileName = "Weapon", menuName = "PFFWeapons/MakeNewWeapon", order = 0)]
    public class WeaponSO : ScriptableObject
    {
        [Header("Components for reference")]
        [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] RuntimeAnimatorController weaponRuntimeAnimatorController = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] GameObject SecondaryHandItem = null;
        [SerializeField] GameObject throwables = null;
        [SerializeField] GameObject props = null;
        [SerializeField] AudioClip weaponLightAttackSound = null;
        [SerializeField] AudioClip weaponHeavyAttackSound = null;
        [SerializeField] AudioClip weaponLightSwingSound = null;
        [SerializeField] AudioClip weaponHeavySwingSound = null;

        [Header("preset weapon values")]
        [SerializeField] int postureValue = 0;
        [SerializeField] float primaryDamageValue;
        [SerializeField] float secondaryDamageValue;

        [Header("Weapon specific Properties")]
        [SerializeField] bool isRightHanded;
        [SerializeField] bool isLeftHanded;
        [SerializeField] bool isDualWielding;  
        [SerializeField] float weaponRange;
        [SerializeField] bool hasHitstun;
        [SerializeField] bool hasthrowables;
        [SerializeField] bool hasSpecialGuard;
        [SerializeField] bool hasStance;

        [SerializeField] public enum SpecialProperties { BlockingTrait = 0, ExtensionTrait = 1, noHitstunTrait = 2 };
        public void Spawn(Transform rightHand, Transform leftHand, Transform dualWield, Animator animator)
        {
            
            if(weaponRuntimeAnimatorController != null)
            {
                Transform handTransform;
                if (isRightHanded )
                {
                    handTransform = rightHand;
                    Instantiate(weaponPrefab, handTransform);
                }
                else if (isLeftHanded)
                {
                    handTransform = leftHand;
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

        public float GetDamage() => primaryDamageValue;
        public float GetRange() => weaponRange;

        public bool GetIsDualWeilding() => isDualWielding;
        
        public bool GetIsRightHanded() => isRightHanded;

        public bool GetIsLeftHanded() => isLeftHanded;

        public bool GetHasHitStun() => hasHitstun;

        public bool GetHasThrowables() => hasthrowables;
        
        public bool GetHasSpecialGuard() => hasSpecialGuard;

        public GameObject GetProps() => props;

        public AnimatorOverrideController GetOverrideController () => weaponOverride;

        public RuntimeAnimatorController GetRuntimeAnimatorController () => weaponRuntimeAnimatorController;

    }
    /*
     * Require component code example, useful for when making individual weapon type traits. 
     * using UnityEngine;

// PlayerScript requires the GameObject to have a Rigidbody component
[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector3.up);
    }
}
    */
}

