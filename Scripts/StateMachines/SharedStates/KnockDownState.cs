using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockDownState : EnemyBaseState
{
    /// <summary>
    /// need to always have a tag refernce attached to each animation
    /// For this animation it is Knockdown
    /// </summary>
    /// <param name="stateMachine"></param>
    /// 

    // Grounded Knockdown States
    private readonly int noWeaponKnockdown = Animator.StringToHash("noWeaponKnockdown");
    private readonly int WeaponKnockdown = Animator.StringToHash("WeaponKnockdownState");
    // AerialKnockdownStates
    private readonly int noWeaponKnockdownInAir = Animator.StringToHash("noWeaponInAirKnowckdown");
    private readonly int WeaponKnockdownInAir = Animator.StringToHash("WeaponInAirKnowckDownState");

    private bool isEnemyGrounded;
    private float duration = 1.2f; // adding this so they dont get up immedeatly after a knockdown is done for a better game feel. 
    // in Future super heavy weapons like greatsword should have a special knockdown
    bool wasInAir;
    public KnockDownState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        FacePlayer();
        isEnemyGrounded = stateMachine.characterController.isGrounded;
       // stateMachine.Animator.applyRootMotion = true;
        if (isEnemyGrounded == true)
        {
            wasInAir = false;
            //stateMachine.Animator.applyRootMotion = true;
            WeaponTypeImpactAnimation(noWeaponKnockdown, WeaponKnockdown, 0.1f);
            
        }
        else
        {
            wasInAir = true;
            WeaponTypeImpactAnimation(noWeaponKnockdownInAir, WeaponKnockdownInAir, 0.1f);
            
        }
    }

    public override void Tick(float deltaTime)
    {
        
      float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Knockdown");

        Move(deltaTime);
        if (normalizedTime < 1f)
        {
            if (wasInAir == true)
            {
                
                if (stateMachine.characterController.isGrounded == true)
                {
                    stateMachine.SwitchState(new KnockDownState(stateMachine));
                }
            }
           // stateMachine.characterController.excludeLayers = 6;
            /*if(isEnemyGrounded == true)
            {
                stateMachine.SwitchState(new KnockDownState(stateMachine));
            }*/
            if (normalizedTime > 0.85f)
            {
                if (isEnemyGrounded == false)
                {
                    stateMachine.SwitchState(new PopUpFallingState(stateMachine));
                }
            }
        }


        if (normalizedTime > 1f)
        {
            duration -= deltaTime;
            stateMachine.SwitchState(new GetupState(stateMachine));
        }
        

    }
    public override void Exit()
    {

    }

  
}
