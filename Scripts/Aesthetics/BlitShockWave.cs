using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlitShockWave : MonoBehaviour
{
    //Shadder effects
    [SerializeField] public Material blitShockwave;
    // Some private string references 
    private string _speed = "_Speed";
    private string _focalPoint = "_FocalPoint";
    private string _magnification = "_Magnification";
    private string m_sizeRatio = "_SizeRatio";

    // Variables that will be tweaked
    private Vector2 focalPointValue = new Vector2(0.5f, 0.5f);
    private float magnificationValue = -0.4f;
    private float speedValue = 1.5f;
    private float sizeRatioValue = 1.77f;
    public void TurnOnShockwave()
    {

        speedValue = 1.2f;
        focalPointValue = new Vector2(0.5f, 0.5f);
        magnificationValue = -0.5f;
        sizeRatioValue = 1.77f;

        blitShockwave.SetFloat(m_sizeRatio, sizeRatioValue);
        blitShockwave.SetFloat(_speed, speedValue);
        blitShockwave.SetFloat(_magnification, magnificationValue);
        blitShockwave.SetVector(_focalPoint, focalPointValue);

    }

    public void TurnOffShockwave()
    {
        speedValue = 0f;
        focalPointValue = new Vector2(0.0f, 0.0f);
        magnificationValue = 0.0f;
        sizeRatioValue = 0f;

        blitShockwave.SetFloat(m_sizeRatio, sizeRatioValue);
        blitShockwave.SetFloat(_speed, speedValue);
        blitShockwave.SetFloat(_magnification, magnificationValue);
        blitShockwave.SetVector(_focalPoint, focalPointValue);
    }
}
