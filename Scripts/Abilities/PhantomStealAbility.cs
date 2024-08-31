using PFF.Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class PhantomStealAbility : phantomAbilites, IAbilites
{
    [Header("Audio Data")]
   // [SerializeField] private AudioSource PhantomSource; // not sure I need this... can just  have one audio source playing from the phantom
   // [SerializeField] private AudioClip phantomStealAudio;
   // This class needs to change, using the skill will force invoke a special blocking state ( the exact same blocking state as before)
   // only this time it will allow the use of parry system, causing the invocation of parrying to trigger the skill steal
   // If you wiff the skill goes on CD
   // Instead of a one time use stolen skills can be used 3 times
   // after 3 uses skills will be exhausted. 
   // forcinng the skill to go on CD (May also have a timer depending on how behavior is so either use all of the skilll within a timer
   // or reset skill

    
    private readonly int play = Animator.StringToHash("PhantomSkillSteal");

    public static event Action<PhantomStealAbility> About;
    public static event Action SpecialBlockTrigger;

    private int ghostAttackHash;
    private int stolenAttackUseCounter;

    bool isStealActive;
    private bool shouldGaurd = false;
    public static bool istapped { set; get; }
    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
       base.Start();
        stolenAttackUseCounter = 3;
    }

    private void OnEnable()
    {
        WeaponDamage.onParried += WeaponDamage_onParried;
        GetPlayerInputs().RightSpecialEvent += PhantomStealAbility_RightSpecialEvent;
        GetPlayerInputs().RightSpecialHoldEvent += PhantomStealAbility_RightSpecialHoldEvent;
    }

  

    private void OnDisable()
    {
        WeaponDamage.onParried -= WeaponDamage_onParried;
        GetPlayerInputs().RightSpecialEvent -= PhantomStealAbility_RightSpecialEvent;
        GetPlayerInputs().RightSpecialHoldEvent -= PhantomStealAbility_RightSpecialHoldEvent;
        isStealActive = false;
    }
    private void PhantomStealAbility_RightSpecialEvent()
    {
            SetGhostAnimator(GetCombatModifier().stolenAnimationController);
            istapped = !istapped;
            ApproachTarget();
            //  GetPhantomContainer().gameObject.SetActive(true);

            GetGhostAnimator().Play(ghostAttackHash);
            ghostAttackHash = 0;
        if (GetGhostAnimator().GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
        {

            //  GetPhantomContainer().gameObject.SetActive(false);
            SetTapStatus(false);

            //  ghostAttackHash = 0;
        }
        if (ghostAttackHash == 0)
        {
            GetCombatModifier().stolenAnimationController = null;
        }
    }

    private void PhantomStealAbility_RightSpecialHoldEvent()
    {
        throw new NotImplementedException();
    }
    // Update is called once per frame
    void Update()
    {
            ghostAttackHash = GetCombatModifier().combatClipHash;
        //  Debug.Log("ghost hash value is " +  ghostAttackHash);
        if (GetPhantomContainer() != null)
        {
            TurnPhantomOnandOff();
        }
        if (GetCombatModifier().stolenAnimationController == null || ghostAttackHash == 0)
        {
            return;
        }

        Debug.Log("is something playing " + GetGhostAnimator().GetCurrentAnimatorStateInfo(0));
       
    }

    public override void UseAbility()
    {
        Steal();
    }
    public void Special_Block_Parry()
    {

        if (GetTapStatus() == true)
        {
            //TODO: CREATE A SPECIAL BLOCK STATE SPECIFICALALY FOR THIS ACTION, 
            // just duplicate the block animation but turn off looping. and remove boolean checks in tick and other animation state possibilites.
            // except freelook state, include parry state as well. This should fix the issue
            // also add a cooldown feature lol 
            // can even use text mesh pro and just have some text counting down 
            SpecialBlockTrigger?.Invoke();
            Debug.Log("Should trigger special block");
            if (GetGhostAnimator().GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
            {
                shouldGaurd = true;
            }

        }

        if (GetCombatModifier() != null)
        {

            ghostAttackHash = GetCombatModifier().combatClipHash;
        }
        if(ghostAttackHash != 0) { shouldGaurd = true; }

        if (shouldGaurd == true)
        {
            if (GetTapStatus() == true && GetCombatModifier() != null)
            {
                TurnPhantomOnandOff();
                Debug.Log("Should trigger ability");
                SetTapStatus(false);
                // DO THE COUNTER JIMMY

                if (ghostAttackHash != 0)
                {
                    ghostAttackHash = GetCombatModifier().combatClipHash;
                }
                if (GetGhostAnimator() != null)
                {
                    SetGhostAnimator(GetCombatModifier().stolenAnimationController);
                }

                if (stolenAttackUseCounter > 0)
                {
                    if (GetTargeter().transform.position != null)
                    {
                        ApproachTarget();
                        // phantomContainer.gameObject.SetActive(true);

                    }
                    GetGhostAnimator().Play(ghostAttackHash);
                    stolenAttackUseCounter--;
                    Debug.Log("Counter sitting at " + stolenAttackUseCounter);
                }
            }
            if (GetGhostAnimator().GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
            {
                shouldGaurd = false;
            }
        }
        
    }
    public void WeaponDamage_onParried(object sender, System.EventArgs e)
    {
        
        // Find the dot product of the enemy and spawn behind him. Play an animation
        Vector3 directionBehind = GetTargeter().currentTarget.transform.position.normalized;
        directionBehind.y -= 0.4f;
        directionBehind.z -= 2.2f;


        this.transform.position = directionBehind; // Set character position to offset otherwise Phantom will be too close causing weird issues and collisons
        // will prevent the proper feel 

        this.transform.RotateAround(GetTargeter().currentTarget.transform.position, Vector3.up, 90 * Time.deltaTime); // spin around 


        Vector3 forward = this.transform.TransformDirection(Vector3.forward); // Find the forward vector of phantom
        Vector3 toOther = GetTargeter().currentTarget.transform.position - this.transform.position; // find the vector position of target - player


        if (Vector3.Dot(forward, toOther) < -0.98f) // I'm using the dot product here to find if i'm behind the enemy or not, I don't need an exact value.
                                                    // doesn't have to be exactly -1 as that can be problematic in calculation so a ballpark figure will help to get the same results
        {
            print("I'm behind the enemy!");
            this.transform.position = toOther;//Vector3.Lerp(this.transform.position, directionBehind, 0.05f / Time.deltaTime);
            FaceTarget();
        }
        //Once behind the enemy trigger everything else so you can view the enemy

        //PhantomSource.PlayOneShot(phantomStealAudio);
        // StealEffects.Play();

        isStealActive = true;

        if(GetGhostAnimator().runtimeAnimatorController != GetFighterController())
        {
            SetGhostAnimator(GetFighterController());
        }
        GetPhantomContainer().gameObject.SetActive(true);
        GetGhostAnimator().Play(play);
        if (GetGhostAnimator().GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            GetPhantomContainer().gameObject.SetActive(true);

        }
    }
    public void Steal()
    {
        // PlayerParryingState_onSpecialParryTriggered();
        Special_Block_Parry();
    }


    public override string GetAbilityName()
    {
        return "Phantom Thief";
    }

    public override void AddComponent()
    {
        PhantomFighter.AddComponent<PhantomStealAbility>();
    }

    public override void RemoveComponent()
    {
        if(PhantomFighter.GetComponent<PhantomStealAbility>() != null)
        Destroy(PhantomFighter.GetComponent<PhantomStealAbility>());
    }
}
