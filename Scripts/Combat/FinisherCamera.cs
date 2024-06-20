using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;
using UnityEngine.Rendering.HighDefinition;

public class FinisherCamera : MonoBehaviour
{
    [SerializeField] private List<GameObject> finisherCamerasList;
    //Initalize finisher camera
    [SerializeField] private GameObject actionCameraGameObject;
    [SerializeField] private GameObject playerOverShoulderCamera;
    [SerializeField] private GameObject enemyPOVCamera;
    [SerializeField] private GameObject rotateAroundCamera;
    [SerializeField] private GameObject stateDrivenCamera;
    [SerializeField] private GameObject frameTargetGroup;
    [SerializeField] private GameObject frameTargetGroupCamera;
    private Transform tempTargetTransformData { set; get; } // cache current framed enemy in camera so that targeting can

    private bool isRotating;
    // work nicely.

    private bool finisherIsStillActive = false;
    private void Awake()
    {
        actionCameraGameObject.GetComponent<CinemachineVirtualCamera>().Follow = this.transform;
    }
    private void OnEnable()
    {
        PlayerFinisherState.OnFinisherActionStarted += PlayerFinisherState_OnFinisherActionStarted;
        PlayerFinisherState.onFinisherActionFinished += PlayerFinisherState_onFinisherActionFinished;
    }
    private void PlayerFinisherState_OnFinisherActionStarted(Transform EnemyPosition)
    {
        if (EnemyPosition != null)
        {
            AssignCameraFocus(EnemyPosition);
        }
        finisherIsStillActive = true;
        ShowFinisherCamera();
        
    }

    private void PlayerFinisherState_onFinisherActionFinished()
    {
        ClearUpCameraFocus();
        finisherIsStillActive = false;
        HideFinisherCamera();
    }
    private void OnDisable()
    {
        PlayerFinisherState.OnPlayerFinisherCamera -= PlayerFinisherState_OnFinisherActionStarted;
        PlayerFinisherState.OnPlayerFinisherCameraEnd -= PlayerFinisherState_onFinisherActionFinished;
    }
    private void AssignCameraFocus(Transform position)
    {
        actionCameraGameObject.GetComponent<CinemachineVirtualCamera>().Follow = position;
        actionCameraGameObject.GetComponent<CinemachineVirtualCamera>().LookAt = position;

        playerOverShoulderCamera.GetComponent<CinemachineVirtualCamera>().Follow = this.transform;
        playerOverShoulderCamera.GetComponent<CinemachineVirtualCamera>().LookAt = this.transform;

        enemyPOVCamera.GetComponent<CinemachineVirtualCamera>().Follow = this.transform;
        enemyPOVCamera.GetComponent<CinemachineVirtualCamera>().LookAt = this.transform;

        rotateAroundCamera.GetComponent<CinemachineVirtualCamera>().Follow = position; // have it go to a spot then set it to null so rotate around sets up nicely
        rotateAroundCamera.GetComponent<CinemachineVirtualCamera>().LookAt = this.transform;
        rotateAroundCamera.GetComponent<CinemachineVirtualCamera>().LookAt = null;

        tempTargetTransformData = position;
        frameTargetGroup.GetComponent<CinemachineTargetGroup>().AddMember(tempTargetTransformData, 1, 2);
        
    }
    private void Update()
    {
        if(isRotating == true)
        {
            Revolve(rotateAroundCamera.GetComponent<CinemachineVirtualCamera>());
        }
        
    }
    private void ClearUpCameraFocus()
    {

        foreach (GameObject camera in finisherCamerasList)
        {
            camera.GetComponent<CinemachineVirtualCamera>().Follow = null;
            camera.GetComponent<CinemachineVirtualCamera>().LookAt = null;
        }
        actionCameraGameObject.GetComponent<CinemachineVirtualCamera>().Follow = transform;
        actionCameraGameObject.GetComponent<CinemachineVirtualCamera>().LookAt = null;
    }
    private void ShowFinisherCamera()
    {
        stateDrivenCamera.SetActive(false);
        actionCameraGameObject.GetComponent<CinemachineVirtualCamera>().Priority = 25;
        foreach (GameObject camera in finisherCamerasList)
        {
            camera.SetActive(true);
            camera.GetComponent<CinemachineVirtualCamera>().Priority = 1;
        }
    }

    public void ShowOverShoulderCamera()
    {
        playerOverShoulderCamera.GetComponent<CinemachineVirtualCamera>().Priority = 25;
        foreach (GameObject camera in finisherCamerasList)
        {
            if(camera.name != playerOverShoulderCamera.name)
            {
                camera.GetComponent<CinemachineVirtualCamera>().Priority = 1;
            }
        }
    }
    public void ShowEnemyPOVCamera()
    {
        enemyPOVCamera.GetComponent<CinemachineVirtualCamera>().Priority = 25;
        foreach (GameObject camera in finisherCamerasList)
        {
            if (camera.name != enemyPOVCamera.name) {
                camera.GetComponent<CinemachineVirtualCamera>().Priority = 1;
            }
        }
    }
    public void ShowRotateAroundCamera()
    {
        isRotating = true;
        rotateAroundCamera.GetComponent<CinemachineVirtualCamera>().Priority = 25;
       // Revolve(rotateAroundCamera.GetComponent<CinemachineVirtualCamera>());
        foreach (GameObject camera in finisherCamerasList)
        {
            if (camera.name != rotateAroundCamera.name)
            {
                camera.GetComponent<CinemachineVirtualCamera>().Priority = 1;
            }
        }
        
    }

    public void ShowTargetGroupCamera()
    {
        frameTargetGroupCamera.GetComponent<CinemachineVirtualCamera>().Priority = 25;
        foreach (GameObject camera in finisherCamerasList)
        {
            camera.GetComponent<CinemachineVirtualCamera>().Priority = 1;
        }
    }

    private void HideFinisherCamera()
    {
        isRotating = false;
        stateDrivenCamera.SetActive(true);
        frameTargetGroup.GetComponent<CinemachineTargetGroup>().RemoveMember(tempTargetTransformData);
        actionCameraGameObject.GetComponent<CinemachineVirtualCamera>().Priority = 1;
        foreach (GameObject camera in finisherCamerasList)
        {
            camera.GetComponent<CinemachineVirtualCamera>().Priority = 1;
        }
    }
    // public functions useful for changing speed during a finisher
    public void SlowMotionSpeed()
    {
        Time.timeScale = 0.5f;
    }
    public void Revolve(CinemachineVirtualCamera objectTORotate)
    {
        if (objectTORotate.isActiveAndEnabled)
        {
            objectTORotate.transform.RotateAround(transform.position, Vector3.up, 90 * Time.unscaledDeltaTime);
        }
    }
    public void SlowMotionQuarterSpeed()
    {
        Time.timeScale = 0.25f;
    }
    public void ReturnToRegularSpeed()
    {
        Time.timeScale = 1;
    }
}
