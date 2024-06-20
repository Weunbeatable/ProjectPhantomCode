using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using DG.DemiLib;

public class RadialMenuEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler // Adding interactive systems 
{
    public delegate void RadialMenuEntryDelegate(RadialMenuEntry pEntry); // crating a delegate to hook up buttons 
    [SerializeField]
    TextMeshProUGUI Label;

    [SerializeField]
    RawImage Icon;

    RectTransform Rect;
    RadialMenuEntryDelegate Callback; //Setting up the call back when we click

 
    private void Start()
    {
        Rect = GetComponent<RectTransform>();
        
    }
    public void SetLabel(string pText)
    {
        Label.text = pText;
    }
    
    //Apply Setters and Getters for Icons
    public void SetIcon(Texture pIcon)
    {
        Icon.texture = pIcon;
    }

    public Texture GetIcon()
    {
        return (Icon.texture);
    }



    
    public void SetCallback(RadialMenuEntryDelegate pCallback)
    {
        Callback = pCallback;
       
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Callback?.Invoke(this); // If we have a call back call it and passing itself as a parameter.
        Debug.Log("activebutton");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Rect.DOComplete(); // This clears out previous interpolations
        Rect.DOScale(Vector3.one * 1.5f, .3f).SetEase(Ease.OutQuad); // out quad is easing at ^2 starts find ends slow
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        Rect.DOComplete(); // This clears out previous interpolations
        Rect.DOScale(Vector3.one, .3f).SetEase(Ease.OutQuad); // animation starts fast and ends slow wtih ease out
    }
}
