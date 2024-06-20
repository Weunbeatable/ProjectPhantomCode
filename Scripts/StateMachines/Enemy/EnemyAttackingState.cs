using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackingState : EnemyBaseState
{
    private  int AttackHash; // storing hash for locomotion speed parameter, can't use readonly for my list of attacks though
    private string chosenAttack;
    private const float TransitionDuration = 0.1f;
    AnimatorStateInfo currentInfo;
    public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        PlayerTargetingState.OnTriggerConfusion += PlayerTargetingState_OnTriggerConfusion;
        stateMachine.Weapon.SetAttack(stateMachine.AttackDamage, stateMachine.Attackknockback);
        stateMachine.headDamage.SetAttack(stateMachine.AttackDamage, stateMachine.Attackknockback);
        stateMachine.rightHandDamage.SetAttack(stateMachine.AttackDamage, stateMachine.Attackknockback);
        stateMachine.leftHandDamage.SetAttack(stateMachine.AttackDamage, stateMachine.Attackknockback);
        stateMachine.rightLegDamage.SetAttack(stateMachine.AttackDamage, stateMachine.Attackknockback);
        stateMachine.leftLegDamage.SetAttack(stateMachine.AttackDamage, stateMachine.Attackknockback);

        SetupChosenAttackFromAttackList();

    }

    public override void Tick(float deltaTime)
    {
        // This will need to be refactored when enemies with other attackign ranges are added in
        stateMachine.isInAttackingRange = true;
        FacePlayer();
        AddAttackToListOfStealableMoves();
        if (GetNormalizedTime(stateMachine.Animator, "Attack") >= 1)
        {
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
        }

    }



    public override void Exit()
    {
        stateMachine.isInAttackingRange = false;
        PlayerTargetingState.OnTriggerConfusion -= PlayerTargetingState_OnTriggerConfusion;
    }

    private void AddAttackToListOfStealableMoves()
    {
        currentInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateMachine.attackManager.animationNamesHash.Count >= 0 /*&&
            stateMachine.Animator.GetCurrentAnimatorClipInfo(0)[0].clip != null &&
            stateMachine.attackManager.animationClips.Count >=0 &&
            !stateMachine.attackManager.animationClips.Contains(stateMachine.Animator.GetCurrentAnimatorClipInfo(0)[0].clip)*/)
        {
            // check to avoid adding duplicates 
            if (currentInfo.IsTag("Attack"))
            {
                //stateMachine.attackManager.animationClips.Add(stateMachine.Animator.GetCurrentAnimatorClipInfo(0)[0].clip);
                if (!stateMachine.attackManager.animationNamesHash.Contains(stateMachine.Animator.GetCurrentAnimatorStateInfo(0).shortNameHash))
                {
                    stateMachine.attackManager.animationNamesHash.Add(stateMachine.Animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
                }
            }
            //  else { return; }
        }
    }
    private void SetupChosenAttackFromAttackList()
    {
        System.Random rnd = new System.Random();
        int randIndex = rnd.Next(stateMachine.enemyAttack.GetAttackList().Count);
        if (stateMachine.enemyAttack.GetAttackList().Count > 0) // verify there are attacks to take from
        {
            chosenAttack = stateMachine.enemyAttack.GetAttackList()[randIndex];
            AttackHash = Animator.StringToHash(chosenAttack);
            Debug.Log("The attack hash value is " + AttackHash);
            stateMachine.Animator.CrossFadeInFixedTime(AttackHash, TransitionDuration);
        }
    }

    private void PlayerTargetingState_OnTriggerConfusion()
    {
            stateMachine.SwitchState(new EnemySearchingState(stateMachine));
    }
}
