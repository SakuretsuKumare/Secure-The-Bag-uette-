using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script was used from Code Monkey https://youtu.be/MIt0PJHMN5Y?t=638

public class KeyScript : MonoBehaviour
{
    [SerializeField] KeyAccess keyAccess;

    public enum KeyAccess
    {
        Level1,
        Level2,
        Level3
    }

    public KeyAccess GetKeyAccess()
    {
        return keyAccess;
    }
}