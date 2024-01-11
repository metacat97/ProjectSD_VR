using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemhitSound : MonoBehaviour
{
    private AudioSource myAudio = null;

    private void Awake()
    {
        myAudio = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            myAudio.Play();
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
    //    {
    //        myAudio.Play();
    //    }
    //}

}
