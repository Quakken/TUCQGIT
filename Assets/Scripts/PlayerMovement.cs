/*
 * Name: Quinn Farrell
 * Date: 11/28/2023
 * Desc: A script to be attached to any player object that moves. Takes in player input and translates it to movement. Relies on a rigidbody/rigidbody2D. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    enum MoveDimensions { X, Y, XY };

    [Header("Config")]
    [Tooltip("Is the player using 2D physics?")]
    [SerializeField] bool twoD = true;

    [Space]
    [Tooltip("The axis the player should be able to move along")]
    [SerializeField] MoveDimensions moveDimension;
    [Tooltip("How fast the player moves")]
    [SerializeField] float movementSpeed;

    Rigidbody rb;
    Rigidbody2D rb2D;

    private void Start()
    {
        // If no rigidbody is attached to the player object, add one 

        rb = GetComponent<Rigidbody>();
        rb2D = GetComponent<Rigidbody2D>();

        if (rb == null && rb2D == null)
        {
            if (twoD)
                rb2D = gameObject.AddComponent<Rigidbody2D>();
            else
                rb = gameObject.AddComponent<Rigidbody>();
        }
    }
    
    private void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Moves the player based on their input. This is messy and ugly, but it should theorhetically work

        if (twoD)
        {
            switch (moveDimension)
            {
                case MoveDimensions.X:
                    rb2D.velocity = new Vector2(input.x * movementSpeed, 0);
                    break;
                case MoveDimensions.Y:
                    rb2D.velocity = new Vector2(0, input.y * movementSpeed);
                    break;
                case MoveDimensions.XY:
                    rb2D.velocity = input.normalized * movementSpeed;
                    break;
            }
        }
        else
        {
            switch (moveDimension)
            {
                case MoveDimensions.X:
                    rb.velocity = new Vector3(input.x * movementSpeed, 0, 0);
                    break;
                case MoveDimensions.Y:
                    rb.velocity = new Vector3(0, input.y * movementSpeed, 0);
                    break;
                case MoveDimensions.XY:
                    rb.velocity = (Vector3)input.normalized * movementSpeed;
                    break;
            }
        }
    }
}
