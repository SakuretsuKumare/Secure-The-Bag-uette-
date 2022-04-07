using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [HideInInspector] public static MusicManager singleton;
    public AudioSource[] listOfAudioSources;

    void Awake()
    {
        singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (AudioSource i in listOfAudioSources)
        {
            i.volume = 0;
        }

        if (listOfAudioSources[0].volume == 0)
        {
            listOfAudioSources[0].volume = 1;
        }
    }

    public void FadeIn(float volume, int layer)
    {
       listOfAudioSources[layer].volume = volume;
    }
}