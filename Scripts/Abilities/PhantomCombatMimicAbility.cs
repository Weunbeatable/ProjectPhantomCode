using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomCombatMimicAbility : phantomAbilites
{
    /// <summary>
    /// This class is meant to mimic the use case of the testPhantomFighter class, albeit more "polished"
    /// purpose of this class is to utilize the attack mimicry skill
    /// This skill has been reworked in some of the following ways
    /// Triggering the skill button  should check to see if your combo list is full
    /// it should also check if recording is active
    /// recording is only active if combo list isn't full and button has been pressed once, 
    /// if you tap again recording stops you can always record again until you reach 10 attacks.
    /// A visual will be accompanied to let the user know that recording is active. 
    /// (some kind of aura or shader change). 
    /// once ready you can unleash attacks at your desire just like you could in test phantom
    /// Modifications will be made to player attacking state to update how and when
    /// information should be passed on. 
    /// tapping should enable or disable recording (sending events)
    /// holding should actually do the play
    /// </summary>
    public static event Action<bool> onActivateRecording;

    
    [Header("Boolean checks")]
    bool isStealActive;
    private bool shouldGaurd = false;

    [Header("Data Containers")]
    public Queue<string> PhantomAttacks;
    public static List<string> playPhantomAttacks;

    public Queue<string> PhantomControllers;
    public static List<string> SwithchPhantomController;
    public Vector3 offset;

    private bool abilityTriggered;
    public static bool istapped { set; get; }

    public static PhantomCombatMimicAbility Instance;

    public override void Awake()
    {
        base.Awake();
       // Debug.Log("host name is " + GetHostAnimator().name);
        //   Debug.Log("ghost name is "+ GetGhostAnimator().name);

    }
    public override void Start()
    {
        base.Start();
        abilityTriggered = false;
       // Debug.Log("loaded controller is " + GetFighterController().name);
        PhantomAttacks = new Queue<string>();
        playPhantomAttacks = new List<string>();
        PhantomControllers = new Queue<string>();
        SwithchPhantomController = new List<string>();

        PhantomControllers.Clear();
        PhantomAttacks.Clear();
        //triggerPhantom.GetComponent<Effects>();
        playPhantomAttacks.Clear();

        isStealActive = false;
       /* Debug.Log("Controller name is " + GetFighterControllerName());
        Debug.Log("Controller name is " +GetGunControllerName());
        Debug.Log("Controller name is " + GetGreatSwordControllerName());
        Debug.Log("Controller name is " + GetFighterControllerName());*/
    }

    private void Update()
    {
        TurnPhantomOnandOff();
        if (GetTapStatus() == true && PhantomAttacks.Count < 1) //Input.GetKeyDown(KeyCode.Z)
        {
            istapped = !istapped;
        }
        // if you are recording attacks invoke an event that triggers the greyscaled component
        // Once you get to 10 attacks should play a clock sound
        string currentvalue, controllerCurrentValue;
        if (SwithchPhantomController.Count == 10)
        {
            for (int i = 0; i < SwithchPhantomController.Count; i++)
            {
                if (PhantomControllers.Count <= 10)
                {
                    controllerCurrentValue = SwithchPhantomController[i];
                    PhantomControllers.Enqueue(controllerCurrentValue);
                }

            }
        }
        if (playPhantomAttacks.Count == 10)
        {
            for (int i = 0; i < playPhantomAttacks.Count; i++)
            {
                if (PhantomAttacks.Count <= 10)
                {
                    currentvalue = playPhantomAttacks[i];
                    PhantomAttacks.Enqueue(currentvalue);
                }

            }
        }
        if (playPhantomAttacks.Count > 10) { return; } // Prevent overloading the container, may not be necessary as container is cleared in attacking states but added as a precautionary measure. 

        if (PhantomAttacks.Count <= 9)
        {
            GetPhantomContainer().transform.parent = null;


        }
        if (GetHoldStatus() == true) //Input.GetKeyDown(KeyCode.X
        {

            Debug.Log("attak count is " + PhantomAttacks.Count);
            if (PhantomAttacks.Count > 0)
            {

                //  triggerPhantom.PlayPhantomCastingTimeEffect();
                ApproachTarget();
                currentAnimatorControllerManagment();
                // phantomContainer.gameObject.SetActive(true);

                //CurrentAnimatorControllerManagment(); // MEWLY ADDED LINE

                if (GetGhostAnimator().GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
                {
                    NextAnimatorControllerManagment(); // first check the state of the controller, make a change if necessary
                    GetGhostAnimator().Play(PhantomAttacks.Dequeue()); // play attack
                    PhantomControllers.Dequeue(); // remove item from controller that we peeked at
                    //phantomContainer.transform.parent = playerPos.transform; // phantom positioning
                }
                //
            }
        }

    }

    public override string GetAbilityName()
    {
        return "Combat Mimic";
    }

    public override void UseAbility()
    {
        MimicLogic();
    }

    private void NextAnimatorControllerManagment()
    {

        string checkNextController = PhantomControllers.Peek();
        if (GetGhostAnimator().runtimeAnimatorController.name != checkNextController)
        {
            if (checkNextController == GetFighterControllerName())
            {
                GetPropHandler().FightingProps();
                GetGhostAnimator().runtimeAnimatorController = GetFighterController();

            }
            else if (checkNextController == GetGunControllerName())
            {
                GetPropHandler().ShootingProps();
                GetGhostAnimator().runtimeAnimatorController = GetGunslingerController();

            }
            else if (checkNextController == GetGreatSwordControllerName())
            {
                GetPropHandler().GreatSwordsmanProps();
                GetGhostAnimator().runtimeAnimatorController = GetGreatSwordController();


            }
            else if (checkNextController == GetAssasinControllerName())
            {
                GetPropHandler().AssasinationProps();
                GetGhostAnimator().runtimeAnimatorController = GetAssassinController();


            }
            // peek the next controller if the next controller name is different switch animation controllers before returning to top 
            // of loop
        }
    }
    private void currentAnimatorControllerManagment()
    {

        string checkNextController = GetGhostAnimator().runtimeAnimatorController.name;
            if (checkNextController == GetFighterControllerName())
            {
                GetPropHandler().FightingProps();
                GetGhostAnimator().runtimeAnimatorController = GetFighterController();

            }
            else if (checkNextController == GetGunControllerName())
            {
                GetPropHandler().ShootingProps();
                GetGhostAnimator().runtimeAnimatorController = GetGunslingerController();

            }
            else if (checkNextController == GetGreatSwordControllerName())
            {
                GetPropHandler().GreatSwordsmanProps();
                GetGhostAnimator().runtimeAnimatorController = GetGreatSwordController();


            }
            else if (checkNextController == GetAssasinControllerName())
            {
                GetPropHandler().AssasinationProps();
                GetGhostAnimator().runtimeAnimatorController = GetAssassinController();


            }
            // peek the next controller if the next controller name is different switch animation controllers before returning to top 
            // of loop
    }


    private void MimicLogic()
    {
        // button swap
        IsMimicActivated();

        if (abilityTriggered == true)
        {
            string currentvalue, controllerCurrentValue;
            if (SwithchPhantomController.Count == 10)
            {
                for (int i = 0; i < SwithchPhantomController.Count; i++)
                {
                    if (PhantomControllers.Count <= 10)
                    {
                        controllerCurrentValue = SwithchPhantomController[i];
                        PhantomControllers.Enqueue(controllerCurrentValue);
                    }

                }
            }
            if (playPhantomAttacks.Count == 10)
            {
                for (int i = 0; i < playPhantomAttacks.Count; i++)
                {
                    if (PhantomAttacks.Count <= 10)
                    {
                        currentvalue = playPhantomAttacks[i];
                        PhantomAttacks.Enqueue(currentvalue);
                    }

                }
            }
            if (playPhantomAttacks.Count > 10) { return; } // Prevent overloading the container, may not be necessary as container is cleared in attacking states but added as a precautionary measure. 

            if (PhantomAttacks != null)
            {
                if (PhantomAttacks.Count <= 9)
                {
                    GetPhantomContainer().transform.parent = null;


                }
            }
        }


        if (abilityTriggered == true && PhantomControllers != null)
        {


            if (PhantomAttacks.Count > 0)
            {

                //  triggerPhantom.PlayPhantomCastingTimeEffect();
                ApproachTarget();
                // phantomContainer.gameObject.SetActive(true);

                //CurrentAnimatorControllerManagment(); // MEWLY ADDED LINE

                if (GetGhostAnimator().GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
                {
                    NextAnimatorControllerManagment(); // first check the state of the controller, make a change if necessary
                    GetGhostAnimator().Play(PhantomAttacks.Dequeue()); // play attack
                    PhantomControllers.Dequeue(); // remove item from controller that we peeked at
                    //phantomContainer.transform.parent = playerPos.transform; // phantom positioning
                }
                //
            }
        }

        // Code has been reduced from 200 lines to 69 :D
    }

    private void IsMimicActivated()
    {
        if (abilityTriggered == true)
        {
            abilityTriggered = false;
        }
        else if (abilityTriggered == false)
        {
            abilityTriggered = true;
        }
        if(!abilityTriggered == true)
        {
            onActivateRecording?.Invoke(abilityTriggered);
        }
    }

    public bool GetAbilityTrigger() => abilityTriggered;
}
