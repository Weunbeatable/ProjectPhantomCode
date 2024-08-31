using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

//using UnityEditorInternal;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    //Purpose:
    /*
     * This script is to allow for shared methods between different player states
     * A reference to player so all player states can reference player
     */
    public static event Action onMimicingCommands;
    public static event Action onFilledPhantomMimicCommandMeter;
 
    public bool isBufferActive  = true; 
    protected PlayerStateMachine stateMachine;
    protected State previousState;

    public enum attackButton { basicAttack = 0, HeavyAttack = 1, basicHold = 2, HeavyHold = 3 }
    public attackButton chosenFightingStyle;

    public List<attackButton> inputBuffer = new List<attackButton>();
    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void Move(Vector3 motion, float deltaTime) // movement based on input and taking in delta time.
    {
        stateMachine.characterController.Move((motion + stateMachine.forceReceiver.Movement) * deltaTime); // whenever we move the player with forces we just call this method, to take the amount we want to move by added with our forces

    }

    protected void Move(float deltaTime)
    {
        stateMachine.characterController.Move((stateMachine.forceReceiver.Movement) * deltaTime);
        Move(Vector3.zero, deltaTime); // Will ensure non moving states like blocking will still allow for knockback and moving with gravity and not input
       //FaceTarget();
    }

    protected void MoveWithDIrectionFace(Vector3 motion, float deltaTime) // movement based on input and taking in delta time.
    {
        stateMachine.characterController.Move((motion + stateMachine.forceReceiver.Movement) * deltaTime); // whenever we move the player with forces we just call this method, to take the amount we want to move by added with our forces

        stateMachine.transform.rotation = Quaternion.Lerp(
        stateMachine.transform.rotation,
        Quaternion.LookRotation(motion),
        deltaTime * stateMachine.RotationDamping);
    }
    protected void FaceTarget()
    {
        if(stateMachine.targeter.currentTarget == null) { return; } // make sure we have a target
        Vector3 facing = (stateMachine.targeter.currentTarget.transform.position - stateMachine.transform.position ); // subtract target from our postion

        facing.y = 0;// dont care about height so we clear it

        float angle = Vector3.Dot(stateMachine.transform.position, stateMachine.targeter.currentTarget.transform.position);
        if (angle >= 0.25f)
        {
            stateMachine.transform.rotation = Quaternion.LookRotation(facing); // creating a vector 3 to a quaternion
        }
       
        //TODO
        // add a field so that within a range facing works
        // if too close dont use facing so the camera doesn't spaz out
    }

    protected void ReturnToLocomotion()
    {
        if (stateMachine.targeter.currentTarget != null)
        {
            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
        }
        else
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        }

    }

    protected void ShouldTargetingStillOccur()
    {
        if (previousState != null)
        {
            if (previousState is PlayerTargetingState)
            {
                if (!stateMachine.targeter.SelectTarget()) { return; } // assuming the last state was the targeting state, if we still have a current active target, lets continue facing that targety
                FaceTarget();
            }
        }
    }

    protected void MoveToEnemy()
    {
        if (stateMachine.targeter.currentTarget != null)
        {
            float distance = (stateMachine.characterController.transform.position - stateMachine.targeter.currentTarget.transform.position).sqrMagnitude;

            /*if (distance < stateMachine.clippingDistance)
            {
                Vector3 extra_distance = new Vector3(1f, 0, 1f);
                Vector3 offset = (stateMachine.transform.position - stateMachine.targeter.currentTarget.transform.position) + extra_distance;
               Move(offset, 0.01f * Time.deltaTime);
            }*/
            Vector3 extra_distance = new Vector3(0.3f, 0, 0.3f);
            Vector3 offset = (stateMachine.transform.position - stateMachine.targeter.currentTarget.transform.position) + extra_distance;
            Vector3 ofssetLerp = Vector3.Lerp(stateMachine.characterController.transform.position, stateMachine.targeter.currentTarget.transform.position + extra_distance, 0.1f );
           
            Move(ofssetLerp, Time.deltaTime);
            //stateMachine.characterController.transform.DOMove(offset, 0.2f);
        }
        else { return; }
    }

    protected void CheckForWalll()
    {
        stateMachine.wallRight = Physics.Raycast(stateMachine.characterController.transform.position, stateMachine.orientation.right, 
                                                out stateMachine.rightWallHit, stateMachine.wallCheckDistance, stateMachine.CheckIfIsWall);
        stateMachine.wallLeft = Physics.Raycast(stateMachine.characterController.transform.position, -stateMachine.orientation.right,
                                               out stateMachine.leftWalllhit, stateMachine.wallCheckDistance, stateMachine.CheckIfIsWall);
    }

    protected bool MinimumWalllRunHeight()
    {
        return !Physics.Raycast(stateMachine.characterController.transform.position, Vector3.down,
                                stateMachine.minJumpHeight, stateMachine.CheckIfIsGround);
    }
    
    protected void WallRunningMovement(float deltaTime)
    {
        //stateMachine.forceReceiver.Reset();
        //Move(stateMachine.InputReader.MovementValue, deltaTime);
        
      Vector3 velocity = new Vector3(
                                    stateMachine.characterController.velocity.x,
                                    0f,
                                    stateMachine.characterController.velocity.z);
        

        Vector3 wallNormal = stateMachine.wallRight ? stateMachine.rightWallHit.normal : stateMachine.leftWalllhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, stateMachine.characterController.transform.up);

        if ((stateMachine.orientation.forward - wallForward).magnitude > (stateMachine.orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        //forward force
       // stateMachine.forceReceiver.WallRunForce(wallForward * stateMachine.wallRunForce, ForceMode.Force);
        Move(wallForward * stateMachine.wallRunForce, 0.75f *deltaTime);

        float pushToWAllForce = .08f;
        //push to wall force
        if(!(stateMachine.wallLeft && stateMachine.InputReader.MovementValue.x >0) && 
           !(stateMachine.wallRight && stateMachine.InputReader.MovementValue.x < 0))
       // stateMachine.forceReceiver.WallRunForce(-wallNormal * pushToWAllForce, ForceMode.Force);
        Move(-wallNormal * pushToWAllForce, 0.75f * deltaTime);

        //upward and downward force
        bool downwardRun;
        bool upwardRun;
        downwardRun = stateMachine.InputReader.isLeftSpecial;
        upwardRun = stateMachine.InputReader.isRightSpecial;
        if (downwardRun == true)
        {
            velocity = new Vector3(
                                   stateMachine.characterController.velocity.x,
                                   stateMachine.wallClimbSpeed,
                                   stateMachine.characterController.velocity.z);
                    
        }
        if (upwardRun == true)
        {
            velocity = new Vector3(
                                  stateMachine.characterController.velocity.x,
                                  -(stateMachine.wallClimbSpeed * 1.2f),
                                  stateMachine.characterController.velocity.z);
        }
        Move(velocity, deltaTime);
    }

    protected void WallJump()
    {
        stateMachine.exitingWall = true;
        stateMachine.exitWallTimer = stateMachine.exitWallTime;
        Vector3 wallNormal = stateMachine.wallRight ? stateMachine.rightWallHit.normal :stateMachine.leftWalllhit.normal;


        Vector3 jumpForceToApply = stateMachine.characterController.transform.up * stateMachine.wallJumpUpForce + wallNormal * stateMachine.wallJumpSideForce;

        // resety velocity add the forces

        Move(jumpForceToApply, Time.deltaTime);
      //  stateMachine.forceReceiver.WallJumpForce(jumpForceToApply, ForceMode.Impulse);
    }
    protected Vector3 ApplyInputToMomentum(Vector3 _momentum, float deltaTime) //TO DO: Implement this in jumping and falling states for better air control
    {
        Vector2 movement = stateMachine.InputReader.MovementValue * stateMachine.JumpingInputSensitivity;
        Vector3 movementVector = new Vector3(movement.x, 0, movement.y);
        movementVector = stateMachine.transform.TransformVector(movementVector);
        return Vector3.Lerp(_momentum, movementVector, deltaTime);
    }
    protected void CheckForAnimationOrientation(int AnimationHashLeftSide, int AnimationHashRightSide, float CrossFadeDuration)
    {
        if (stateMachine.wallLeft) // this may be extracted in the future to accomodate for other types of left & right side checks
        {
            stateMachine.Animator.CrossFadeInFixedTime(AnimationHashLeftSide, CrossFadeDuration);
        }

        if (stateMachine.wallRight)
        {
            stateMachine.Animator.CrossFadeInFixedTime(AnimationHashRightSide, CrossFadeDuration);
        }
    }

    protected void Spacing()
    {
        if (stateMachine.targeter != null)
        {
            float distance = Vector3.Distance(stateMachine.transform.position, stateMachine.targeter.transform.position);
        }
    }

    protected void StopAcceptingPlayerInputOnLastHit()
    {
        if(stateMachine.InputReader.isBasicAttack ||
           stateMachine.InputReader.isHeavyAttack ||
           stateMachine.InputReader.isBasicHoldAttack ||
           stateMachine.InputReader.isHeavyHoldAttack ||
           stateMachine.InputReader.isBasicMultiTapAttack ||
           stateMachine.InputReader.isHeavyMultiTapAttack)
        {
            return;
        }
    }

    protected void CheckToTeleportOrDash()
    {
        if (stateMachine.meshTrailVisual.GetStatusOfLastPosition() == true && stateMachine.dashCoolDownTimer > 0)
        {
           
            stateMachine.characterController.enabled = false;
            stateMachine.transform.position = stateMachine.lastKnownPost;
            stateMachine.transform.rotation = stateMachine.lastKnownRotation;
            stateMachine.characterController.enabled = true;
            stateMachine.meshTrailVisual.DeSpawnMeshesPostTimer();
            stateMachine.meshTrailVisual.SetStatusOfLastPosition(false);
        }
        if (stateMachine.dashCoolDownTimer <= 0)
        {
            stateMachine.meshTrailVisual.SetStatusOfLastPosition(false);
            Debug.Log("LastPosStatus is " + stateMachine.meshTrailVisual.GetStatusOfLastPosition());
            stateMachine.SwitchState(new PlayerDashingState(stateMachine, stateMachine.InputReader.MovementValue)); // flexibility if we wantto dodge with something other than movement value
        }
        
    }
    protected virtual void PhantomAttackHandler(Attack attack) // use this instead of calling the exact same method
                                                               // over and over, will make code much easier to read and will get the same effect
                                                               // invoke the event here  with this protected virtual method in order to have consistency
                                                               // in calling for the visual change to be made, may need a boolean check? 
    {

        if (PhantomCombatMimicAbility.istapped == true)
        {
            if (PhantomCombatMimicAbility.playPhantomAttacks.Count <= 10)
            {
                onMimicingCommands?.Invoke();
                //stateMachine.TestDataCollection.Add(attack.AnimationName);
                PhantomCombatMimicAbility.SwithchPhantomController.Add(stateMachine.Animator.runtimeAnimatorController.name);
                PhantomCombatMimicAbility.playPhantomAttacks.Add(attack.AnimationName);
                stateMachine.effects.PlayMultipleStoringEffect();
            }
            if (PhantomCombatMimicAbility.playPhantomAttacks.Count == 9)
            {
                onFilledPhantomMimicCommandMeter?.Invoke();
                //return;
               // stateMachine.effects.PlayPhantomCastingTimeEffect();
            }
          
        }
        if (PhantomCombatMimicAbility.isAttackReplayActive == true)
        {
            if (PhantomCombatMimicAbility.playPhantomAttacks.Count >= 10)
            {
                PhantomCombatMimicAbility.SwithchPhantomController.Clear();
                PhantomCombatMimicAbility.playPhantomAttacks.Clear();
                // stateMachine.TestDataCollection.Clear();
            }
        }
    }

    protected virtual void PhantomParry()
    {
        //if(PhantomStealAbility.istapped())
    }
    protected void TryApplyForce(bool alreadyAppliedForce, Attack attack)
    {
        if (alreadyAppliedForce) { return; }
        FaceTarget();
        stateMachine.forceReceiver.AddForce(stateMachine.transform.forward * attack.Force * .01f);

        alreadyAppliedForce = true;
    }

    protected void InputBufferForFinishers()
    {
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Attack");
        
            stateMachine.targeter.SelectTarget();
            if (stateMachine.targeter.currentTarget != null)
            {
                float distanceToAllowFinisher = Vector3.Distance(stateMachine.characterController.transform.position,
                       stateMachine.targeter.currentTarget.transform.position);
                if (distanceToAllowFinisher < 2f && stateMachine.targeter.GetFinishState() == true)
                {
                    Debug.Log("Finish Him");
                    stateMachine.targeter.SetFinishState(false);
                    stateMachine.SwitchState(new PlayerFinisherState(stateMachine));
                    return;
                }
                else if (stateMachine.targeter.GetFinishState() == false)
                {
                    return;
                }
            }
        
    }
    protected Vector3 CalculateMovement()
    {
        Vector3 cameraForward = stateMachine.MainCameraTransform.forward;
        Vector3 cameraRight = stateMachine.MainCameraTransform.right;
        cameraForward.y = 0;// don't need to tilt camera
        cameraRight.y = 0;
        Vector3.Normalize(cameraForward); // want to ensure uniform speed in all directions
        cameraRight.Normalize();

        return cameraForward * stateMachine.InputReader.MovementValue.y + cameraRight * stateMachine.InputReader.MovementValue.x;
    }

    /*protected bool ValidateDistanceDuringAttack()
    {
        bool isValid;
        if (stateMachine.targeter.currentTarget != null)
        {
            //Vector3 distance =  (stateMachine.transform.position - stateMachine.targeter.transform.position).sqrMagnitude;
            float value = (stateMachine.targeter.currentTarget.transform.position - stateMachine.transform.position).sqrMagnitude;
            Debug.Log("Distance between player and enemy is " + value);
            // If within range and allowed to move
            if (value < stateMachine.boundaryDistance && stateMachine.combatModifiers.shouldAdjustPositionFromEnemy == 1) // Boundary distance was a throwaway variable, here I can check the distance between the enemy and player and determine if i'm close enough to allow for some magic lerping
            {
                
                isValid = true;
            }
            else
            {
                isValid = false;
            }
        }
        else
        {
            isValid = false;
        }
        return isValid;
    }

    protected void MoveToOpponent(float deltaTime)
    {
        if(ValidateDistanceDuringAttack() == true)
        {
            Vector3 offset = new Vector3(1.4f, 0, 1.4f);  
            Vector3 adjustment = Vector3.MoveTowards(stateMachine.transform.forward, stateMachine.targeter.currentTarget.transform.position - stateMachine.transform.position,  0.08f * deltaTime);
            adjustment.y = 0;
            stateMachine.transform.position = adjustment;
           
            Move(adjustment, deltaTime);
        }
    }
*/

    protected void HandleInputBuffer()
    {
 // this input buffer doesn't account for movement inputs (among other things) or multi hit, for the sake of this demo it won't be addressed here
 // but will be remodified in the unreal engine version of this project 
        
        if(stateMachine.InputReader.isBasicAttack && isBufferActive == true) 
        {           
            chosenFightingStyle = 0;
            inputBuffer.Add(chosenFightingStyle);
            Debug.Log(inputBuffer.Count);
            Debug.Log(inputBuffer[0].ToString());
            isBufferActive = false;
            return;
            
        }
        else if (stateMachine.InputReader.isHeavyAttack && isBufferActive == true) 
        {
          //  chosenFightingStyle = 1;
            inputBuffer.Add(chosenFightingStyle);
           // Debug.Log(inputBuffer.Count);
            return;
        }
        else
        {
            return;
        }
    }

    protected void ResetBufferActive()
    {
        isBufferActive = true;
    }

    public int GetInput() =>(int) chosenFightingStyle;

}
