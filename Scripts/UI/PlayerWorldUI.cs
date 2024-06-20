using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWorldUI : CharacterWorldUI
{
    public static EventHandler onPlayDaHorn;

    [SerializeField] GameObject Player;
    [SerializeField] GameObject DashCooldownContainer;
    [SerializeField] Image DashCooldownImage;
    [SerializeField] RuntimeAnimatorController DashCooldownAnimatorController;
    [SerializeField] Sprite[] loadingSprites;
    public PlayerWorldUI(Image barImage, Health health, GameObject healthBarContainer) : base(barImage, health, healthBarContainer)
    {
    }

    private void Awake()
    {
        Player = GameObject.FindWithTag("Player");
        healthSystem = Player.GetComponent<Health>();
    }
    protected override void Start()
    {
        base.Start();
        PlayerDashingState.OnActivateMeshTrail += PlayerDashingState_OnActivateMeshTrail;
    }

    IEnumerator loadImages()
    {
        DashCooldownContainer.SetActive(true);
        while(Player.GetComponent<PlayerStateMachine>().dashCoolDownTimer > 0)
        {
            for (int i = 0; i < loadingSprites.Length; i++)
            {
                if (Player.GetComponent<PlayerStateMachine>().dashCoolDownTimer <= 0) // check if the cooldown is 0 while this loop is running, if thats the case just break out of the loop to avoid
                    // an awkward visual delay with the UI
                {
                    onPlayDaHorn?.Invoke(this, EventArgs.Empty);
                     break;
                }
                DashCooldownImage.sprite = loadingSprites[i];
               
                yield return new WaitForSeconds(0.25f);
                if (i == loadingSprites.Length) // loop through the sprites again if cooldown is still active. 
                {
                    i = 0;
                }

            }
            
        }
        DashCooldownContainer.SetActive(false);
    }
    private void PlayerDashingState_OnActivateMeshTrail(bool obj)
    {
        if(obj == true) { return; }//  trails are only active if dashing is still on, As soon as dashing finishes this boolean switches to false hence starting the counddown which triggers
        // this coroutine. 
        else 
        {
            StartCoroutine(loadImages());
        }
       
    }
    protected void UpdateCooldownBar()
    {
        DashCooldownImage.fillAmount = Player.GetComponent<PlayerStateMachine>().dashCoolDownTimer; 
    }

    private void OnDestroy()
    {
        PlayerDashingState.OnActivateMeshTrail -= PlayerDashingState_OnActivateMeshTrail;
    }
}
