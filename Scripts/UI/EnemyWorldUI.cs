using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PFF.UI
{
    public class EnemyWorldUI : CharacterWorldUI
    {
        [SerializeField] List<Sprite> finisherImageIcons = new List<Sprite>();
        [SerializeField] GameObject FinishGameobject;
        [SerializeField] Image FinishIcon;
        [SerializeField] GameObject currentTargetIcon;
       // [SerializeField] Image detectionCone;
        public EnemyWorldUI(Image barImage, Health health, GameObject healthBarContainer) : base(barImage, health, healthBarContainer)
        {
        }
        private void Awake()
        {
      /*      Quaternion detectionDirection = new Quaternion(-90f, 0, 0f,0f);
            detectionCone.transform.rotation =  detectionDirection;*/
        }


        protected override void Start()
        {
            base.Start();
            healthSystem.onFinishable += HealthSystem_onFinishable;
            PlayerTargetingState.OnTriggerLockOnIcon += PlayerTargetingState_OnTriggerLockOnIcon;
            Targeter.OnUpdateLockOnIcon += Targeter_OnUpdateLockOnIcon;
            Targeter.OnTurnOffLockOnIcon += PlayerTargetingState_OnTurnOffLockOnIcon;
            // 3 main states for handling finisher icon and healthbar UI
            // you have no health so ther should be no icon or healthbar showing
            // you're health is critical so you should see both a finisher icon and your health
            // You're not in a critical state but you are alive so you can't see the finishable icon but you can see health
            EnemyFinishedState.onCleanupFinisherUI += EnemyFinishedState_onCleanupFinisherUI;
            EnemyDeadState.OnDead += EnemyDeadState_OnDead;

        }



        private void HealthSystem_onFinishable(bool obj)
        {
            InputReference.Instance.GetDeviceName();
            FinishGameobject.SetActive(obj);

            DetermineImageIconToDisplay();
        }

        private void EnemyFinishedState_onCleanupFinisherUI(object sender, EventArgs e)
        {
            if (healthSystem.getCurrentHealth() <= 0)
            {
                FinishGameobject.SetActive(false);
                healthBarContainer.gameObject.SetActive(false);
            }
            else if (healthSystem.getCurrentHealth() > 0 && healthSystem.CriticalHealthPercentage() == true)
            {
                FinishGameobject.SetActive(true);
                healthBarContainer.gameObject.SetActive(true);
            }
            else
            {
                FinishGameobject.SetActive(false);
                healthBarContainer.gameObject.SetActive(true);
            }
        }

        private void EnemyDeadState_OnDead(object sender, EventArgs e)
        {
            if (healthSystem.getCurrentHealth() <= 0)
            {
                FinishGameobject.SetActive(false);
                healthBarContainer.gameObject.SetActive(false);
            }
            else if (healthSystem.getCurrentHealth() > 0 && healthSystem.CriticalHealthPercentage() == true)
            {
                FinishGameobject.SetActive(true);
                healthBarContainer.gameObject.SetActive(true);
            }
            else
            {
                FinishGameobject.SetActive(false);
                healthBarContainer.gameObject.SetActive(true);
            }
        }


        private void DetermineImageIconToDisplay()
        {
            if (InputReference.Instance.GetDeviceName() == "Mouse" ||
                InputReference.Instance.GetDeviceName() == "Keyboard")
            {
                // grab the sprite and set the value equal to a specific array value depending on the input currently detected. 
                FinishIcon.sprite = finisherImageIcons[0];
            }
            else if (InputReference.Instance.GetDeviceName() == "DualSenseGamepadHID" ||
                     InputReference.Instance.GetDeviceName() == "DualShock4GamepadHID")
            {
                FinishIcon.sprite = finisherImageIcons[1];
            }
            else
            {
                FinishIcon.sprite = finisherImageIcons[2];
            }
        }

        private void PlayerTargetingState_OnTriggerLockOnIcon(Transform target)
        {
            if (this.transform.parent.name == target.name)
            {
                currentTargetIcon.SetActive(true);
            }
        }

        private void Targeter_OnUpdateLockOnIcon(Transform arg1, Transform arg2)
        {
            if (this.transform.parent.name == arg1.name)
            {
                currentTargetIcon.SetActive(false);
            }

            if (this.transform.parent.name.Equals(arg2.name))
            {
                currentTargetIcon.SetActive(true);
            }
        }
        private void PlayerTargetingState_OnTurnOffLockOnIcon()
        {
            if (this.transform.gameObject.activeSelf == true)
            {
                currentTargetIcon.SetActive(false);
            }
        }
        private void OnDestroy()
        {
            healthSystem.onFinishable -= HealthSystem_onFinishable;
            EnemyFinishedState.onCleanupFinisherUI -= EnemyFinishedState_onCleanupFinisherUI;
            Targeter.OnUpdateLockOnIcon -= Targeter_OnUpdateLockOnIcon;
            PlayerTargetingState.OnTriggerLockOnIcon -= PlayerTargetingState_OnTriggerLockOnIcon;
        }
    }
}