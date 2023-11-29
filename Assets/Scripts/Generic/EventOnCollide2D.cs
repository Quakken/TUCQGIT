/*
 * Name: Quinn Farrell
 * Date: 11/27/2023
 * Desc: A script to be attached to any game object. Relies on a collider2D and a rigidbody2D. When the gameobject collides with something that has one of the specified tags,
 * it invokes a unityevent. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class EventOnCollide2D : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("A list of all tags that will trigger the OnCollision event")]
    [SerializeField] string[] tags;
    [Header("Events")]
    [Tooltip("Event called when the object collides with something else")]
    [SerializeField] UnityEvent OnCollision;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (collision.transform.CompareTag(tags[i]))
            {
                OnCollision.Invoke();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (collision.transform.CompareTag(tags[i]))
            {
                OnCollision.Invoke();
            }
        }
    }
}
