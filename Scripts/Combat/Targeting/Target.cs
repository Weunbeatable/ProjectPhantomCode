using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Target : MonoBehaviour
{
    public event Action<Target> OnDestroyed;
    
    // just before it is destroyed from the world let the targeter know its been destroyed so it can remove from the list. This will help to prevent issues in case target was destroyed and on trigger exit can't run
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this);// In this instance the object that is being destoryed is the target so we refrence itself here
    }
}

