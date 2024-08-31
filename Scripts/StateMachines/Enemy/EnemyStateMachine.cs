using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : StateMachine
{
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public float PlayerSuspicionRange { get; private set; }
    [field: SerializeField] public float PlayerChasingRange { get; private set; }
    [field: SerializeField] public float MovementSpeed { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public int AttackDamage { get; private set; }
    [field: SerializeField] public int Attackknockback { get; private set; }
    [field: SerializeField] public float impactHitstun { get; private set; }
    [field: SerializeField] public Health health { get; private set; }
    [field: SerializeField] public WeaponDamage Weapon { get; private set; } // made more of these fields for the other body parts
    [field: SerializeField] public WeaponDamage headDamage { get; private set; }
    [field: SerializeField] public WeaponDamage rightHandDamage { get; private set; }
    [field: SerializeField] public WeaponDamage leftHandDamage { get; private set; }
    [field: SerializeField] public WeaponDamage rightLegDamage { get; private set; }
    [field: SerializeField] public WeaponDamage leftLegDamage { get; private set; }
    [field: SerializeField] public EnemyAttack enemyAttack { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public ForceReceiver forceReceiver  { get; private set; }
    [field: SerializeField] public NavMeshAgent navMesh { get; private set; }
    [field: SerializeField] public Target target { get; private set; }
    [field: SerializeField] public Ragdoll ragdoll { get; private set; }
    [field: SerializeField] public EnemyPlayerResponse playerResponse { get;  set; }

    [field: SerializeField] public EnemyAttackManager attackManager { get; set; }

    [field: SerializeField] public CharacterController characterController { get; set; }
    [field: SerializeField] public WeaponHandler handler { get; set; }

    [field: SerializeField] public GameObject boundary { get; set; }

    //TODO: Create a patrol & alert State
    public Health player { get; private set; }
    public Hashtable hitStates;
   /* public hashm
    public Hashtable<string, State> hitStates;*/
    [field: SerializeField] public string hitReaction { get; set; }
    public bool isTargetOfFinish { get; set; }

    public string weaponType { get; set; }

    public bool isInAttackingRange { get; set; }

    private float initial_offset;
    private void Awake()
    {
        initial_offset = characterController.stepOffset;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
       

        navMesh.updatePosition = false;
        navMesh.updateRotation = false;
        isTargetOfFinish = false;
        SwitchState(new EnemyIdleState(this));
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<CharacterController>(out CharacterController controller))
        {
            if (characterController.bounds.min == controller.bounds.max)
            {
                characterController.detectCollisions = false;
            }
            else
            {
                characterController.detectCollisions = true;
            }
        }
        
    }


    private void OnEnable()
    {
        if (!characterController.isGrounded)
        {
            characterController.stepOffset = 0;
            if(playerResponse.isStandingOnPlayer() == true)
            {
                Debug.Log("We are getting somewhere");
                if(playerResponse.GetPlayer() != null)
                {
                    Debug.Log("MOVE MY MAN MOVE");
                    Vector3 direction = (playerResponse.GetPlayer().transform.position - transform.position).normalized;
                    direction.y = 0;
                    characterController.Move(direction * 3 * Time.deltaTime);
                }
                
            }
        }
        
        //health.onFinishable += health_onFinishable;
        health.onTakeDamage += HandleTakeDamage;
        health.onDie += HandleDeath;
        WeaponDamage.onParried += WeaponDamage_onParried;
        PlayerFinisherState.onPassFinisherStatedata += PlayerFinisherState_onPassFinisherStatedata;
        EnemyPlayerResponse.OnAbovePlayer += EnemyPlayerResponse_OnAbovePlayer;
    }

    private void EnemyPlayerResponse_OnAbovePlayer(object sender, bool e)
    {
       
            //characterController.stepOffset = 0;
            if(e == true)
            {
                Debug.Log("MOVE MY MAN MOVE");
            //  Vector3 direction = (playerResponse.GetPlayer().transform.position - transform.position).normalized;
            // direction.y = 0;
            Vector3 enemy = transform.position;
            enemy.y = 0;
                characterController.Move(enemy * 0.5f * Time.deltaTime);
            }
         /*   if (playerResponse.isStandingOnPlayer() == true)
            {
                Debug.Log("We are getting somewhere");
                if (playerResponse.GetPlayer() != null)
                {
                    Debug.Log("MOVE MY MAN MOVE");
                    Vector3 direction = (playerResponse.GetPlayer().transform.position - transform.position).normalized;
                    direction.y = 0;
                    characterController.Move(direction * 3 * Time.deltaTime);
                }

            }*/
        

    }

    private void OnDisable()
    {
        if (characterController.isGrounded)
        {
            characterController.stepOffset = initial_offset;
        }

       // health.onFinishable -= health_onFinishable;
        health.onTakeDamage -= HandleTakeDamage;
        health.onDie -= HandleDeath;
        WeaponDamage.onParried -= WeaponDamage_onParried;
        PlayerFinisherState.onPassFinisherStatedata -= PlayerFinisherState_onPassFinisherStatedata;
    }

    private void PlayerFinisherState_onPassFinisherStatedata(string obj)
    {
        string datTopass;
        datTopass = obj;
        if(isTargetOfFinish == true)
        SwitchState(new EnemyFinishedState(this, datTopass));
        else { return; }
    }

    private void WeaponDamage_onParried(object sender, EventArgs e)
    {
        SwitchState(new EnemyParriedState(this));
    }

    private void HandleTakeDamage()
    {
        // Test: to prevent null path error only allow loading states if enemy is vulnerable
        if(health.GetVulnerabilityStatus() == false)
        {
            LoadStates();
        }
        
       /* if(hitReaction == "stun")
        {
            SwitchState(new StunState(this));
        }
        else
        SwitchState(new EnemyImpactState(this));*/
    }
    private void health_onFinishable(bool obj)
    {
        if (obj == true)
        {
            if (player == null) { return; } // make sure we have a target
            else
            {
                
                Vector3 facing = (player.transform.position - transform.position); // subtract target from our postion
                facing.y = 0;// dont care about height so we clear it

                transform.rotation = Quaternion.LookRotation(facing); // creating a vector 3 to a quaternion
            }
        }
    }
    private void HandleDeath()
    {
        SwitchState(new EnemyDeadState(this));
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, PlayerSuspicionRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, PlayerChasingRange);
        Gizmos.DrawFrustum(transform.position, initial_offset, 20, 4, 2);
    }

    public void LoadStates()
    {
        // hitStates.Add("stun", SwitchState(new EnemyImpactState(this));
        // add heavy stun state, may need check for if hit by weapon or some other opening condition
        string hitState = hitReaction;
        switch (hitReaction)
        {
            case "stun":
                SwitchState(new StunState(this));
                break;
            case "stagger":
                SwitchState(new StaggerState(this));
                break;
            case "flyback":
                SwitchState(new FlyBackState(this));
                break;
            case "launcher":
                SwitchState(new PopUpStartState(this));
                break;
            case "dizzy":
                SwitchState(new DizzyState(this));
                break;
            case "knockdown":
                SwitchState(new KnockDownState(this));
                break;

            default:
                SwitchState(new EnemyImpactState(this));
                break;
        }
        // Outputs "Thursday" (day 4)

    }

    public bool GetFinishableState() => isTargetOfFinish;

}

