using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script was used from Code Monkey https://youtu.be/MIt0PJHMN5Y?t=638

public class DoorScript : MonoBehaviour
{
    public float doorSpeed;
    public float doorLiftedAmount;
    public float openingDistance;
    public float closingDistance;
    public bool goingUp;
    public bool stayOpen;
    public bool moving;
    public bool accessGranted;
    public AudioSource doorAudio;
    public AudioClip[] clips;
    private bool cooledDown;
    private bool callOnce;
    private Vector3 movedUp;
    private Vector3 origin;
    private GameObject player;
    [SerializeField] KeyScript.KeyAccess keyAccess;
    //private Animator animator;

    void Start()
    {
        doorAudio = gameObject.GetComponent<AudioSource>();
        player = GameObject.Find("Player");
        origin = transform.position;
        movedUp = transform.position + new Vector3(0, doorLiftedAmount, 0);
    }

    void Update()
    {
        if (accessGranted)
        {
            if (Vector3.Distance(transform.root.position, player.transform.position) <= openingDistance)
            {
                if (!moving)
                {
                    OpenDoorAccepted();
                }
                moving = true;
                cooledDown = false;
                goingUp = true;
            }

            if (Vector3.Distance(transform.root.position, player.transform.position) >= closingDistance && Vector3.Distance(transform.root.position, origin) > Mathf.Epsilon)
            {
                if (!moving)
                {
                    goingUp = false;
                }

                if (!callOnce && !cooledDown && Vector3.Distance(transform.root.position, origin) > Mathf.Epsilon)
                {
                    callOnce = true;
                    StartCoroutine(CoolDown());
                }
            }

            if (goingUp && !stayOpen)
            {
                transform.root.position = Vector3.MoveTowards(transform.root.position, movedUp, doorSpeed * Time.deltaTime);
            }

            else
            {
                if (goingUp && moving)
                {
                    transform.root.position = Vector3.MoveTowards(transform.root.position, movedUp, doorSpeed * Time.deltaTime);
                }
            }

            if (!goingUp && !stayOpen)
            {
                transform.root.position = Vector3.MoveTowards(transform.root.position, origin, doorSpeed * Time.deltaTime);
            }

            else
            {
                if (!goingUp && moving)
                {
                    transform.root.position = Vector3.MoveTowards(transform.root.position, origin, doorSpeed * Time.deltaTime);
                }
            }

            if (Vector3.Distance(transform.root.position, origin) < Mathf.Epsilon)
            {
                cooledDown = false;
            }

            if (Vector3.Distance(transform.root.position, movedUp) < Mathf.Epsilon)
            {
                moving = false;
            }

            else
            {
                if (Vector3.Distance(transform.root.position, origin) > Mathf.Epsilon)
                {
                    moving = true;
                }
            }

            if (Vector3.Distance(transform.root.position, origin) < Mathf.Epsilon)
            {
                moving = false;
            }

            else
            {
                if (Vector3.Distance(transform.root.position, movedUp) > Mathf.Epsilon)
                {
                    moving = true;
                }
            }
        }
    }

    public KeyScript.KeyAccess GetKeyAccess()
    {
        return keyAccess;
    }

    public void OpenDoor()
    {
        //gameObject.SetActive(false);
        //animator.SetBool("DoorOpening", true);
        accessGranted = true;
        transform.root.GetComponent<DoorScript>().enabled = true;
        transform.root.GetComponent<DoorScript>().accessGranted = true;
    }

    public void OpenDoorAccepted()
    {
        doorAudio.PlayOneShot(clips[1], 1);
    }

    public void OpenDoorDenied()
    {
        doorAudio.clip = clips[0];
        doorAudio.Play();
    }

    IEnumerator CoolDown()
    {
        stayOpen = true;
        yield return new WaitForSeconds(2f);
        stayOpen = false;
        callOnce = false;
        cooledDown = true;
    }
}