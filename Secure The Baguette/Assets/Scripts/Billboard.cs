using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cameraToLookAt;

    // Start is called before the first frame update
    void Start()
    {
        cameraToLookAt = GameObject.Find("Main Camera").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + cameraToLookAt.forward);
    }
}