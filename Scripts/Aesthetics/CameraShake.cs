using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// From CodeMonkeys How to do CameraShake with Cinemachine YT video!
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }    
    private CinemachineVirtualCamera virtualCamera;
    private float shakeTimer;

    private void Awake()
    {
        Instance = this;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    // Start is called before the first frame update
public void ShakeCamera(float intensity, float time)
    {
         CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }

    private void Update()
    {
        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if(shakeTimer <= 0f)
            {
                //Time is up
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
           virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }
    }
}
