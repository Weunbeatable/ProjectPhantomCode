using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureShader : MonoBehaviour
{
    [SerializeField] private Material dissolveMat;
    // Start is called before the first frame update'
    [SerializeField] private float EffectValue;
    [SerializeField] private float EffectValueStart;
    [SerializeField] private float EffectValueEnd;
    [SerializeField] private Vector3 fracturePosition;
    
    void Start()
    {
        EffectValueStart = 13;
    }

    // Update is called once per frame
    void Update()
    {
        fracturePosition = transform.position;
        dissolveMat.SetVector("_Center", fracturePosition);
        EffectValue = EffectValue + Mathf.Sin(Time.time + 3f );
        if (EffectValue <= 30)
            EffectValue = 30;

        /*if (EffectValue > 500)
        {
            EffectValue = EffectValue * -1f;
        }*/
        dissolveMat.SetFloat("_Effect", EffectValue);
    }
}
