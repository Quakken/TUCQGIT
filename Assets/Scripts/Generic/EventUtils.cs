/*
 * Name: Quinn Farrell
 * Date: 11/28/2023
 * Desc: A collection of functions to be used with unity events.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventUtils : MonoBehaviour
{
    public float shakeDuration;

    public void SpawnAtPosition(GameObject toSpawn)
    {
        Instantiate(toSpawn, transform.position, toSpawn.transform.rotation);
    }

    public void ShakeScreen(float shakeMag)
    {
        Camera.main.GetComponent<CameraFollow>().ShakeScreen(shakeDuration, shakeMag);
    }

    public void LoadScene(string toLoad)
    {
        SceneLoader.LoadScene(toLoad);
    }

    public void UpdateQuitState(bool newState)
    {
        TrapUser.UpdateQuitState(newState);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
