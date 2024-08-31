using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class phantomAbilites : BaseAbilitySystem
{
    // Start is called before the first frame update
    [Header("Animators")]
    [SerializeField] private Animator hostAnimator;
    [SerializeField] private Animator ghostAnimator;


    // getters for animatiors have been applied via the Ability Manager, this serves 2 purposes
    // ease of access across multiple class types (derived classes of the ability system for both player and phantom
    // ease of resource loading, this prevents duplication of loading resources that would otherwise be on both sets of managers.
    // They will be gotten from the Ability manager and inherited into both archetypes. 
    // DUE to the different controllers being used in for phantom and player and the derivation from an abstract class its better to 
    // call on animators from a persistent scene instance that contains a gameobjectr with the script like Ability Manager. 
    // this will also help for future adjustments and prevent me having to switch controllers depending on ability system being used. 

    [Header("Input Reader Reference")]
    public GameObject PhantomFighter;
    [Header("Input Reader Reference")]
    public InputReader playerInputs;
    //public PlayerAttackingState playersATtacks;


    [Header("Anchor Points")]
    [SerializeField] private Transform playerPos; // should reference this through scripting. 
    [SerializeField] private GameObject phantomContainer;
    [SerializeField] private Targeter targeter;

    [Header("Prop Reference")]
    private PropHandler props;
    //TODO: apply getters for these public variables
    [SerializeField] private CombatModifiers playerModifier;
/* made obsolete by instance manager
    [Header("String reference to controllers")]
    // String refrence for animatiors, string REF ARE DANGEROUS
    [SerializeField] private string fightController;
    [SerializeField] private string gunController;
    [SerializeField] private string greatSwordController;
    [SerializeField] private string assasinController;
*/

    public virtual void Awake()
    {
        PhantomFighter = GameObject.FindWithTag("Phantom");
        props = GameObject.FindWithTag("Phantom").GetComponent<PropHandler>();
        hostAnimator = GameObject.FindWithTag("Player").GetComponent<Animator>();
        ghostAnimator = GameObject.FindWithTag("Phantom").GetComponent<Animator>(); // grab animator component on player 
        playerInputs = GameObject.FindWithTag("Player").GetComponent<InputReader>();
        phantomContainer = GameObject.FindWithTag("PhantomContainer"); // GetComponent<GameObject>();
       // phantomContainer = GameObject.FindWithTag("Phantom"); // GetComponent<GameObject>();
        targeter = GameObject.FindWithTag("Player").GetComponentInChildren<Targeter>();
        playerModifier = GameObject.FindWithTag("Player").GetComponent<CombatModifiers>();
        playerPos = GameObject.FindWithTag("Player").GetComponent<Transform>();
        SetAbilityName(this.ToString());
        //SetTapStatus(false);
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        phantomContainer = GameObject.FindWithTag("PhantomContainer"); // GetComponent<GameObject>();
        phantomContainer.transform.parent = playerPos.transform;
        phantomContainer.gameObject.SetActive(false);
    }

    public virtual void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public override string GetAbilityName()
    {
        return "PhantomAbilites";
    }

    protected void FaceTarget()
    {
        Vector3 offset = new Vector3(.15f, .15f, .15f);
        if (GetTargeter().currentTarget == null) { return; } // make sure we have a target
        Vector3 facing = (GetTargeter().currentTarget.transform.position - PhantomFighter.transform.position); // subtract target from our postion
        facing.y = 0;// dont care about height so we clear it
        float angle = Vector3.Dot(PhantomFighter.transform.position, GetTargeter().currentTarget.transform.position);
        
        if (angle >= 0.25f)
        {
            PhantomFighter.transform.rotation = Quaternion.LookRotation(facing); // creating a vector 3 to a quaternion
        }
    }

    //Can always make overloads for approach and face target methods if needed 
    protected void TurnPhantomOnandOff()
    {
        if (phantomContainer.gameObject != null)
        {
            if (ghostAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
            {
                phantomContainer.gameObject.SetActive(false);
                GetPropHandler().TurnOffAllProps(); // added specifically for combat mimic but may be refactored if issues arise from newer changes.
            }
            else
            {
                phantomContainer.gameObject.SetActive(true);
            }
        }
        else { return; }
    }

    protected void ApproachTarget()
    {
        Vector3 offset = new Vector3(1.1f, 0f, 1.1f);
        if (GetTargeter().currentTarget != null)
        {

            this.gameObject.transform.position = (GetTargeter().currentTarget.transform.position - new Vector3(1.0f, 0f, 0.0f)) ;
            this.gameObject.GetComponent<Rigidbody>().MoveRotation( GetTargeter().currentTarget.transform.rotation);
            /*            this.gameObject.gameObject.GetComponent<CharacterController>().transform.position = GetTargeter().currentTarget.transform.position - new Vector3(0f, 0f, 1.0f);
                        this.gameObject.gameObject.GetComponent<CharacterController>().transform.position = this.gameObject.gameObject.GetComponent<CharacterController>().transform.position; 
                       this.gameObject.gameObject.GetComponent<CharacterController>().enabled = false;*/
            // this.transform.position = Vector3.Lerp(this.transform.position, GetTargeter().currentTarget.transform.right, 0.01f * Time.deltaTime);
           // FaceTarget();
                return;
            
        }
       
        else
        {
         
            this.gameObject.transform.position = playerPos.transform.position + new Vector3(1.0f, 0f, 1.0f);
        /*    this.gameObject.gameObject.GetComponent<CharacterController>().transform.position = playerPos.transform.position;// Vector3.Lerp(PhantomFighter.transform.position, playerPos.transform.position + offset, 0.02f / Time.deltaTime);
            this.gameObject.gameObject.GetComponent<CharacterController>().transform.position = this.gameObject.gameObject.GetComponent<CharacterController>().transform.position;
            this.gameObject.gameObject.GetComponent<CharacterController>().enabled = true;*/
            return;
        }
    }

    protected void editTapstatus()
    {
        if(playerInputs.isRightSpecial == true)
        {
        //    Debug.Log("right special status is " + playerInputs.isRightSpecial);
            SetTap();
          //  Debug.Log("Tap status is " + GetTapStatus());
        }
    }
    protected Animator GetHostAnimator() => hostAnimator;
    protected Animator GetGhostAnimator() => ghostAnimator;

    protected RuntimeAnimatorController SetGhostAnimator(RuntimeAnimatorController animator) => ghostAnimator.runtimeAnimatorController = animator;

    protected RuntimeAnimatorController GetFighterController() => AbilityManager.Instance.GetFighterController(); 
    protected RuntimeAnimatorController GetGunslingerController() => AbilityManager.Instance.GetGunslingerController();
    protected RuntimeAnimatorController GetGreatSwordController() => AbilityManager.Instance.GetGreatSwordController();
    protected RuntimeAnimatorController GetAssassinController() => AbilityManager.Instance.GetAssassinController();
    
    protected RuntimeAnimatorController GetPhantomController() => AbilityManager.Instance.GetPhantomController();

    public string GetFighterControllerName() => AbilityManager.Instance.GetFighterController().name;
    public string GetGunControllerName() => AbilityManager.Instance.GetGunslingerController().name;
    public string GetGreatSwordControllerName() => AbilityManager.Instance.GetGreatSwordController().name;
    public string GetAssasinControllerName() => AbilityManager.Instance.GetAssassinController().name;

    protected Targeter GetTargeter() => targeter;

    protected CombatModifiers GetCombatModifier() => playerModifier;
    protected PropHandler GetPropHandler() => props;

    protected Transform GetPlayerPos() => playerPos;

    protected InputReader GetPlayerInputs() => playerInputs;

    protected GameObject GetPhantomContainer() => phantomContainer;

    protected void PreliminarySetup()
    {
        GetHostAnimator();
        GetGhostAnimator();
        GetPlayerInputs();
        GetPhantomContainer();
        GetTargeter();
    }
    protected void setupContainer()
    {
        GetPhantomContainer();
        GetPhantomContainer().transform.parent = GetPlayerPos().transform;
        GetPhantomContainer().gameObject.SetActive(false);
    }

    public override void UseAbility()
    {
       
    }

    public override void AddComponent()
    {
    }

    public override void RemoveComponent()
    {

    }



    /*
    public virtual void Awake()
    {
        hostAnimator = GameObject.FindWithTag("Player").GetComponent<Animator>();
        ghostAnimator = GameObject.FindWithTag("Phantom").GetComponent<Animator>();
        playerInputs = GameObject.FindWithTag("Player").GetComponent<InputReader>();
        phantomContainer = GameObject.FindWithTag("PhantomContainer").GetComponent<GameObject>();
        targeter = GameObject.FindWithTag("Player").GetComponent<Targeter>();
    }

    // Start is called before the first frame update
*/

}
