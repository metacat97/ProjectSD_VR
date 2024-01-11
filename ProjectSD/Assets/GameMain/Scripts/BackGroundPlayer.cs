using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundPlayer : MonoBehaviour
{
    AudioSource audioSource;

    public AudioClip lobbySong;
    public AudioClip gameSong;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = lobbySong;
        audioSource.Play();
    }

    public void ChangeSong(bool isGame)
    {
        if (isGame)
        {
            audioSource.clip = gameSong;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = lobbySong;
            audioSource.Play();
        }
    }
}
