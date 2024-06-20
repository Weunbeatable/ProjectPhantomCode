using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStop : MonoBehaviour
{
    //This script is modified from a tutorial video designed by Slug glove on YT
    private float speed;
    private bool restoreTime;
    // floats will control how fast we stop time and when we restore it

    void Start()
    {
        restoreTime = false;   
    }

    // Update is called once per frame
    void Update()
    {
        if (restoreTime)
        {
            if(Time.timeScale < 1f)
            {
                Time.timeScale += Time.deltaTime * speed; // increase time
            }
            else
            {
                Time.timeScale = 1f;
                restoreTime = false; //restore time back to state and stop function
            }
        }
    }

    public void StopTime(float changeTime, int restoreSpeed, float delay)
        // float is for what time dialation to change to, int is how quick to restore time and float is for delay of game.
    {
        //speed is set to restore time value
        //once delay is set a coroutine starts to delay script
        //otherwise it will start automatically.
        speed = restoreSpeed;
        if(delay > 0)
        {
            StopCoroutine(StartTimeAgain(delay));
            StartCoroutine(StartTimeAgain(delay));
        }
        else
        {
            restoreTime = true;
        }
        Time.timeScale = changeTime;
    }

    private IEnumerator StartTimeAgain(float delay)
    {
        restoreTime = true;
        yield return new WaitForSecondsRealtime(delay);
        // waitForSEcondsRealTime is used so slower time delay doesn't interrupt function
    }
}
