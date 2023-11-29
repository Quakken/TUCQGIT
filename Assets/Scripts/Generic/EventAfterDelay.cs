/*
 * Name: Quinn F
 * Date: 11/29/2023
 * Desc: A script that has a function that calls an event after a specified duration
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventAfterDelay : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("How long to wait before calling the event")]
    [SerializeField] float delay;
    [Tooltip("Starts the timer when the object is loaded into the scene")]
    [SerializeField] bool startOnAwake;

    [Header("Events")]
    [SerializeField] UnityEvent timedEvent;

    public void CallEvent()
    {
        Invoke("InvokeEvent", delay);
    }

    void InvokeEvent()
    {
        timedEvent.Invoke();
    }
}
