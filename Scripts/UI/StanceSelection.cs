using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
namespace PFF.UI
{
    public class StanceSelection : MonoBehaviour
    {
        // TODO:
        // possibly if a finisher is done can turn camera to transfer finisher state so phantom can do its finisher? 

        //Reference to player statemachine
        public PlayerStateMachine machine;

        //audio source for UI 
        AudioSource selectSound;

        private Controls playerInput;
        int currentStance;
        float selectionTimer = 0.0f;
        float nextItemTimer;
        bool canStartTimer;

        private void Awake()
        {
            playerInput = new Controls();
            selectSound = GetComponent<AudioSource>();
        }
        void Start()
        {
            currentStance = 0;
            canStartTimer = false;
        }
        private void OnEnable()
        {
            PlayerFinisherState.OnFinisherActionStarted += PlayerFinisherState_OnFinisherActionStarted;
            PlayerFinisherState.onFinisherActionFinished += PlayerFinisherState_onFinisherActionFinished;
        }


        private void PlayerFinisherState_OnFinisherActionStarted(Transform obj)
        {
            playerInput.Player.Disable();
            playerInput.UINavigation.Disable();
        }

        private void PlayerFinisherState_onFinisherActionFinished()
        {
            playerInput.Player.Enable();
            playerInput.UINavigation.Enable();
        }

        // Update is called once per frame
        void Update()
        {
            ChangeStance();

            if (nextItemTimer > 0)
            {
                nextItemTimer -= 1 * Time.deltaTime;
            }
            if (canStartTimer == true)
            {
                selectionTimer -= 1 * Time.deltaTime;
            }
            if (selectionTimer <= 0)
            {
                ChosenStance(currentStance);

                canStartTimer = false;
                selectionTimer = 0.9f;
                currentStance = 0; // Create UI menu option to toggle reset or continuous cycle so players can choose what they like best 
            }
        }


        private void SelectStance(int _index)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i == _index);
            }
        }

        private void ChangeStance()
        {
            if (machine.InputReader.RightStanceEnabled && nextItemTimer <= 0)
            {
                currentStance += 1;
                SelectStance(currentStance);
                if (currentStance > transform.childCount - 2)
                {
                    currentStance = -1;
                }
                nextItemTimer = 0.18f;
                selectionTimer = 1f;
                canStartTimer = true;

            }
        }

        private void ChosenStance(int _chosenStance)
        {
            if (_chosenStance == 0)
            {
                machine.FighterMode();
                transform.GetChild(_chosenStance).gameObject.SetActive(false);
            }
            else if (_chosenStance == 1)
            {
                machine.GunMode();
                transform.GetChild(_chosenStance).gameObject.SetActive(false);

            }
            else if (_chosenStance == 2)
            {
                machine.GreatSwordMode();
                transform.GetChild(_chosenStance).gameObject.SetActive(false);

            }
            else if (_chosenStance == -1)
            {
                machine.AssasinMode();
                transform.GetChild(3).gameObject.SetActive(false);

            }
        }
        private void OnDisable()
        {
            PlayerFinisherState.OnFinisherActionStarted -= PlayerFinisherState_OnFinisherActionStarted;
            PlayerFinisherState.onFinisherActionFinished -= PlayerFinisherState_onFinisherActionFinished;
        }
    }

}

