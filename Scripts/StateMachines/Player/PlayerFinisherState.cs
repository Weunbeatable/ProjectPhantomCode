using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFinisherState : PlayerBaseState
{
    // Camera specific events
    public static event Action<Transform> OnPlayerFinisherCamera = delegate { };
    public static event Action OnPlayerFinisherCameraEnd;
    // General finisher event 
    public static event Action<Transform> OnFinisherActionStarted;
    public static event Action onFinisherActionFinished;
    public static event Action<string> onPassFinisherStatedata;

    private string FighterAnimator = "Controller_Player";
    private string GunSlingerAnimator = "GunslingerStyle";
    private string GreatSwordAnimator = "GreatSword_Controller";
    private string AssasinAnimator = "Assasin_Controller";

    private bool isFighterAnimator;
    private bool isGunSlingerAnimator;
    private bool isGreatSwordAnimator;
    private bool isAssasinAnimator;

    private float transitionDuration = 0.1f;
    public PlayerFinisherState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        
        stateMachine.characterController.Move(Vector3.MoveTowards(stateMachine.characterController.transform.forward, stateMachine.targeter.currentTarget.transform.position, 1.5f) * Time.deltaTime);
        FaceTarget();
        // To avoid messing with script timings and to get around camera triggering late I will have two more event actions specifically for the camera. 
        OnPlayerFinisherCamera?.Invoke(stateMachine.targeter.currentTarget.transform); // call for finisher camera 
        OnFinisherActionStarted?.Invoke(stateMachine.targeter.currentTarget.transform); 
        stateMachine.Animator.applyRootMotion = true;
        DetermineFinisher(stateMachine.playerFinisher.finishers);
        stateMachine.targeter.Cancel();
    }

    public override void Tick(float deltaTime)
    {
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Finisher");

        FaceTarget();
        if (normalizedTime < 1f) // if greater than previous do something. if greater than 1 animation has finished, may remove the && for animation cancel
        {
            return;
        }
        else
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine)); // free look state
        }
    }

    public override void Exit()
    {
        OnPlayerFinisherCameraEnd?.Invoke(); 
        onFinisherActionFinished?.Invoke(); // broadcast to listeners that finisher has ended
        stateMachine.Animator.applyRootMotion = false;
    }
    private void DetermineFinisher(List<SetupFinishers> setup)
    {

        CheckFinisherStanceAnimatorNameData();
        var random = new System.Random(); // setup randomizer
        bool isSelectedAFinisher = false;

        stateMachine.Animator.applyRootMotion = true;

        int index;  // select a random element within the list
        if (setup.Count > 0)
        {
            while (isSelectedAFinisher != true) // Going to refactor this for a dedicated finisher state to help prevent... mishaps
            {
                index = random.Next(setup.Count);  // pick a random finisher value
                if ( setup[index].GetCurrentFightingStyleValue() == 0 && isFighterAnimator != true) { continue; } // If one of these conditions isn't met, break out of the loop
                else if(setup[index].GetCurrentFightingStyleValue() == 0 && isFighterAnimator == true) // upon acheiving both conditions play animation
                {
                    //Debug.Log("Animation name is " + setup[index].FinisherName);
                        stateMachine.Animator.Play(setup[index].FinisherName);
                    if(stateMachine.targeter.currentTarget != null) // Secondary check to ensure there is a target is still alive.
                    {
                        if (stateMachine.targeter.currentTarget.gameObject.TryGetComponent<EnemyStateMachine>(out EnemyStateMachine baseAttribute))
                        {
                            baseAttribute.isTargetOfFinish = true; // probably not the best way to expose, may rework later. 
                        }
                        onPassFinisherStatedata?.Invoke(setup[index].FinishedName);
                      /*  if(stateMachine.targeter.currentTarget.gameObject.TryGetComponent<Animator>(out Animator victimAniamtor))
                        {
                            victimAniamtor.applyRootMotion = true;
                            victimAniamtor.Play(setup[index].FinishedName);
                        }*/
                    }
                    
                    isSelectedAFinisher = true;
                }
                if (setup[index].GetCurrentFightingStyleValue() == 1 && isGunSlingerAnimator != true) { continue; }
                else if(setup[index].GetCurrentFightingStyleValue() == 1 && isGunSlingerAnimator == true)
                {
                        stateMachine.Animator.Play(setup[index].FinisherName);
                    if (stateMachine.targeter.currentTarget != null) // Secondary check to ensure there is a target is still alive.
                    {
                        if (stateMachine.targeter.currentTarget.gameObject.TryGetComponent<EnemyStateMachine>(out EnemyStateMachine baseAttribute))
                        {
                            baseAttribute.isTargetOfFinish = true; // probably not the best way to expose, may rework later. 
                        }
                        onPassFinisherStatedata?.Invoke(setup[index].FinishedName);
                       /* if (stateMachine.targeter.currentTarget.gameObject.TryGetComponent<Animator>(out Animator victimAniamtor))
                        {
                            victimAniamtor.applyRootMotion = true;
                            victimAniamtor.Play(setup[index].FinishedName);
                        }*/
                    }
                    isSelectedAFinisher = true;
                }
                if (setup[index].GetCurrentFightingStyleValue() == 2 && isGreatSwordAnimator != true) { continue; }
                else if(setup[index].GetCurrentFightingStyleValue() == 2 && isGreatSwordAnimator == true)
                {
                        stateMachine.Animator.Play(setup[index].FinisherName);
                    if (stateMachine.targeter.currentTarget != null) // Secondary check to ensure there is a target is still alive.
                    {
                        if (stateMachine.targeter.currentTarget.gameObject.TryGetComponent<EnemyStateMachine>(out EnemyStateMachine baseAttribute))
                        {
                            baseAttribute.isTargetOfFinish = true; // probably not the best way to expose, may rework later. 
                        }
                        onPassFinisherStatedata?.Invoke(setup[index].FinishedName);
                      /*  if (stateMachine.targeter.currentTarget.gameObject.TryGetComponent<Animator>(out Animator victimAniamtor))
                        {
                            victimAniamtor.applyRootMotion = true;
                            victimAniamtor.Play(setup[index].FinishedName);
                        }*/
                    }
                    isSelectedAFinisher = true;
                }
                if (setup[index].GetCurrentFightingStyleValue() == 3 && isAssasinAnimator != true) { continue; }
                else if(setup[index].GetCurrentFightingStyleValue() == 3 && isAssasinAnimator == true)
                {
                    stateMachine.Animator.Play(setup[index].FinisherName);
                    if (stateMachine.targeter.currentTarget != null) // Secondary check to ensure there is a target is still alive.
                    {
                        if (stateMachine.targeter.currentTarget.gameObject.TryGetComponent<EnemyStateMachine>(out EnemyStateMachine baseAttribute))
                        {
                            baseAttribute.isTargetOfFinish = true; // probably not the best way to expose, may rework later. 
                        }
                        onPassFinisherStatedata?.Invoke(setup[index].FinishedName);
                       /* if (stateMachine.targeter.currentTarget.gameObject.TryGetComponent<Animator>(out Animator victimAniamtor))
                        {
                            victimAniamtor.applyRootMotion = true;
                            victimAniamtor.Play(setup[index].FinishedName);
                            //victimAniamtor.applyRootMotion = false;
                        }*/
                    }
                    isSelectedAFinisher = true;
                }
              /*  else
                {
                    continue;
                }
*/
            }

        }
    }


private void CheckFinisherStanceAnimatorNameData()
    {
        if (stateMachine.Animator.runtimeAnimatorController.name == FighterAnimator)
        {
            isFighterAnimator = true;
        }
        if (stateMachine.Animator.runtimeAnimatorController.name == GunSlingerAnimator)
        {
            isGunSlingerAnimator = true;
        }
         if (stateMachine.Animator.runtimeAnimatorController.name == GreatSwordAnimator)
        {
            isGreatSwordAnimator = true;
        }
        if(stateMachine.Animator.runtimeAnimatorController.name == AssasinAnimator)
        {
            isAssasinAnimator = true;
        }
    }
}
