using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] KeyScript.KeyAccess keyAccess;

    public KeyScript.KeyAccess GetKeyAccess()
    {
        return keyAccess;
    }

    public void OpenDoor()
    {
        gameObject.SetActive(false);
    }
}