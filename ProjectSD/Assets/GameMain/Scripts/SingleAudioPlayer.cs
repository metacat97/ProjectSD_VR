using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleAudioPlayer : MonoBehaviour
{
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        Destroy(gameObject, audioSource.clip.length + 1);
        audioSource.Play();
    }
}

