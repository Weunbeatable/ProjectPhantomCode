using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    //could be problematic for projectiles may need to rework
    public static event EventHandler onParried;
    
    bool wasParried;

    [SerializeField] private Collider myCollider;
    [SerializeField] private int damage;
    [SerializeField] public float knockback;
    public bool lastHitOmph;
    private bool isCountered;
    [field: SerializeField] public string triggeredHitStateData {  get;  set; }
    [field: SerializeField] public string weaponsTag { get; set; }
    public WeaponHandler weaponProperties;
    private Vector3 retrieveClosestPoint { get; set; }

    private List<Collider> alreadyCollidedWith = new List<Collider>();
    private void Awake()
    {
        weaponsTag = this.gameObject.tag;
    }
    private void OnEnable() // whenever script is enabled
    {
        alreadyCollidedWith.Clear();
      //  PlayerBlockingState.onParrying += PlayerBlockingState_onParrying; // MAKE SURE THIS DOESNT PERMANENTLY set damage to 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        CollisionImpactEffect(other);
        cameraShake();
        CheckforParry(other);
        
        DamageAnimationInfo(other);
        if (other == myCollider) { return; }

        //  if statement and already collided should be taken into consideration. having them off allows for multi hit but has side effects requirinng  refactoring and abstraction

        if (alreadyCollidedWith.Contains(other)) { return; }

         alreadyCollidedWith.Add(other);
        //Debug.Log("THe weapon type is  " + weaponsTag);
       

        DamageDealingInfo(other);
        PlayTimeStop(other);
        CollisionSoundEffect(other);

        KnockbackAdjustment(other);
    }
    

    private void OnTriggerExit(Collider other)
    {
        
    }
    private void OnDisable()
    {
      //  PlayerBlockingState.onParrying -= PlayerBlockingState_onParrying;
    }

    private void DamageAnimationInfo(Collider other)
    {
        if(other.TryGetComponent<EnemyStateMachine>(out EnemyStateMachine weaponInfo))
        {
            weaponInfo.weaponType = weaponsTag;
        }
        if(other.TryGetComponent<EnemyStateMachine>(out EnemyStateMachine reactState))
        {
            reactState.hitReaction = triggeredHitStateData;        
        }
    }
    private void DamageDealingInfo(Collider other)
    {
        if (other.TryGetComponent<Health>(out Health health))// checking if other component has health component do damage
        {
            health.DealDamage(damage);
            //create a list of things hit so we don't hit the same thing twice on an  attack
        }
    }

    //TODO on taking damage in the air knockback must either halved or fully negated to allow for phantom and follow up combos to have any meaning with the exception of attacks with the knockback property/ signature
    private void KnockbackAdjustment(Collider other)
    {
        if (other.TryGetComponent<ForceReceiver>(out ForceReceiver receiver))
        {
            Vector3 direction = (other.transform.position - myCollider.transform.position).normalized;
            Vector3 Updirection = (other.transform.position - myCollider.transform.forward).normalized;
            
            if (this.gameObject.tag == "Player")
            {
                //TODO if statement here to check trigger/flag on animation event to see if attack launches or not for vertical knockback

                receiver.AddForce(direction * knockback /*+ weaponProperties.SetKnockback())*/);
                //   Debug.Log("current sitting value is " + weaponProperties.AddedKnockbackValue);
            }
            else
            {
                // Add knockback angles for flyback and inverse knockback for wall bounce and gorund bounce params 

                if (this.triggeredHitStateData == "launcher")
                {
                   // Updirection.x = 0f;
                    receiver.AddVerticalForce(Updirection);
                }
                else if (this.triggeredHitStateData == "flyback")
                {
                    receiver.AddFlybackForce(Updirection / 1.2f);
                }
                else if (this.triggeredHitStateData == "knockdown")
                {
                    if (other.TryGetComponent<CharacterController>(out CharacterController controller))
                    {
                        if(controller.isGrounded == true)
                        {
                            receiver.AddKnockdownForce(Updirection / 2f);
                        }
                        else
                        {
                            receiver.AddKnockdownForce(Updirection);
                        }
                    }
                   
                }
                //TODO CODE FOR KNOCKDOWN, force should be negative if in the air (probably above a certain distance
                // to avoid issues with how finicky the character controller can be) 
                else
                {
                    direction.y = 0f; // test to see if it removes weird issues of floating
                    receiver.AddForce(direction * SetKnockback(knockback)); // only care about direction hence why we normalize the force we add
                }



            }
        }
    }

  
    private void PlayTimeStop(Collider other)
    {

        if (other.TryGetComponent<TimeStop>(out TimeStop stoppage))
        {

            if(this.gameObject.tag == "Melee")
            {
                stoppage.StopTime(0.08f, 60, 0.05f);
            }
            else if(this.gameObject.tag == "Weapon")
            {
                stoppage.StopTime(0.04f, 35, 0.1f);
            }
            
        }
    }

    private void CollisionImpactEffect(Collider other)
    {
        var collisionPoint = other.ClosestPoint(this.transform.position);
        retrieveClosestPoint = collisionPoint;
        if (other.TryGetComponent<ImpactEffects>(out ImpactEffects impact))
        {
            if (this.gameObject.tag != null)
            {
                if (this.gameObject.tag == "Melee")
                {
                    impact.meleeImpact.GetComponent<ParticleSystem>().transform.position = collisionPoint;
                    impact.meleeImpact.GetComponent<ParticleSystem>().Play();
                }
                else if (this.gameObject.tag == "Weapon")
                {
                    impact.weaponImpact.GetComponent<ParticleSystem>().transform.position = collisionPoint;
                    impact.weaponImpact.GetComponent<ParticleSystem>().Play();
                }
            }


        }
    } // should be a vfx script where you subscribe to the event of hit collision, takes weapon damage data and string argument

    private void CollisionSoundEffect(Collider other) // should be Sfx script where you subscribe to the event of hit collision event takes weapon damage data and string argument
    {
        if(other.TryGetComponent<ImpactSounds>(out ImpactSounds impactSounds))
            {
            if(this.gameObject.tag != null)
            {
                if (this.gameObject.tag == "Melee")
                {

                    impactSounds.GetComponent<ImpactSounds>().PlayMeleeAAudio();
                }
                else if (this.gameObject.tag == "Weapon")
                {

                    impactSounds.GetComponent<ImpactSounds>().PlayWeaponAudio();
                }
            }
        }
    }

    public void SetAttack(int damage, float Knockback) // later add knockcback
    {
        this.damage = damage;
        this.knockback = Knockback;
    }

    public void cameraShake()
    {
        if (this.gameObject.tag == "Melee")
        {
            CameraShake.Instance.ShakeCamera(0.5f, .1f);
        }
        if (this.gameObject.tag == "Weapon")
        {
            CameraShake.Instance.ShakeCamera(1f, .1f);
        }

    }

    public float SetKnockback(float Knockback)
    {
        Knockback = this.knockback;
        return Knockback;
    }
 
    private void CheckforParry(Collider other)
    {
       // alreadyCollidedWith.Add(other);
        if (other.gameObject.tag == "Player")
        {
           // alreadyCollidedWith.Add(other);
            if (other.TryGetComponent<PlayerCombatTimers>(out PlayerCombatTimers playerCombatTimers))
            {
                isCountered = playerCombatTimers.GetParryState();
                if (isCountered == true)
                {
                    knockback = 0;
                    alreadyCollidedWith.Add(other);
                    Debug.Log("I've been parried");
                    Debug.Log(myCollider.gameObject.name); // TEST THIS WHEN YOU GET HOME!!!
                    if(PhantomCombatMimicAbility.istapped == true)
                    {
                        if (other.TryGetComponent<CombatModifiers>(out CombatModifiers combatModifiers))
                        {

                            if (combatModifiers.stolenAnimationController == null)
                            {
                                // assigning the appropriate animator to the ghost animator and grabbing animation name data. 
                                combatModifiers.stolenAnimationController = myCollider.gameObject.GetComponent<Animator>().runtimeAnimatorController;
                                System.Random rnd = new System.Random();
                                int randIndex = rnd.Next(myCollider.gameObject.GetComponent<EnemyAttackManager>().animationNamesHash.Count);
                                if (myCollider.gameObject.GetComponent<EnemyAttackManager>().animationNamesHash.Count > 0)
                                {
                                    combatModifiers.combatClipHash = myCollider.gameObject.GetComponent<EnemyAttackManager>().animationNamesHash[randIndex];
                                }


                            }
                            if (combatModifiers.CombatClip == null)
                            {
                                System.Random rnd = new System.Random();
                                int randIndex = rnd.Next(myCollider.gameObject.GetComponent<EnemyAttackManager>().animationClips.Count);
                                if (myCollider.gameObject.GetComponent<EnemyAttackManager>().animationClips.Count > 0)
                                {
                                    combatModifiers.CombatClip = myCollider.gameObject.GetComponent<EnemyAttackManager>().animationClips[randIndex];
                                }

                            }
                        }
                    }
                  
                    onParried?.Invoke(this, EventArgs.Empty);

                   

                }
               
            }
            else
            {
                Debug.Log("Can't access player combat timer");
            }
        }
        return;
    }

    public Vector3 GetClosestPoint() => retrieveClosestPoint;


}
