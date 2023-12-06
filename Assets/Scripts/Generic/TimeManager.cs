/*
 * Name: Quinn Farrell
 * Date: 10/2/2023
 * Description: A basic script put on any game object in the level which handles things like sleep & time slow. Also lets you 
 * manually set time scale if that's something you want to do.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Congig")]
    [Tooltip("How fast the game should normally be running")]
    [SerializeField] float defaultTimeScale = 1;

    //Allows this script to be accessed by others without having to find the specific game object
    public static TimeManager instance;

    float sleepDuration;
    float slowDuration;
    float slowFactor;

    float timeScale;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        timeScale = defaultTimeScale;
    }

    private void Update()
    {
        if (sleepDuration >= 0)
        {
            Time.timeScale = 0;

            //Decrease the sleep duration & set the time back to normal if it is 0
            sleepDuration -= Time.unscaledDeltaTime;
            if (sleepDuration <= 0)
                Time.timeScale = timeScale;
        }

        if (slowDuration >= 0)
        {
            Time.timeScale = slowFactor;
            slowDuration -= Time.unscaledDeltaTime;

            if (slowDuration <= 0)
                Time.timeScale = timeScale;
        }
    }

    // Set the time scale to a specific value
    public void SetTimeScale(float scale)
    {
        // Store the current default time scale
        timeScale = scale;

        // Set the time scale if not currently slowed or sleeping
        if (sleepDuration <= 0 && slowDuration <= 0)
            Time.timeScale = timeScale;
    }

    // Sets the time scale to its default value
    public void ResetTimeScale()
    {
        timeScale = defaultTimeScale;

        // Set the time scale if not currently slowed or sleeping
        if (sleepDuration <= 0 && slowDuration <= 0)
            Time.timeScale = timeScale;
    }

    // "Sleeps" the game, basically just freezing it, for a specific duration
    public void Sleep(float duration)
    {
        if (sleepDuration < duration)
            sleepDuration = duration;
    }

    // Slows the game for a specific duration
    public void SlowTime(float duration, float factor)
    {
        if (Time.timeScale > factor)
            slowFactor = factor;
        if (slowDuration < duration)
            slowDuration = duration;
    }
}