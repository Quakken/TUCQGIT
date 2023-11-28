/*
 * Name: Quinn Farrell
 * Date: 11/27/2023
 * Desc: A script to be attached to any game object. Will move the afforementioned game object in a specified direction at a certain speed for all of eternity.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInDirection : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The direction to move the object")]
    [SerializeField] Vector3 direction;
    [Tooltip("The speed at which the object should move")]
    [SerializeField] float moveSpeed;

    private void Update()
    {
        // Moves the object in the specified direction
        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
    }
}
