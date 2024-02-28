/*
 * Name: Quinn Farrell
 * Date: 2/27/2024
 * Desc: Manages the user's ability to quit the game. It's not malware i promise.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TrapUser
{
    private static bool canQuit = true;

    public static void UpdateQuitState(bool state)
    {
        canQuit = state;
    }

    // Called whenever the application tries to quit. If it returns false, then the quit message is discarded. 
    static bool WantsToQuit()
    {
        return canQuit;
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        Application.wantsToQuit += WantsToQuit;
    }
}
