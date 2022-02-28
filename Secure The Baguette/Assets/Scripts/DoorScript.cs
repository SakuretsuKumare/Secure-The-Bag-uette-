using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script was used from Code Monkey https://youtu.be/MIt0PJHMN5Y?t=638

public class DoorScript : MonoBehaviour
{
    [SerializeField] KeyScript.KeyAccess keyAccess;
    private Animator animator;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public KeyScript.KeyAccess GetKeyAccess()
    {
        return keyAccess;
    }

    public void OpenDoor()
    {
        //gameObject.SetActive(false);
        animator.SetBool("DoorOpening", true);
    }
}