using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public AudioClip clip;

    void OnTriggerEnter(Collider player)
    {
        if(player.CompareTag("Player"))
        {
            player.GetComponent<AudioSource>().PlayOneShot(clip, 1);
        }
    }
}
