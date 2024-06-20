using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu1 : MonoBehaviour
{
    public static EventHandler onMenuPaused;
    public static EventHandler onMenuClosed;

    private Controls playerInput;
    [SerializeField] private InputReader inputReader;
    [SerializeField] GameObject panel;
    [SerializeField] private List<GameObject> ObjectsToTurnOff;

    bool isPaused = false;
    // Start is called before the first frame update\
    // use event system for on selected to cycle through panels 
    // link title of gameobject to what is shown on top of UI panel for user. 
    // 

    private void Awake()
    {
       playerInput = new Controls();
    }
    void Update()
    {

        // VisualizeControlSchemes();

    }
    private void OnEnable()
    {
        inputReader.PauseEvent += InputReader_PauseEvent;
        playerInput.Enable();
    }

  

    private void OnDisable()
    {
        inputReader.PauseEvent -= InputReader_PauseEvent;
        playerInput.Disable();
    }

    public void InputReader_PauseEvent() // Event call to either pause or resume gameplay 
    {

        if (isPaused == false)
        {
            isPaused = true;
            onMenuPaused?.Invoke(this, EventArgs.Empty);
            playerInput.UINavigation.Enable();
            playerInput.Player.Disable();
            Time.timeScale = 0;
            panel.SetActive(true);
            Cursor.visible = true;
        }
        else
        {
            isPaused = false;
            onMenuClosed?.Invoke(this, EventArgs.Empty);
            playerInput.UINavigation.Disable();
            playerInput.Player.Enable();
            Time.timeScale = 1;
            panel.SetActive(false);
            Cursor.visible = false;

            foreach (GameObject listItem in ObjectsToTurnOff)
            {
                if (listItem.activeSelf == true)
                {
                    listItem.SetActive(false);
                }
            }
        }
    }
    private void SwitchActionMap(InputAction.CallbackContext context)
    {

    }
    private void VisualizeControlSchemes()
    {
  /*      if (Input.GetKeyDown(KeyCode.B))
        {
            if (testforUINav.activeSelf == false)
            {
                testforUINav.SetActive(true);
            }
            else
            {
                testforUINav.SetActive(false);
            }
        }*/
    }

    public void ResumeGame()
    {
        foreach (GameObject listItem in ObjectsToTurnOff)
        {
            if(listItem.activeSelf == true)
            {
                listItem.SetActive(false);
            }
        }
        if(isPaused == true)
        {
            Time.timeScale = 1;
            panel.SetActive(false);
            Cursor.visible = false;
            isPaused = false;
            onMenuClosed?.Invoke(this, EventArgs.Empty);
            playerInput.UINavigation.Disable();
            playerInput.Player.Enable();
        }
    }

    public void RestartGame()
    {
        /* string currentSceneName = SceneManager.GetActiveScene().name;
         SceneManager.LoadScene(currentSceneName);*/
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
        panel.SetActive(false);
        Cursor.visible = false;
        isPaused = false;
        onMenuClosed?.Invoke(this, EventArgs.Empty);
        playerInput.UINavigation.Disable();
        playerInput.Player.Enable();
    }
}
