/*
 * Name: Quinn Farrell
 * Date: 11/29/2023
 * Desc: Basic script, has a function to play a sound. Just attach it to something that you want to play a sound and then call the function
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] AudioSource source;

    public void PlayAudioClip(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }
}
