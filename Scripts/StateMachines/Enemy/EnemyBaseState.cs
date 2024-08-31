using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState : State
{
    protected EnemyStateMachine stateMachine;
    public EnemyBaseState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    protected bool IsInSuspicionRange()
    {
        if (stateMachine.player.isDead) { return false; } // not in range to attack or chase if player dies

        float rangeToPlayer = Vector3.Distance(stateMachine.transform.position, stateMachine.player.transform.position);

            return rangeToPlayer <= stateMachine.PlayerSuspicionRange;
    }
    protected bool IsInChasingRange()
    {
        if (stateMachine.player.isDead) { return false; } // not in range to attack or chase if player dies

        float rangeToPlayer = Vector3.Distance(stateMachine.transform.position, stateMachine.player.transform.position);

        return rangeToPlayer <= stateMachine.PlayerChasingRange;
    }
    protected void Move(float deltaTime)
    {
        Move(Vector3.zero, deltaTime); // Will ensure non moving states like blocking will still allow for knockback and moving with gravity and not inpu
    }
    protected void Move(Vector3 motion, float deltaTime) // movement based on input and taking in delta time.
    {
        stateMachine.Controller.Move((motion + stateMachine.forceReceiver.Movement) * deltaTime); // whenever we move the player with forces we just call this method, to take the amount we want to move by added with our forces

    }

    protected void FacePlayer()
    {
        if (stateMachine.player == null) { return; } // make sure we have a target
        Vector3 facing = (stateMachine.player.transform.position - stateMachine.transform.position); // subtract target from our postion
        facing.y = 0;// dont care about height so we clear it

        stateMachine.transform.rotation = Quaternion.LookRotation(facing); // creating a vector 3 to a quaternion
    }
    // This function may be expanded to accomodate light and heavy weapon variants for better feel of impact animation. 
    protected void WeaponTypeImpactAnimation(int melee, int Weapon,   float CrossFadeDuration) // add int for projectile in future. or separate protected void
    {
        if (stateMachine.weaponType == "Melee")
        {
            stateMachine.Animator.CrossFadeInFixedTime(melee, CrossFadeDuration);
        }
         if(stateMachine.weaponType == "Weapon")
        {
            stateMachine.Animator.CrossFadeInFixedTime(Weapon, CrossFadeDuration);
        }
      
    }
    protected void StanceDependentImpactAnimation(int fighterResponse, int HeavySwordResponse, int AssasinResponse, int GunnerDefaultResponse, float CrossFadeDuration) //TO DO: Refactor and move to enemy Base state so this takes 4 string values instead so I can just call the function and hot swap values instead of manually adding and changing each time. 
    {
        if (stateMachine.playerResponse.isRespondingToFighter == true)
        {
            stateMachine.Animator.CrossFadeInFixedTime(fighterResponse, CrossFadeDuration);
        }
        else if (stateMachine.playerResponse.isRespondingToGreatSword == true)
        {
            stateMachine.Animator.CrossFadeInFixedTime(HeavySwordResponse, CrossFadeDuration);
        }
        else if (stateMachine.playerResponse.isRespondingToAssassin == true)
        {
            stateMachine.Animator.CrossFadeInFixedTime(AssasinResponse, CrossFadeDuration);
        }
        else
        {
            stateMachine.Animator.CrossFadeInFixedTime(GunnerDefaultResponse, CrossFadeDuration);
        }
    }
    public void FinisheedByPlayerFacePlayer()
    {
        if (stateMachine.player == null) { return; } // make sure we have a target
        Vector3 facing = (stateMachine.player.transform.position - stateMachine.transform.position); // subtract target from our postion
        facing.y = 0;// dont care about height so we clear it

        stateMachine.transform.rotation = Quaternion.LookRotation(facing); // creating a vector 3 to a quaternion
    }

    public virtual void LoadStates( EnemyStateMachine thisState)
    {
        // hitStates.Add("stun", SwitchState(new EnemyImpactState(this));
        // add heavy stun state, may need check for if hit by weapon or some other opening condition
        string hitState = stateMachine.hitReaction;
        switch (hitState)
        {
            case "stun":
               stateMachine.SwitchState(new StunState(thisState));
                break;
            case "stagger":
                stateMachine.SwitchState(new StaggerState(thisState));
                break;
            case "flyback":
                stateMachine.SwitchState(new FlyBackState(thisState));
                break;
            case "launcher":
                stateMachine.SwitchState(new PopUpStartState(thisState));
                break;
            case "dizzy":
                stateMachine.SwitchState(new DizzyState(thisState));
                break;
            case "knockdown":
                stateMachine.SwitchState(new KnockDownState(thisState));
                break;

            default:
                stateMachine.SwitchState(new EnemyImpactState(thisState));
                break;
        }
        // Outputs "Thursday" (day 4)

    }
}
