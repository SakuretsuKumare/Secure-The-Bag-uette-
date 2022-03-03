using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportWait : MonoBehaviour
{
    private GameObject player;
    private CharacterMovement characterMovement;
    public Transform teleportTo;
    public float waitSeconds;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        characterMovement = player.GetComponent<CharacterMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine("TeleportPlayer");
        }
    }

    IEnumerator TeleportPlayer()
    {
        characterMovement.disabled = true;
        yield return new WaitForSeconds(0.1f);
        // Sets the player's transform
        player.transform.position = teleportTo.transform.position;
        player.transform.rotation = teleportTo.transform.rotation;
        yield return new WaitForSeconds(0.1f);
        characterMovement.disabled = false;
        characterMovement.playerSpawnPoint = teleportTo.transform.position;
        characterMovement.playerSpawnRotation = teleportTo.transform.rotation;
    }
}