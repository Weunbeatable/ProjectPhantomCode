/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using UnityEngine;

public class FootStepManager : MonoBehaviour
{
    [Header("FootStepAudio")]
    [SerializeField] public AudioClip[] walkAudioarray;
    [SerializeField] public AudioClip[] runAudioArray;
    public AudioSource playerSource;


    private void OnEnable()
    {
        PlayerFreeLookState.OnTriggerFootsteps += PlayerFreeLookState_OnTriggerFootsteps;

    }


    private void OnDisable()
    {
        PlayerFreeLookState.OnTriggerFootsteps -= PlayerFreeLookState_OnTriggerFootsteps;
    }
    void Start()
    {
        playerSource.GetComponent<AudioSource>();
    }

    private void PlayerFreeLookState_OnTriggerFootsteps(bool isWalking, bool isRunning)
    {
        if (isWalking == true && isRunning == false)
        {
            AudioClip walkSound = walkAudioarray[UnityEngine.Random.Range(1, walkAudioarray.Length - 1)];
            if (walkAudioarray.l != null)
            {
                if (playerSource.isPlaying == true)
                {
                    return;
                }
                else
                {
                    playerSource.PlayOneShot(walkSound);
                }

            }
        }
        else if (isWalking == false && isRunning == true)
        {
            AudioClip runsound = walkAudioarray[UnityEngine.Random.Range(1, runAudioArray.Length - 1)];
            if (runAudioArray != null)
            {
                if (playerSource.isPlaying == true)
                {
                    return;
                }
                else
                {

                    playerSource.PlayOneShot(runsound);
                }

            }
        }
    }
}
*/