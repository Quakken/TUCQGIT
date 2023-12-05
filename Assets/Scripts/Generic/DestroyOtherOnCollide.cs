/*
 * Name: Quinn Farrell
 * Date: 11/30/2023
 * Desc: A script to attach to an object with a collider. When the object collides with something else, it will destroy the other object. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOtherOnCollide : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] LayerMask affectedLayers;

    private void OnCollisionEnter(Collision collision)
    {
        // Compare the other object's layer to the affected layers

        if (((1 << collision.gameObject.layer) & affectedLayers) != 0)
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Compare the other object's layer to the affected layers

        if (((1 << collision.gameObject.layer) & affectedLayers) != 0)
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        // Compare the other object's layer to the affected layers

        if (((1 << col.gameObject.layer) & affectedLayers) != 0)
        {
            Destroy(col.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Compare the other object's layer to the affected layers

        if (((1 << col.gameObject.layer) & affectedLayers) != 0)
        {
            Destroy(col.gameObject);
        }
    }
}
