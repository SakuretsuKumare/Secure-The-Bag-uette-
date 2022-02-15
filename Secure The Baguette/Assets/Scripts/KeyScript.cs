using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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