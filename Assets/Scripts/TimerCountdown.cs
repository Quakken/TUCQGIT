/*
* Name: Alice Pocek
* Date: 11/28/2023
* Description: A timer that counts down from 1:30 to 0 seconds
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;

public class TimerCountdown : MonoBehaviour
{
    public float secondsRemaining = 180;
    public bool timerStarts = false;
    public TextMeshProUGUI timeText;
    public UnityEvent OnTimerFinish;

    private void Start()
    {
        timerStarts = true;
    }

    void Update()
    {
        if (timerStarts)
        {
            if (secondsRemaining > 0)
            {
                secondsRemaining -= Time.deltaTime;
            }
            else
            {
                //the dialogue can be changed later
                Debug.Log("You made it!");
                OnTimerFinish.Invoke();
                secondsRemaining = 0;
                timerStarts = false;
            }

            timerDisplay(secondsRemaining);
        }
    }

    void timerDisplay(float secondsDisplayed)
    {
        //converting the minutes and seconds
        secondsDisplayed += 1;
        float minutes = Mathf.FloorToInt(secondsDisplayed / 60);
        float seconds = Mathf.FloorToInt(secondsDisplayed % 60);
        timeText.text = string.Format(minutes + ":" + seconds.ToString("00"));
    }

    public void StopTimer()
    {
        timerStarts = false;
    }
}