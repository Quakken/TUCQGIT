/*
 * Name: Quinn Farrell
 * Date: 12/1/2023
 * Desc: A script to be attached to any of the food objects in the food minigame. It needs to be specific enough to warrant its own class, otherwise this wouldn't exist.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [Header("Config")]
    public bool isFake = true;

    // When the player clicks on the food, let the shopping manager know
    private void OnMouseDown()
    {
        ShoppingManager.instance.CheckFood(this);
    }
}
