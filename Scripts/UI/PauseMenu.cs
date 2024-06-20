using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    /// <summary>
    /// This should manage Navigating basics of the UI in the pause menu and can be used for other menus as well. 
    /// buttons may work in a hiearchy due to having sub menus (or a separate panel may be the alternative where the script will be attached for naavigtion
    /// once a button is selected
    /// Activate panel (for each panel in list if name is not the same deactivate. 
    /// On pause, should always default to the first option (should listen for pause action? (event action???))
    /// Preconditions: Selected buttons and panels must be setup in inspector
    /// Posconditions: upon closing pause menu values should be reset. 
    /// Side effects: Its a pause menu bro I don't think there should be side effects unless I want it to affect the actual game :/
    /// </summary>

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
