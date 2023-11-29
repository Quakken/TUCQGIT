/*
 * Name: Quinn Farrell
 * Date: 11/28/2023
 * Desc: A script that will destroy the attached gameobject after a set duration
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedDestruction : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("How long it takes for the object to self destruct")]
    [SerializeField] float destroyTime = 3;
    [Tooltip("How long after the destroy time it takes for the object to actually destroy itself")]
    [SerializeField] float destroyDelay = 0;

    [Header("Events")]
    [SerializeField] UnityEvent onDestroy;

    float destroyTimer;

    private void Start()
    {
        destroyTimer = destroyTime;
    }

    private void Update()
    {
        destroyTimer -= Time.deltaTime;

        if (destroyTimer <= 0)
        {
            onDestroy.Invoke(); 
            Destroy(gameObject, destroyDelay);
        }
    }
}
