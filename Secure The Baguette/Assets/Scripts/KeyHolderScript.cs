using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script was used from Code Monkey https://youtu.be/MIt0PJHMN5Y?t=638

public class KeyHolderScript : MonoBehaviour
{
    public event EventHandler OnKeysChanged;
    private List<KeyScript.KeyAccess> keyList;

    private void Awake()
    {
        keyList = new List<KeyScript.KeyAccess>();
    }

    public List<KeyScript.KeyAccess> GetKeyList()
    {
        return keyList;
    }

    // Add a key
    public void AddKey(KeyScript.KeyAccess keyAccess)
    {
        Debug.Log("Added Key: " + keyAccess);
        keyList.Add(keyAccess);
        OnKeysChanged?.Invoke(this, EventArgs.Empty);
    }

    // Remove a key
    public void RemoveKey(KeyScript.KeyAccess keyAccess)
    {
        keyList.Remove(keyAccess);
        OnKeysChanged?.Invoke(this, EventArgs.Empty);
    }

    // If it contains a key.
    public bool ContainsKey(KeyScript.KeyAccess keyAccess)
    {
        return keyList.Contains(keyAccess);
    }

    private void OnTriggerEnter(Collider collider)
    {
        KeyScript key = collider.GetComponent<KeyScript>();
        if (key != null)
        {
            AddKey(key.GetKeyAccess());
            Destroy(key.gameObject);
        }

        DoorScript doorScript = collider.GetComponent<DoorScript>();
        if (doorScript != null)
        {
            if (ContainsKey(doorScript.GetKeyAccess()))
            {
                // Currently holding the Key to open the right door.
                //RemoveKey(doorScript.GetKeyAccess());
                doorScript.OpenDoor();
            }
        }
    }
}