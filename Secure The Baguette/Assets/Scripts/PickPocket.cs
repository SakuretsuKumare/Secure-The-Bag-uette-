using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickPocket : MonoBehaviour
{
    public AudioSource playerAudio;
    public AudioClip pickPocketSound;
    public GameObject player;
    public GameObject key;
    public Text keyCardText;
    public float distance;
    public bool noKey;

    void Start()
    {
        player = GameObject.Find("Player");
        playerAudio = player.GetComponent<AudioSource>();
        playerAudio.clip = pickPocketSound;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= distance)
        {
            keyCardText.text = "Press E to pickpocket";
        }
        else
        {
            if (Vector3.Distance(transform.position, player.transform.position) > distance)
            {
                keyCardText.text = "Key Card";
            }
        }

        if (Vector3.Distance(transform.position, player.transform.position) <= distance && Input.GetKeyDown(KeyCode.E))
        {
            playerAudio.PlayOneShot(pickPocketSound, 1);
            Instantiate(key, player.transform.position, player.transform.rotation);
            noKey = true;
            keyCardText.gameObject.SetActive(false);
        }

        if (noKey)
        {
            keyCardText.gameObject.SetActive(false);
            transform.GetComponent<PickPocket>().enabled = false;
        }
    }
}