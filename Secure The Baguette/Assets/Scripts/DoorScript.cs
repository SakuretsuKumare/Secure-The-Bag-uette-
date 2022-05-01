using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parts of this script was used from Code Monkey https://youtu.be/MIt0PJHMN5Y?t=638

public class DoorScript : MonoBehaviour
{
    public float doorSpeed;
    public float doorLiftedAmount;
    public bool accessGranted;
    public AudioSource doorAudio;
    public AudioClip[] clips;
    [SerializeField] private bool close;
    [SerializeField] private Vector3 movedUp;
    [SerializeField] private Vector3 origin;
    private GameObject player;
    [SerializeField] KeyScript.KeyAccess keyAccess;

    void Start()
    {
        doorAudio = transform.root.GetComponent<AudioSource>();
        player = GameObject.Find("Player");
        origin = transform.position;
        movedUp = transform.position + new Vector3(0, doorLiftedAmount, 0);
    }

    void Update()
    {
        if (accessGranted && !close && Vector3.Distance(transform.root.position, movedUp) > Mathf.Epsilon)
        {
            transform.root.position = Vector3.MoveTowards(transform.root.position, movedUp, doorSpeed * Time.deltaTime);
        }

        if (close && accessGranted && Vector3.Distance(transform.root.position, origin) > Mathf.Epsilon)
        {
            transform.root.position = Vector3.MoveTowards(transform.root.position, origin, doorSpeed * Time.deltaTime);
        }

    }

    public KeyScript.KeyAccess GetKeyAccess()
    {
        return keyAccess;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (accessGranted)
            {
                close = false;
            }
            else
            {
                doorAudio.clip = clips[0];
                doorAudio.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && accessGranted)
        {
            close = true;
        }
    }
}