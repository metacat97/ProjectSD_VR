using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    AudioSource audioSource = default;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        transform.parent = null;    // 파괴되지 않도록 부모 끊기

        Destroy(gameObject, audioSource.clip.length + 1);
        audioSource.Play();
    }

}
