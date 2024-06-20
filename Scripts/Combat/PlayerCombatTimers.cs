using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatTimers : MonoBehaviour
{
    //TODO: Make an event that lets the vfx flash for a second
    public static PlayerCombatTimers Instance { get; private set; }

    public static event Action OnEquipWeapon;
    public static event Action<bool> OnCallToSwapBlendTrees;
    public static event Action onCleanupDash;
    public static event Action ReachedHighVelocity;
    public static event Action<bool> OnTriggerChargeFlash;


    private readonly int ChargeTimeHash = Animator.StringToHash("ChargeTime");
    private bool isFullCharge;
    private bool shouldTriggerflash;
    private string animatorIsFullChargeVariable = "isFullCharge";
    //Charge Modifiers
    [field: Header("Dashing variables")]
    public float HeavyChargeTime { get;  set; }
    public bool isChargingHeavy;
    public PlayerStateMachine updateState;
    public float testFloat;

    [field: Header("WeaponSwap Variables")]
    private bool isWeaponEquiped { get; set; }
    public bool SwapTree;

    [field: Header("Parrying Variables")]
    public bool isInParryState { get; set; }
    private float slowMotion; 

    // Start is called before the first frame update
    private float isNotOutOfFreelook = 5f;
    private void Awake()
    {
        if(Instance != null)
        {
            Debug.Log("Already have more than one combatTimer Running... Somewhere... Hmm " + Instance + transform);
            Destroy(gameObject);
            Instance = this;
        }
        else { return; }
        slowMotion = 1f;
    }
    void Start()
    {
        PlayerFreeLookState.OnSwitchToWeaponEquip += PlayerFreeLookState_OnSwitchToWeaponEquip;
        isChargingHeavy = false;
    }

    private void OnEnable()
    {
        WeaponDamage.onParried += WeaponDamage_onParried;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWeaponEquiped)
        {
            OnEquipWeapon?.Invoke(); // call to equip/unequpi weapon
        }
        isNotOutOfFreelook -= Time.deltaTime;

        AreWeInFreelook();

        updateChargeAnimations();

        DashCooldownVisualTriggerCleanup();

        HighVelocityVisualTrigger();// this isn't really a timer may need to move it
    }

    private void HighVelocityVisualTrigger()
    {
        updateState.MonitoredCharacterVelocity = Mathf.Abs(updateState.characterController.velocity.z);
        if (Mathf.Abs(updateState.characterController.velocity.z) >= 17.5)
        {
            ReachedHighVelocity?.Invoke();
            return;
        }
    }

    private void DashCooldownVisualTriggerCleanup()
    {
        if (updateState.dashCoolDownTimer <= 0)
        {
            onCleanupDash?.Invoke();
        }
    }

    private void AreWeInFreelook()
    {
        if (isNotOutOfFreelook < 0f)
        {
           // Debug.Log("Now putting Away Weapon ");
            SwapTree = false;
        }
        else
        {
          //  Debug.Log("Weapon Has Been Equipped ");
            SwapTree = true;
        }
        OnCallToSwapBlendTrees?.Invoke(SwapTree);
    }

    private void OnDisable()
    {
        WeaponDamage.onParried -= WeaponDamage_onParried;
    }
    private void OnDestroy()
    {
        PlayerFreeLookState.OnSwitchToWeaponEquip -= PlayerFreeLookState_OnSwitchToWeaponEquip;
        
    }

    private void PlayerFreeLookState_OnSwitchToWeaponEquip(bool obj)
    {
        if (obj == true)
        {
            isNotOutOfFreelook = 8f;
        }
    }

    void updateChargeAnimations()
    {
        float normalizedTime = GetNormalizedTime(updateState.Animator, "Attack");
        if (normalizedTime < 1f)
        {
            if (isChargingHeavy == true)
            {
                isFullCharge = false;
                if (normalizedTime < 0.5f && !updateState.InputReader.isHeavyHoldAttack)
                {
                    isFullCharge = false;
                    shouldTriggerflash = false;
                    updateState.GetComponent<PlayerStateMachine>().Animator.SetBool(animatorIsFullChargeVariable, isFullCharge);
                }
                else if (normalizedTime > 0.5f && updateState.InputReader.isHeavyHoldAttack)
                {
                    isFullCharge = true;
                    shouldTriggerflash = true;
                    if (shouldTriggerflash == true)
                    {
                        OnTriggerChargeFlash?.Invoke(shouldTriggerflash);
                    }
                    shouldTriggerflash = false;
                    // Have to reset boolean trigger to true otherwise if default is false and exit time is off it will auto play
                    // false condition and then set trigger to to, also causing invoke of flash incorrectly aka unwanted behavior.
                    updateState.GetComponent<PlayerStateMachine>().Animator.SetBool(animatorIsFullChargeVariable, isFullCharge);
                    isChargingHeavy = false;
                }
            }
        }
        else
        {
            isChargingHeavy = false;
            updateState.GetComponent<PlayerStateMachine>().Animator.SetBool(animatorIsFullChargeVariable, true);
            return;
        }
    }
    protected float GetNormalizedTime(Animator animator, string tag) // purpose here is to check how far through the animation and if we pass a threshold so if they player is still attacking or holding attack , go to the next animation.
    {
        
        AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0); // gettinig animation layer info and storing in variables
        AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

        if (animator.IsInTransition(0) && nextInfo.IsTag(tag)) // already know we are in layer 0, so if we are in layer 0 and transitioning to an attack we want to get the data for the next state.

        {
            return nextInfo.normalizedTime;
        }
        else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag)) // if not in an attack animation case
        {
            return currentInfo.normalizedTime;
        }
        else
        {
            return 0f; // just in case none of this is true;s
        }
    }

    public bool GetParryState() => isInParryState;

    public void TestState()
    {
        Debug.Log("Accessing Combat Timer");
    }
    private void WeaponDamage_onParried(object sender, EventArgs e)
    {
        if(slowMotion > 0)
        {
            Time.timeScale = 0.5f;
            slowMotion -= Time.deltaTime;
        }

        Time.timeScale = 1f;
        slowMotion = 1f;
    }

    public float GetSlowMotionValue() => slowMotion;
}
