using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardVoiceLines : MonoBehaviour
{
    public AudioClip[] clip;
    private int voiceLineRandomizer;
    private float waitSeconds;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("VoiceLine");
    }

    public IEnumerator VoiceLine()
    {
        waitSeconds = Random.Range(9, 25);
        yield return new WaitForSeconds(waitSeconds);
        voiceLineRandomizer = Random.Range(0, clip.Length);
        gameObject.GetComponent<AudioSource>().PlayOneShot(clip[voiceLineRandomizer], 1);
        StartCoroutine("VoiceLine");
    }
}