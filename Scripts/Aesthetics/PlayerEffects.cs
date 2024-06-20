using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] originalMaterials;
    [SerializeField] private Material[] cachedMaterials;
    [SerializeField] private Material dissolveMat;
    [SerializeField] private float dissolveSpeed = 0;

    [SerializeField] private float dissolveDuration = 1.4f;

    private void Awake()
    {
        if(cachedMaterials != null)
        {
            cachedMaterials = new Material[originalMaterials.Length];
        }
        for(int i = 0; i < originalMaterials.Length - 1; i++)
        {
            cachedMaterials[i] = originalMaterials[i].material;
        }
        
    }


    public void SwapToDissolve()
    {
        for(int i = 0; i <= originalMaterials.Length - 1; i++)
        {
            originalMaterials[i].material = dissolveMat;
        }
    }

    public void SwapBackToOriginal()
    {
        for (int i = 0; i <= originalMaterials.Length - 1; i++)
        {
            originalMaterials[i].material = cachedMaterials[i];
        }
        dissolveSpeed = 0;
        dissolveMat.SetFloat("_DissolveStrength", dissolveSpeed);       
    }

    public void PlayDissolveEffect()
    {
        float elappsedTime = 0;
        while (elappsedTime < dissolveDuration)
        {
            elappsedTime += Time.unscaledDeltaTime;
            dissolveMat.SetFloat("_DissolveStrength", dissolveSpeed);
            dissolveSpeed = Mathf.Lerp(dissolveSpeed, 1, elappsedTime / dissolveDuration);
        }
        
    }
}
