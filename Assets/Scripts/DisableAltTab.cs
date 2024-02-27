/*
 * Name: Quinn Farrell
 * Date: 2/27/2024
 * Desc: I am prefacing this by saying that this is not malware. It does not actually disable alt+f4 because unity doesn't have the permission to change window focus.
 *       Instead it just blinks a bunch until you switch back. Either way it's just sort of here to encourage the player to click back.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DisableAltTab : MonoBehaviour
{
    IntPtr _hWnd_Unity;
    bool focused = true;

    private void Awake()
    {
        _hWnd_Unity = (IntPtr)GetActiveWindow();
    }

    private void OnApplicationFocus(bool focus)
    {
        focused = focus;

        if (!focus)
        {
            StartCoroutine(TryFocus());
        }
    }

    IEnumerator TryFocus()
    {
        while (!focused)
        {
            SetForegroundWindow(_hWnd_Unity);
            print("trying focus");
            yield return null;
        }
    }

    [DllImport("user32.dll")] static extern uint GetActiveWindow();
    [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr hWnd);
}
