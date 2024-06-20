using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;


public class InputReader : MonoBehaviour, Controls.IPlayerActions

{
    public enum AttackInputType { light = 0, heavy = 1, /*leftSpecial = 2, rightSpecial = 3*/ }
    public Vector2 MovementValue { get; private set; }
    public Vector2 LookValue { get; private set; }

    public event Action JumpEvent, DodgeEvent, TargetEvent, TauntEvent, FinishEvent, PauseEvent;

    private Controls controls;

    public bool isBasicAttack { get; private set; }
    public bool isHeavyAttack { get; private set; }
    public bool isBasicHoldAttack { get; private set; }
    public bool isHeavyHoldAttack { get; private set; }
    public bool isBasicMultiTapAttack { get; private set; }
    public bool isHeavyMultiTapAttack { get; private set; }
    public bool isFinisher { get; private set; }
    public bool isLeftSpecial { get; private set; }
    public bool isRightSpecial { get; private set; }
    public bool isLeftSpecialHold { get; private set; }
    public bool isRightSpecialHold { get; private set; }
    public bool isParkourEnabled { get; private set; }
    public bool LeftStanceEnabled { get; private set; }
    public bool RightStanceEnabled { get; private set; }
    public bool UpStanceEnabled { get; private set; }
    public bool DownStanceEnabled { get; private set; }
    public bool StanceMenuActive { get; private set; }


    public bool isBlocking { get; private set; }

    void Start()
    {
        controls = new Controls();
        controls.Player.SetCallbacks(this); // hooking up class to set callbacks so controls work properly

        controls.Player.Enable();
    }

    private void OnDestroy()
    {
        controls.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        JumpEvent?.Invoke(); // checking for subscribers to jump event then invoking it.
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        DodgeEvent?.Invoke();
    }
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookValue = context.ReadValue<Vector2>();
      //  Debug.Log("x Value is " + LookValue.x + " y Value is " + LookValue.y);
    }

    public void OnTarget(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        TargetEvent?.Invoke(); // checking for subscribers to jump event then invoking it.
    }



    public void OnBasicAttack(InputAction.CallbackContext context)
    {
        // TODO: Refactor this for a combo system in the future, instead of press and hold change it for press and release
        if (context.performed)
        {
            isBasicAttack = true;
        }
        else if (context.canceled)
        {
            isBasicAttack = false;
        }
    }

    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        // TODO: Refactor this for a combo system in the future, instead of press and hold change it for press and release
        if (context.performed)
        {
            isHeavyAttack = true;
        }
        else if (context.canceled)
        {
            isHeavyAttack = false;
        }
    }
    public void OnLeftSpecial(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction)
        {
            if (context.started)
            {
                isLeftSpecial = true;
                Debug.Log("tap interaction started");
            }
            else if (context.canceled || context.performed && context.time > 0.5f)
            {
                isLeftSpecial = false;
                Debug.Log("Tap interaction ended");
            }

        }
        else if (context.interaction is HoldInteraction)
        {
            if (context.performed)
            {
                isLeftSpecialHold = true;
               // Debug.Log("hold interaction started");
            }
            else if (context.canceled)
            {
                isLeftSpecialHold = false;
              //  Debug.Log("hold interaction ended");
            }
        }
    }

    public void OnRightSpecial(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction)
        {
            if (context.started)
            {
                isRightSpecial = true;
                Debug.Log("tap interaction started");
            }
            else if (context.canceled || context.performed && context.time > 0.5f)
            {
                isRightSpecial = false;
                Debug.Log("Tap interaction ended");
            }
           
        }
       else if(context.interaction is HoldInteraction)
        {
            if(context.performed)
            {
                isRightSpecialHold = true;
                Debug.Log("hold interaction started");
            }
            else if (context.canceled)
            {
                isRightSpecialHold = false;
                Debug.Log("hold interaction ended");
            }
        }
       
    }

    public void OnTaunt(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        TauntEvent?.Invoke(); // checking for subscribers to jump event then invoking it.
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isBlocking = true;
        }
        else if (context.canceled)
        {
            isBlocking = false;
        }
    }

    public void OnRightTriggerParkour(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isParkourEnabled = true;
        }
        else if (context.canceled)
        {
            isParkourEnabled = false;
        }
    }

    public void OnLeftStance(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            LeftStanceEnabled = true;
        }
        else if (context.canceled)
        {
            LeftStanceEnabled = false;
        }
    }

    public void OnRightStance(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RightStanceEnabled = true;
        }
        else if (context.canceled)
        {
            RightStanceEnabled = false;
        }
    }

    public void OnUpStance(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UpStanceEnabled = true;
        }
        else if (context.canceled)
        {
            UpStanceEnabled = false;
        }
    }

    public void OnDownStance(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DownStanceEnabled = true;
        }
        else if (context.canceled)
        {
            DownStanceEnabled = false;
        }
    }

    public void OnStanceMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StanceMenuActive = true;
        }
        else if (context.canceled)
        {
            StanceMenuActive = false;
        }
    }

    public void OnHoldBasicAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isBasicHoldAttack = true;
        }
        else if (context.canceled)
        {
            isBasicHoldAttack = false;
        }
    }

    public void OnHoldHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isHeavyHoldAttack = true;
        }
        else if (context.canceled)
        {
            isHeavyHoldAttack = false;
        }
    }

    public void OnMultiTapBasicAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isBasicMultiTapAttack = true;
        }
        else if (context.canceled)
        {
            isBasicMultiTapAttack = false;
        }
    }

    public void OnMultiTapHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isHeavyMultiTapAttack = true;
        }
        else if (context.canceled)
        {
            isHeavyMultiTapAttack = false;
        }
    }

    public void OnFinisher(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isFinisher = true;
        }
        else if (context.canceled)
        {
            isFinisher = false;
        }
        FinishEvent?.Invoke();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
       if(!context.performed) { return; }
           PauseEvent?.Invoke();
    }
}
