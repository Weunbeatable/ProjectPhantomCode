using PFF.Abilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

/// <summary>
/// purpose of this class is to create some base rules for how an ability system should work,
/// Each ability system should implement the iabilites interface
/// they should be of type ability so as to make it easier for delegate calls 
/// They should check to see if the ability has been selected in an ability slot
/// check to see if an ability is in use
/// should be tied to the player. 
/// The class will be abstract so as to force implementing some key functionality across
/// all abilites, making it easeir for production once an ability inherits the class
/// when making the delegate call for the ability UI it will make it
/// easy to reference with some values being public like the name of the ability etc. 
/// Also have to account for phantom abilites vs regular player abilites. 
/// requring a further level of abstraction, separate inheritance for phantom abilites and player
/// abilites, as phantom constantly needs access to animator and a bunch of other data. 
/// </summary>
public abstract class BaseAbilitySystem : MonoBehaviour, IAbilites, IPointerClickHandler
{
    protected bool isAbilityInUse;
    protected Action onAbilityUseComplete;
    [SerializeField] protected string abilityName;
    [SerializeField] protected Image abilityIcon;

    private bool istapOtion; 
    private bool isholdOption;

    public delegate void IAbilitesDelegate(IAbilites ability); // delegate allows flexibility for passing Iabilites interface function to UI manager
    IAbilitesDelegate delegateAbilityCallback; // setting up callback as I have to account for controller and m&k inputs

    public abstract string GetAbilityName();

    public void SetAbilityName(string abilityName)
    {
        this.abilityName = abilityName;
    }
    public string GetAbilityStringName() => abilityName;

    public Sprite GetAbilityImageIcon() => abilityIcon.sprite;

    public void OnPointerClick(PointerEventData eventData)
    {
        delegateAbilityCallback?.Invoke(this);
    }

    // CALLBACKS FOR LEFT AND RIGHT CLICKING
    // SYSTEM SHOULD BE A LIST AND THE ARRAY ITEM AT THAT INDEX SHOULD BE WHAT THE CALLBACK CALLS

    public abstract void UseAbility();

    public bool GetTapStatus() => istapOtion;

    public bool GetHoldStatus() => isholdOption;

    public bool SetTapStatus(bool tapStatus) => istapOtion = tapStatus;

    public bool SetHoldStatus(bool holdStatus) => isholdOption = holdStatus;

    public abstract void AddComponent();

    public abstract void RemoveComponent();

}
