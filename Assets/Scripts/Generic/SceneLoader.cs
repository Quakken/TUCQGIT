/*
 * Name: Quinn Farrell
 * Date: 11/29/2023
 * Desc: Globally accessable class used to handle scene loading
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static void LoadScene(string toLoad)
    {
        SceneManager.LoadSceneAsync(toLoad);
    }
}
