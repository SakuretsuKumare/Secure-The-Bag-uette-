using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickPocket : MonoBehaviour
{

    public GameObject player;
    public GameObject key;
    public Text keyCardText;
    public bool noKey;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= 3)
        {
            keyCardText.text = "Press E to pickpocket";
        }
        else
        {
            if (Vector3.Distance(transform.position, player.transform.position) > 3)
            {
                keyCardText.text = "Key Card";
            }
        }

        if (Vector3.Distance(transform.position, player.transform.position) <= 3 && Input.GetKeyDown(KeyCode.E))
        {
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
