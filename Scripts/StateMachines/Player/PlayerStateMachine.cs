using PFF.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    //field before serailzed will turn it into a field then serilize it for us
    [field: SerializeField] public InputReader InputReader { get; private set; } // Rules are public getting reader and privately setting it
    [field: SerializeField] public CharacterController characterController { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public RuntimeAnimatorController FightController { get; private set; }
    [field: SerializeField] public RuntimeAnimatorController GunController { get; private set; }
    [field: SerializeField] public RuntimeAnimatorController GreatSwordController { get; private set; }
    [field: SerializeField] public RuntimeAnimatorController AssasinController { get; private set; }
    [field: SerializeField] public Targeter targeter { get; private set; }
    [field: SerializeField] public ForceReceiver forceReceiver { get; private set; }
    [field: SerializeField] public Parkour parkour { get; private set; }
    [field: SerializeField] public Attack[] Attacks { get; private set; }
    [field: SerializeField] public Attack[] HeavyAttacks { get; private set; }
    [field: SerializeField] public Attack[] BasicHoldAttack { get; private set; }
    [field: SerializeField] public Attack[] HeavyHoldAttack { get; private set; }
    [field: SerializeField] public Attack[] LightAirAttacks { get; private set; }
    [field: SerializeField] public Attack[] HeavyAirAttacks { get; private set; }
    // Fields specific to stance attacks. 
    [field: SerializeField] public Health health { get; private set; }
    [field: SerializeField] public WeaponDamage Weapon { get; private set; } // made more of these fields for the other body parts
    [field: SerializeField] public WeaponDamage kunaiDamage { get; private set; }
    [field: SerializeField] public WeaponDamage greatSwordDamage { get; private set; }
    [field: SerializeField] public WeaponDamage headDamage { get; private set; }
    [field: SerializeField] public WeaponDamage rightHandDamage { get; private set; }
    [field: SerializeField] public WeaponDamage leftHandDamage { get; private set; }
    [field: SerializeField] public WeaponDamage rightLegDamage { get; private set; }
    [field: SerializeField] public WeaponDamage leftLegDamage { get; private set; }
    [field: SerializeField] public Ragdoll ragdoll { get; private set; }
    [field: SerializeField] public LedgeDetector ledgeDetector { get; private set; }
    [field: SerializeField] public Transform orientation { get; private set; }
    [field: SerializeField] public VFXEffects effects { get; private set; }
    [field: SerializeField] public SoundEffects sfx { get; set; }
    [field: SerializeField] public MeshTrailVisual meshTrailVisual { get; set; }
    [field: SerializeField] public TestPhantomFighter fighter { get; private set; }
    [field: SerializeField] public RadialMenu radialMenu { get; private set; }
    [field: SerializeField] public PropHandler props { get; private set; }
    [field: SerializeField] public PlayerCombatTimers combatTimers { get; private set; }
    [field: SerializeField] public CombatModifiers combatModifiers { get; private set; }
    [field: SerializeField] public Finisher playerFinisher { get; private set; }
    [field: SerializeField] public StanceManager stanceManager { get; private set; }
    [field: SerializeField] public PlayerEffects playerEffects { get; private set; }

    [field: SerializeField] public Collider playerCollider { get; private set; }

    [field: SerializeField] public GameObject[] dodgeObjects { get;  set; } = new GameObject[4];
    [field: SerializeField] public WeaponSO[] weaponSOs { get; set; }


    //
    // Grounded & Jump Variables
    [field: Header("Ground & Jump mechanics")]
    [field: SerializeField] public float FreeLookMovementSpeed { get; set; } // allowed this variable to be changed through out project so that momentum is maintained
    public float MonitoredCharacterVelocity { get; set; }
    [field: SerializeField] public float TargetingMovementSpeed { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; } // Damping is essentially smoothing
    [field: SerializeField] public float DodgeDuration { get; private set; } // Damping is essentially smoothing
    [field: SerializeField] public float DodgeLength { get; private set; } // Damping is essentially smoothing
    [field: SerializeField] public float JumpForce { get; private set; } // Damping is essentially smoothing
    [field: SerializeField] public float JumpingInputSensitivity { get; private set; } = 2.0f;  //Adjust to taste
    [field: SerializeField] public float clippingDistance { get; private set; } = 0.5f;  //Adjust to taste


    [field: Header("Parkour Params!")]
    [field: SerializeField] public float parkourVaultDistance { get; private set; } = 2.0f;  //Adjust to taste
    // Wall Running Variables 
    [field: Header("Wall Running variables")]
    [field: SerializeField] public float wallRunForce { get; private set; }
    [field: SerializeField] public float MaxWallRunTime { get; private set; }
    [field: SerializeField] public float WallRunTimer { get; private set; }
    [field: SerializeField] public float wallRunSpeed { get; private set; }
    [field: SerializeField] public float wallJumpUpForce { get; private set; }
    [field: SerializeField] public float wallJumpSideForce { get; private set; }
    [field: SerializeField] public float exitWallTime { get; private set; }
    [field: SerializeField] public float exitWallTimer { get; set; }
    [field: SerializeField] public float wallClimbSpeed { get; set; }

    //Dashing variables
    [field: Header("Dashing variables")]
    [field: SerializeField] public float dashForce { get; private set; }
    [field: SerializeField] public float dashUpwardForce { get; private set; }
    [field: SerializeField] public float dashDuration { get; private set; }
    [field: SerializeField] public float dashCoolDown { get; private set; }
    [field: SerializeField] public float dashCoolDownTimer { get; set; }


    [field: Header("StanceSpecificData")]
    [field: SerializeField] public float AddedKnockbackValue { get; set; }
    public List<WeaponDamage> updatestate = new List<WeaponDamage>();
    [field: SerializeField] public float boundaryDistance { get; set; }

    [field: Header("Collision Checks")]

    public LayerMask CheckIfIsWall;

    public LayerMask CheckIfIsGround;

    public int airJumpCounter = 0;

    public float wallCheckDistance;

    public float minJumpHeight;

    public float wallEjectForce;

    public RaycastHit leftWalllhit;

    public RaycastHit rightWallHit;

    public bool wallLeft;

    public bool wallRight;

    public bool exitingWall;

    public bool isStartingComboCounter;

    public bool isAirJumpExhausted;

    public bool isDashing;

    public bool isRunning;

    private bool alreadyAppliedForce;
    public int currentCombatIndexValue { get; set; }

    public Vector3 lastKnownPost;
    public Quaternion lastKnownRotation;
    public string currentActiveStance { get; set; }
    public Transform MainCameraTransform { get; private set; }
    public float previousDodgeTime { get; private set; } = Mathf.NegativeInfinity; // we need this to take into account current itme, reason being to prevent double dodging after first dodge so we keep uniform dodge time all around
    public Vector3 ClosestImpactPoint { get; set; }

    [field: Header("Test Varables To be Privatized")]
    [field: SerializeField] public bool isLastHit { get; set; }
    //TO DO: BIG OVERHAUL OF THIS CURRENT SYSTEM
    // This needs to be pulled into its own class so I can set specific values for class changes
    public readonly int FighterReadyAnim = Animator.StringToHash("Equip");
    public readonly int GunReadyAnim = Animator.StringToHash("Gun_Equip");
    public readonly int AssasinReadyAnim = Animator.StringToHash("Assasin_Equip");
    public readonly int GreatSwordReadyAnim = Animator.StringToHash("GreatSword_Equip");
    private const float CrossFadeDuration = 0.1f;
    private string previousStance { set; get; }
    public event Action UsingAssassinStance, UsingFighterStance, UsingGunStance, UsingHeavySwordStance;
    [SerializeField] public List<Action> actionList = new List<Action>();
    // TODO: Implement Taunt - should be able to transition into  jump, walk and run states; 

    private void Awake()
    {
        actionList.Add(FighterMode);
        actionList.Add(GunMode);
        actionList.Add(GreatSwordMode);
        actionList.Add(AssasinMode);
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        MainCameraTransform = Camera.main.transform;

        SwitchState(new PlayerFreeLookState(this));
        fighter.GetComponent<TestPhantomFighter>();
        previousStance = Animator.runtimeAnimatorController.name;
        currentActiveStance = Animator.runtimeAnimatorController.name;
    }

    private void OnEnable()
    {
        health.onTakeDamage += HandleTakeDamage;
        MeshTrailVisual.onAllowRewind += MeshTrailVisual_onAllowRewind;
        PhantomStealAbility.SpecialBlockTrigger += PhantomStealAbility_SpecialBlockTrigger;
        health.onDie += HandleDeath;
        ShootRayFromCharacterControllerToDetermineMovement();
    }

    private void PhantomStealAbility_SpecialBlockTrigger()
    {
        SwitchState(new PlayerSpecialBlockingState(this, true));
    }

    private void MeshTrailVisual_onAllowRewind()
    {
        lastKnownPost = characterController.transform.position;
        lastKnownRotation = characterController.transform.rotation;
    }

    private void FixedUpdate()
    {
       /* foreach (WeaponDamage point in updatestate)
        {
            if (point.GetClosestPoint() != null)
            {
                ClosestImpactPoint = point.GetClosestPoint();
                return;
            }
        }*/
    }

    //TODO
    //Move all the stance mode systems into a class/Struct/Method structure
    //for easy calling, this current model is messy, hard to manage and is quite repetetive.
    public void AssasinMode()
    {
        State storedState = GetCurrentState();
        previousStance = Animator.runtimeAnimatorController.name;
        Debug.Log("Current running controller is " + previousStance);
        UsingAssassinStance?.Invoke();
        effects.assasinDustParticle.Play();
        props.AssasinationProps();
        Animator.runtimeAnimatorController = AssasinController;
        currentActiveStance = Animator.runtimeAnimatorController.name;
        Debug.Log("Current running controller is " + currentActiveStance);
        // Animator.CrossFadeInFixedTime(AssasinReadyAnim, CrossFadeDuration);
        sfx.playerSource.PlayOneShot(sfx.assasinmode);
        // SwitchState(new PlayerFreeLookState(this));
        //check for combat, go to defined combat state otherwise get currentstate. 

        CheckforCombatOrFreeState();

    }

    public void GreatSwordMode()
    {
        State storedState = GetCurrentState();
        previousStance = Animator.runtimeAnimatorController.name;
        Debug.Log("Current running controller is " + currentActiveStance);
        props.GreatSwordsmanProps();
        UsingHeavySwordStance?.Invoke();
        effects.greatSwordParticle.Play();

        Animator.runtimeAnimatorController = GreatSwordController;
        currentActiveStance = Animator.runtimeAnimatorController.name;
        Debug.Log("Current running controller is " + currentActiveStance);
        //  Animator.CrossFadeInFixedTime(GreatSwordReadyAnim, CrossFadeDuration);
        sfx.playerSource.PlayOneShot(sfx.greatSwordmode);
        CheckforCombatOrFreeState();
    }
    public void GunMode()
    {
        State storedState = GetCurrentState();
        previousStance = Animator.runtimeAnimatorController.name;
        Debug.Log("Current running controller is " + currentActiveStance);
        props.ShootingProps();
        UsingGunStance?.Invoke();
        effects.gunModeDustParticle.Play();

        Animator.runtimeAnimatorController = GunController;
        currentActiveStance = Animator.runtimeAnimatorController.name;
        Debug.Log("Current running controller is " + currentActiveStance);
        // Animator.CrossFadeInFixedTime(GunReadyAnim, CrossFadeDuration);
        sfx.playerSource.PlayOneShot(sfx.gunmode);
        //SwitchState(new PlayerFreeLookState(this));
        CheckforCombatOrFreeState();

    }

    // Since functionally they all run the same, I can convert the basic structure of this into a function call made within Weapon SO
    // This will help to alleviate the need for repetitious code. 
    // and more efficency when making function calls. 
    // There will instead be a generic function name like mode switch, Which will make all the necessary calls on the SO. 
    // This will require a modification to the stance manager in terms of what is being called and how weapons are currently set in inventory on the player.
    //  That will require an overhaul of the system but should ultimately fix a log of problems

    // preconditions: have alraedy been met with creation of stance system and weaponSO
    // postcondition: that weapon swithching functions as is already setup and new weapons can easily be added on with all their necessary information
    // side effects: changes to logic of stance system and modifications will most likely need to be made for the delegate functions I'm using
    // opens other avenues such as tying combo index to weapons allowing for different weapons to give different lenghts of attack if they level up (much like Nier Automatas combat system). 
    // While fists are exclusively part of a fighting archetype, kicks can be found in every archetype, they will be tied to the base stats of the player to prevent awkward scaling but will not be tied to SO's as there is no need.
    // Save for a possible change in VFX on use
    // other side effects still yet to be found. 

    public void FighterMode()
    {
        State storedState = GetCurrentState();
        previousStance = Animator.runtimeAnimatorController.name;
        Debug.Log("Current running controller is " + currentActiveStance);
        props.FightingProps();
        UsingFighterStance?.Invoke();
        effects.fightModeDustParticle.Play();

        Animator.runtimeAnimatorController = FightController;
        currentActiveStance = Animator.runtimeAnimatorController.name;
        Debug.Log("Current running controller is " + currentActiveStance);
        //Animator.CrossFadeInFixedTime(FighterReadyAnim, CrossFadeDuration);
        sfx.playerSource.PlayOneShot(sfx.fighterMode);
        CheckforCombatOrFreeState();
    }

    private void OnDisable()
    {
        health.onTakeDamage -= HandleTakeDamage;
        health.onDie -= HandleDeath;
        MeshTrailVisual.onAllowRewind -= MeshTrailVisual_onAllowRewind;
        PhantomStealAbility.SpecialBlockTrigger -= PhantomStealAbility_SpecialBlockTrigger;
    }

    private void HandleTakeDamage()
    {
        if(Time.timeScale < 1f) // if for whatever reason while taking damage there is some slow motion going on, cancel it to avoid unexpected behavior. 
            Time.timeScale = 1f;
        SwitchState(new PlayerImpactState(this));
    }

    private void HandleDeath()
    {
        SwitchState(new PlayerDeadState(this));
    }
    private Vector3 Eject = new Vector3(0f, 40f, -220f);
    public void CalltoEject()
    {

        forceReceiver.WallJumpForce(Eject, ForceMode.Impulse);
    }

    public void SetUpAttacks(Attack attack)
    {
        /*  Weapon.knockback = combatModifiers.modifiedKnockBack;
          headDamage.knockback = combatModifiers.modifiedKnockBack;
          rightHandDamage.knockback = combatModifiers.modifiedKnockBack;
          leftHandDamage.knockback = combatModifiers.modifiedKnockBack;
          rightLegDamage.knockback = combatModifiers.modifiedKnockBack;
          leftLegDamage.knockback = combatModifiers.modifiedKnockBack;
          Weapon.SetAttack(attack.Damage, Weapon.knockback );*/// replace with appropriate weapon knockback, this will replace combat modifier KB
        headDamage.SetAttack(attack.Damage, headDamage.knockback);
        rightHandDamage.SetAttack(attack.Damage, rightHandDamage.knockback);
        leftHandDamage.SetAttack(attack.Damage, leftHandDamage.knockback);
        rightLegDamage.SetAttack(attack.Damage, rightLegDamage.knockback);
        leftLegDamage.SetAttack(attack.Damage, leftLegDamage.knockback);
        kunaiDamage.SetAttack(attack.Damage, kunaiDamage.knockback);
        greatSwordDamage.SetAttack(attack.Damage, greatSwordDamage.knockback);

        foreach (WeaponSO weaponSO in weaponSOs)
        {

        }
        //greatSwordDamage.SetAttack(attack.Damage, attack.Knockback + combatModifiers.modifiedKnockBack);
        // Check for a SO != null
        // to accomodate dual weidling itterate through through all SO by running a for loop. 

    }
    public void UpdateHitState()
    {
        foreach (WeaponDamage weapon in updatestate)
        {
            weapon.triggeredHitStateData = combatModifiers.updatedHitStatus;
        }
    }

    private void ShootRayFromCharacterControllerToDetermineMovement()
    {
        RaycastHit hit;
        int layerMask = 1 << 8;
        Vector3 Offset = new Vector3(-.8f, 0f, -.8f);
        if (Physics.Raycast(characterController.transform.position, characterController.transform.TransformDirection(Vector3.up), out hit, 5f, layerMask))
        {
            Debug.DrawRay(characterController.transform.position, characterController.transform.TransformDirection(Vector3.up) * hit.distance, Color.yellow);
            Debug.Log("did hit");
            transform.position += Offset;
        }
    }

    private void CheckforCombatOrFreeState()
    {
        // pass array index as parameter for a given state change. this way its easy to monitor the index 
        // then make a new constructor for the attacking states. 
        if (GetCurrentState() == new PlayerAttackingState(this, currentCombatIndexValue))
        {
            SwitchState(new PlayerAttackingState(this, currentCombatIndexValue));
        }
        else if (GetCurrentState() == new PlayerHeavyAttackingState(this, currentCombatIndexValue))
        {
            SwitchState(new PlayerHeavyAttackingState(this, currentCombatIndexValue));
        }
        else if (GetCurrentState() == new PlayerHoldBasicAttack(this, currentCombatIndexValue))
        {
            SwitchState(new PlayerHoldBasicAttack(this, currentCombatIndexValue));
        }
        else if (GetCurrentState() != new PlayerFreeLookState(this))
        {
            SwitchState(new PlayerFreeLookState(this));
           
        }
        else
        {
            SwitchState(GetCurrentState());
        }
    }
}
