using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReference : MonoBehaviour
{
    public static InputReference Instance { get; private set; }

    [SerializeField]
    private InputActionAsset m_InputActionAsset;

    [SerializeField]
    private List<string> inputActionString = new List<string>();

    private string deviceName;

    private void Awake()
    {
        m_InputActionAsset.Enable();

        // Cycle through list of appropriate inputs, once an input is validated it will be passed into the find action string
        // I can then use this information to determine what kind of device is currently being used. 
        foreach (string action in inputActionString)
        {
            m_InputActionAsset.FindAction(action).performed += InputReference_performed;
        }
        
        //singleton
        if(Instance != null)
        {
            Debug.Log("Appears there is already an instance of this object " + transform + "oof" + m_InputActionAsset);
            Destroy(gameObject);
            return;
        }
        Instance = this;
       
    }
    private void Update()
    {
    }
    private void InputReference_performed(InputAction.CallbackContext obj)
    {
        // taking the callback context we can access the name method to figure out what kind of input was being used.
        //Debug.Log(obj.action.activeControl.device.name);
        SetDeviceName(obj.action.activeControl.device.name);
    }

    private void OnDestroy()
    {
        foreach (string action in inputActionString)
        {
            m_InputActionAsset.FindAction(action).performed -= InputReference_performed;
        }
    }

    public string GetDeviceName() => deviceName;
    public void SetDeviceName(string deviceName)
    {
        this.deviceName = deviceName;
    }
}
