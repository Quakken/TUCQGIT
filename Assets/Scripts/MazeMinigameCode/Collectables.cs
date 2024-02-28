/*
Name: Alice Pocek
Date: 2/22/2024
Desc: A script in which keeps track of the amount of collectables and changes the scene accordingly
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public static bool allflowersCollected { get; private set; }

    private int targetFlowers = 10;

    private int flowersCollected;

    public static void OnFlowerCollected()
    {
        flowersCollected += 1;
        if (flowersCollected == 10)
        {
            allflowersCollected = true;
        }
    }

    void Start()
    {
        _charController = GetComponent<PlayerMovement>();

        if (_charController is null)
        {
            Debug.LogError("Player movement is NULL");
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnFlowerCollected();
        }
    }
}