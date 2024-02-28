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
    private static int flowersCollected;

    public void OnFlowerCollected()
    {
        flowersCollected += 1;
        if (flowersCollected == 10)
        {
            MazeManager.secretEnding = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnFlowerCollected();
        }
    }
}