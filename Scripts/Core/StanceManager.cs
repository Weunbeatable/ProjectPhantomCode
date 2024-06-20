using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StanceManager : StateMachine
{
    // Stance data for specific classes on what they can and can't do will be configured here and then passed to the base class due to statemachines being inherited from the base class. 
    // This will make it much easier for them. 
    // Start is called before the first frame update
  //  [field: SerializeField] public RuntimeAnimatorController FightController { get; private set; }
   // [field: SerializeField] public RuntimeAnimatorController GunController { get; private set; }
   // [field: SerializeField] public RuntimeAnimatorController GreatSwordController { get; private set; }
   // [field: SerializeField] public RuntimeAnimatorController AssasinController { get; private set; }

    public readonly int FighterReadyAnim = Animator.StringToHash("Equip");
    public readonly int GunReadyAnim = Animator.StringToHash("Gun_Equip");
    public readonly int AssasinReadyAnim = Animator.StringToHash("Assasin_Equip");
    public readonly int GreatSwordReadyAnim = Animator.StringToHash("GreatSword_Equip");
    private const float CrossFadeDuration = 0.1f;

    private string previousStance;
    private string currenctStance;

    #region StanceAttackArrays
    // Fighters Arrays

    //Grounded
    [field: SerializeField] public Attack[] FighterAttacks { get; private set; }
    [field: SerializeField] public Attack[] FighterHeavyAttacks { get; private set; }
    [field: SerializeField] public Attack[] FighterBasicHoldAttack { get; private set; }
    [field: SerializeField] public Attack[] FighterHeavyHoldAttack { get; private set; }

    // Aireal


    // Assassins Arrays
    // Grounded
    [field: SerializeField] public Attack[] AssassinAttacks { get; private set; }
    [field: SerializeField] public Attack[] AssasinHeavyAttacks { get; private set; }
    [field: SerializeField] public Attack[] AssasinBasicHoldAttack { get; private set; }
    [field: SerializeField] public Attack[] AssasinHeavyHoldAttack { get; private set; }

    // Aireal
    #endregion

    //Generic function for switching stances, Most things will stay the same so there is no need to make separate functions multiple times. and this way I can have a function call that has some separate mechanics. 
    // since I can' tinherit this class due to it being called in the player statemachine as this call is made on enable as it can happen anywhere. 
    // I can just make a function that calls for this then lets me make adjustments as needed by adding extra calls in the function. 

    public void StanceModeChange(Action StanceInUseInvoke, VFXEffects effects, PropHandler props, Animator animator, RuntimeAnimatorController genericController, SoundEffects sfx, CharacterController characterController, PlayerStateMachine state)
    {
        StanceInUseInvoke?.Invoke();
        effects.assasinDustParticle.Play(); // event trigeer
        props.AssasinationProps(); // event trigger
        animator.runtimeAnimatorController = genericController; // event trigger
        animator.CrossFadeInFixedTime(AssasinReadyAnim, CrossFadeDuration); // event trigger if else check if in combat mode or not. 
        sfx.playerSource.PlayOneShot(sfx.assasinmode); // event trigger 
        if (characterController.isGrounded) // TODO:
        {
            //Check if in one of the many attacking states
            // pass to a constructor that checks animator data to see the controller currently in use. 
            // dependent on  the controller instructions can then be made to determine how to handle the state switc 

            //Check if targeting

            //Check if in a regular moving state
            SwitchState(new PlayerFreeLookState(state));
            // for freelook swapping, need to make another constructor, this will check to see if the player is not moving so that when entering the freelook state they play the ready anim first
            // otherwise just switch to freelook state. 
        }
        else
        {
            // Check if in air attacking state 

            // Check if falling
            SwitchState(new PlayerFallingState(state));
        }
    }

}
