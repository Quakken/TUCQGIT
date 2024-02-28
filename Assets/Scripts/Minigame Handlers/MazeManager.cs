/*
 * Name: Quinn Farrell
 * Date: 2/28/2024
 * Desc: Handles the usage of the "bromance ending" in the maze minigame. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MazeManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent OnBromance;
    [SerializeField] private UnityEvent OnNormal;
    
    public static bool secretEnding;

    // Called by an external event when the end scene trigger is touched
    public void OnLevelComplete()
    {
        if (secretEnding)
            OnBromance.Invoke();
        else
            OnNormal.Invoke();
    }
}
