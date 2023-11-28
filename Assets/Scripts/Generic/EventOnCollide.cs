/*
 * Name: Quinn Farrell
 * Date: 11/27/2023
 * Desc: A script to be attached to any game object. Relies on a collider and a rigidbody. When the gameobject collides with something that has one of the specified tags,
 * it invokes a unityevent. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class EventOnCollide : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("A list of all tags that will trigger the OnCollision event")]
    [SerializeField] string[] tags;
    [Header("Events")]
    [Tooltip("Event called when the object collides with something else")]
    [SerializeField] UnityEvent onCollision;

    private void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (collision.transform.CompareTag(tags[i]))
            {
                onCollision.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (collision.transform.CompareTag(tags[i]))
            {
                onCollision.Invoke();
            }
        }
    }
}
