using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;
using TMPro;
using UnityEngine.UI;
using System;

public abstract class CharacterWorldUI : MonoBehaviour
{
    [SerializeField] protected Image healthBarImage;
    [SerializeField] protected Health healthSystem;
    [SerializeField] protected GameObject healthBarContainer;

    public CharacterWorldUI(Image barImage, Health health, GameObject healthBarContainer)
    {
        healthBarImage = barImage;
        healthSystem = health;
        this.healthBarContainer = healthBarContainer;
    }

    protected virtual void Start()
    {
        healthSystem.onTakeDamage += HealthSystem_onTakeDamage;
    }

    private void OnEnable()
    {
        PauseMenu1.onMenuPaused += PauseMenu1_onMenuPaused;
        PauseMenu1.onMenuClosed += PauseMenu1_onMenuClosed;
    }

    private void OnDisable()
    {
        PauseMenu1.onMenuPaused -= PauseMenu1_onMenuPaused;
        PauseMenu1.onMenuClosed -= PauseMenu1_onMenuClosed;
    }

    private void PauseMenu1_onMenuPaused(object sender, EventArgs e)
    {
        healthBarContainer.SetActive(false);
    }

    private void PauseMenu1_onMenuClosed(object sender, EventArgs e)
    {
        healthBarContainer.SetActive(true);
    }
    private void HealthSystem_onTakeDamage()
    {
        UpdateHeatlhBar();
    }

    protected void UpdateHeatlhBar()
    {
        healthBarImage.fillAmount = healthSystem.GetNormalizedHealth();
    }

   

}
