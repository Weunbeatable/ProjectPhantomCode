using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;

public class TestPhantomFighter : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Animators")]
    [SerializeField] public Animator hostAnimator;
    [SerializeField] public Animator ghostAnimator;


    [Header("Animtator Controllers")]
    //TODO: apply getters for animatiors
    [SerializeField] public RuntimeAnimatorController FightModeController;
    [SerializeField] public RuntimeAnimatorController GunModeController;
    [SerializeField] public RuntimeAnimatorController GreatSwordModeController;
    [SerializeField] public RuntimeAnimatorController AssasinModeController;

    [Header("Input Reader Reference")]
    public InputReader playerInputs;
    //public PlayerAttackingState playersATtacks;
    

    [Header("Anchor Points")]
    public GameObject playerPos;
    public GameObject phantomContainer;
    [SerializeField] public Targeter targeter;

    [Header("Prop Reference")]
    public PropHandler props;
    //TODO: apply getters for these public variables
    [SerializeField] public CombatModifiers playerModifier;
    [SerializeField] public RuntimeAnimatorController stolenAnimationController;

    [SerializeField] private ParticleSystem StealEffects;
   
    [SerializeField] private AudioSource PhantomSource;
    [SerializeField] private AudioClip phantomStealAudio;
    private readonly int play = Animator.StringToHash("PhantomSkillSteal");
    bool isStealActive;

    [Header("Data Containers")]
    public Queue<string> PhantomAttacks;
    public List<string>  playPhantomAttacks;

    public Queue<string> PhantomControllers;
    public List<string>  SwithchPhantomController;
    public Vector3 offset;

    private int ghostAttackHash;
    
    [Header("String reference to controllers")]
    // String refrence for animatiors, string REF ARE DANGEROUS
    public string fightController;
    public string gunController;
    public string greatSwordController;
    public string AssasinController;

    /* [Header("Temporary placeholderFor VFX")]
     public Effects triggerPhantom;*/
    private void Awake()
    {
        PhantomSource = GetComponent<AudioSource>();
        hostAnimator.GetComponent<Animator>();
        ghostAnimator.GetComponent<Animator>();
        playerInputs.GetComponent<InputReader>();
        phantomContainer.GetComponent<GameObject>();
        targeter.GetComponent<Targeter>();
    }
    void Start()
    {
        PhantomAttacks = new Queue<string>();
        playPhantomAttacks = new List<string>();
        PhantomControllers = new Queue<string>();
        SwithchPhantomController = new List<string>();

        PhantomControllers.Clear();
        PhantomAttacks.Clear(); 
        //triggerPhantom.GetComponent<Effects>();
        playPhantomAttacks.Clear();
      
        phantomContainer.transform.parent = playerPos.transform;
        phantomContainer.gameObject.SetActive(false);

        
        
        isStealActive = false;
    }

    private void OnEnable()
    {
        WeaponDamage.onParried += WeaponDamage_onParried;
    }

    private void WeaponDamage_onParried(object sender, System.EventArgs e)
    {
        // Find the dot product of the enemy and spawn behind him. Play an animation
        Vector3 directionBehind = targeter.currentTarget.transform.position;
        directionBehind.y += 0.6f;
        directionBehind.z -= 1f;

        
        this.transform.position = directionBehind; // Set character position to offset otherwise Phantom will be too close causing weird issues and collisons
        // will prevent the proper feel 

        this.transform.RotateAround(targeter.currentTarget.transform.position, Vector3.up, 90 * Time.deltaTime); // spin around 
        

        Vector3 forward = this.transform.TransformDirection(Vector3.forward); // Find the forward vector of phantom
        Vector3 toOther = targeter.currentTarget.transform.position - this.transform.position; // find the vector position of target - player

        
        if (Vector3.Dot(forward, toOther) < -0.98f) // I'm using the dot product here to find if i'm behind the enemy or not, I don't need an exact value.
            // doesn't have to be exactly -1 as that can be problematic in calculation so a ballpark figure will help to get the same results
        {
            print("I'm behind the enemy!");
            this.transform.position = toOther;//Vector3.Lerp(this.transform.position, directionBehind, 0.05f / Time.deltaTime);
            FaceTarget();
        }
        //Once behind the enemy trigger everything else so you can view the enemy
        phantomContainer.gameObject.SetActive(true);
        PhantomSource.PlayOneShot(phantomStealAudio);
        StealEffects.Play();
        
        isStealActive = true;
        ghostAnimator.CrossFadeInFixedTime(play, 0.05f);
        

        }

    private void OnDisable()
    {
        WeaponDamage.onParried -= WeaponDamage_onParried;
        isStealActive = false;
    }

    void Update()
    {
        TurnPhantomOnandOff();

        if (playerModifier.combatClipHash != 0)
        {
            ghostAttackHash = playerModifier.combatClipHash;

        }
        if (playerInputs.isRightSpecial)
        {
            if (ghostAttackHash != 0)
            {

                ghostAttackHash = playerModifier.combatClipHash;
            }
            ghostAnimator.runtimeAnimatorController = playerModifier.stolenAnimationController;
            if (targeter.transform.position != null)
            {
                ApproachTarget();
                // phantomContainer.gameObject.SetActive(true);

            }
            ghostAnimator.Play(ghostAttackHash);

        }

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
            phantomContainer.transform.parent = null;


        }
        if (playerInputs.isLeftSpecial)
        {


            if (PhantomAttacks.Count > 0)
            {

                //  triggerPhantom.PlayPhantomCastingTimeEffect();
                ApproachTarget();
                // phantomContainer.gameObject.SetActive(true);

                //CurrentAnimatorControllerManagment(); // MEWLY ADDED LINE

                if (ghostAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
                {
                    CurrentAnimatorControllerManagment(); // first check the state of the controller, make a change if necessary
                    ghostAnimator.Play(PhantomAttacks.Dequeue()); // play attack
                    PhantomControllers.Dequeue(); // remove item from controller that we peeked at
                    //phantomContainer.transform.parent = playerPos.transform; // phantom positioning
                }
                //
            }
        }

        // Code has been reduced from 200 lines to 69 :D
    }

    private void TurnPhantomOnandOff()
    {
        if (ghostAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
        {
            phantomContainer.gameObject.SetActive(false);
        }
        else
        {
            phantomContainer.gameObject.SetActive(true);
        }
    }

    private void ApproachTarget()
    {
        Vector3 offset = new Vector3(0.7f, 0f, 0.8f);
        if (targeter.currentTarget != null)
        {
            FaceTarget();
            this.transform.position = Vector3.Lerp(this.transform.position, targeter.currentTarget.transform.position - offset, 0.1f/Time.deltaTime);
            
        }
        else
        {
            this.transform.position = Vector3.Lerp(this.transform.position, playerPos.transform.position + offset, 0.1f);
        }
    }

    private void CurrentAnimatorControllerManagment()
    {
        
        string checkNextController = PhantomControllers.Peek();
            if (ghostAnimator.runtimeAnimatorController.name != checkNextController)
            {
                    if (checkNextController == fightController)
                    {
                     props.FightingProps();
                     ghostAnimator.runtimeAnimatorController = FightModeController;
                        
                    }
                    else if (checkNextController == gunController)
                    {
                     props.ShootingProps();
                     ghostAnimator.runtimeAnimatorController = GunModeController;
                        
                    }
                    else if (checkNextController == greatSwordController)
                    {
                     props.GreatSwordsmanProps();
                     ghostAnimator.runtimeAnimatorController = GreatSwordModeController;
                        

                    }
                    else if (checkNextController == AssasinController)
                    {
                     props.AssasinationProps();
                     ghostAnimator.runtimeAnimatorController = AssasinModeController;
                        

                    }
                
                
                // peek the next controller if the next controller name is different switch animation controllers before returning to top 
                // of loop
            }
    }
    // to prevent a duplicate need to make in base state a getFacingTarget that returns a vector 3 and just call for that instead 
    protected void FaceTarget()
    {
        Vector3 offset = new Vector3(.15f, .15f, .15f);
        if (targeter.currentTarget == null) { return; } // make sure we have a target
        Vector3 facing = (targeter.currentTarget.transform.position - this.transform.position); // subtract target from our postion
        facing.y = 0;// dont care about height so we clear it
        float angle = Vector3.Dot(this.transform.position, targeter.currentTarget.transform.position);
        float distance = Vector3.Distance(this.transform.position, targeter.currentTarget.transform.position);
        if (angle >= 0.3f)
        {
            this.transform.rotation = Quaternion.LookRotation(facing); // creating a vector 3 to a quaternion
        }
    }
}
