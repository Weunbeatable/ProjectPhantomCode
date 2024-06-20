using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParkourState : PlayerBaseState
{
    //Parkour animations
    private readonly int ParkourSlide = Animator.StringToHash("Running Slide");
    private readonly int ParkourCorkscrewSlide = Animator.StringToHash("Corkscrew Slide");
    private readonly int SlideForwardHash = Animator.StringToHash("SlideForward");
    private readonly int JumpOverdHash = Animator.StringToHash("Jump Over");
    private readonly int CloseVaultHash = Animator.StringToHash("CloseDistanceVault");
    private readonly int FancyVaultHash = Animator.StringToHash("Fancyvault");
    private readonly int SwingtHash = Animator.StringToHash("Run_And_Swing");

    //Animation blend parameters
    private float previousFrameTime;
    private const float CrossFadeDuration = 0.1f;

    //player vector data
    private Vector3 parkourDirectionInput;
    
    public PlayerParkourState(PlayerStateMachine stateMachine, Vector3 parkourDirectionInput) : base(stateMachine)
    {
        this.parkourDirectionInput = parkourDirectionInput;
    }

    public override void Enter()
    {
        //stateMachine.parkour.GetSlideDistance()
        VaultingLogic();
        SlidingLogic();
        SwingLogic();
        stateMachine.Animator.applyRootMotion = true;
    }

  


    public override void Tick(float deltaTime)
    {

        Vector3 movement = new Vector3();

        
        movement += stateMachine.transform.forward * parkourDirectionInput.y * stateMachine.DodgeLength / (stateMachine.DodgeDuration * 3f);
        // movement vector is transform.right * input gets direction, length divided by duration gets us the distance
        // same thing for forwad
        Move(movement, deltaTime); // dodge depending on movement
        float normalizedTime = GetNormalizedTime(stateMachine.Animator, "Parkour");
        if (normalizedTime < 1f) { return; }
        stateMachine.parkour.ResetLastHit();
        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine, true));
        previousFrameTime = normalizedTime;
        



    }
    public override void Exit()
    {
        stateMachine.parkour.ResetLastHit();
        //TO DO: on exiting the animation and this can probably go in the tick
        // blend into running if forward input is being held so you don't just randomly
        // stop then keep going
    }

    private void SlidingLogic()
    {
        var chosenSlideAnim = Random.Range(0, 10);

        if (stateMachine.parkour.GetObjectParkourStatus() == 11 && chosenSlideAnim < 5f)
        {
            stateMachine.Animator.SetFloat(SlideForwardHash, parkourDirectionInput.y);
            stateMachine.Animator.CrossFadeInFixedTime(ParkourSlide, CrossFadeDuration);
        }
        if (stateMachine.parkour.GetObjectParkourStatus() == 11 && chosenSlideAnim >= 5f)
        {
            stateMachine.Animator.SetFloat(SlideForwardHash, parkourDirectionInput.y);
            stateMachine.Animator.CrossFadeInFixedTime(ParkourCorkscrewSlide, CrossFadeDuration);
        }
    }

    private void VaultingLogic()
    {
        var vaultDistance = Vector3.Distance(parkourDirectionInput, stateMachine.parkour.lasthit.transform.position);

        if (stateMachine.parkour.GetObjectParkourStatus() == 12)
        {
          /*  if (vaultDistance > stateMachine.parkourVaultDistance)
            {
                stateMachine.Animator.SetFloat(SlideForwardHash, parkourDirectionInput.y);
                stateMachine.Animator.CrossFadeInFixedTime(JumpOverdHash, CrossFadeDuration);
            }

            else 
            {*/
                stateMachine.Animator.SetFloat(SlideForwardHash, parkourDirectionInput.y);
                stateMachine.Animator.CrossFadeInFixedTime(FancyVaultHash, CrossFadeDuration);
            
        }
    }
    private void SwingLogic()
    {
        var vaultDistance = Vector3.Distance(parkourDirectionInput, stateMachine.parkour.lasthit.transform.position);
        if (stateMachine.parkour.GetObjectParkourStatus() == 13)
        {
            if (vaultDistance > stateMachine.parkourVaultDistance)
            {
                stateMachine.Animator.SetFloat(SlideForwardHash, parkourDirectionInput.y);
                stateMachine.Animator.CrossFadeInFixedTime(SwingtHash, CrossFadeDuration);
            }
        }
    }
}
