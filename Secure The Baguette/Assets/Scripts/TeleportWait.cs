using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportWait : MonoBehaviour
{
    private GameObject player;
    private CharacterMovement characterMovement;
    public Transform teleportTo;
    public float waitSeconds = 1f;
    public int setAudioLayer;
    private Image blackFadingScreen;
    private float speed = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        characterMovement = player.GetComponent<CharacterMovement>();
        blackFadingScreen = GameObject.Find("Fading Screen").GetComponent<Image>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.GetComponent<CharacterMovement>().accessGranted = false;
            player.GetComponent<CharacterMovement>().totalRecipesCollected = player.GetComponent<CharacterMovement>().totalRecipesCollected + player.GetComponent<CharacterMovement>().levelRecipesCollected.Count;
            player.GetComponent<CharacterMovement>().levelRecipesCollected.Clear();
            StartCoroutine("FadeIn");
            StartCoroutine("TeleportPlayer");
        }
    }

    IEnumerator TeleportPlayer()
    {
        characterMovement.disabled = true;
        yield return new WaitForSeconds(waitSeconds);
        // Sets the player's transform
        player.transform.position = teleportTo.transform.position;
        player.transform.rotation = teleportTo.transform.rotation;
        yield return new WaitForSeconds(0.1f);
        characterMovement.disabled = false;
        characterMovement.playerSpawnPoint = teleportTo.transform.position;
        characterMovement.playerSpawnRotation = teleportTo.transform.rotation;
        MusicManager.singleton.FadeIn(1, setAudioLayer);
    }

    /* This creates a black fade in effect in the UI.
    Part of the code was used from https://www.youtube.com/watch?v=6WmF7bNACKo&t=714s */
    IEnumerator FadeIn()
    {
        //blackFadingScreen.enabled = true;
        float alpha = blackFadingScreen.color.a;

        while (alpha < 1)
        {
            alpha += Time.deltaTime * speed;
            blackFadingScreen.color = new Color(blackFadingScreen.color.r, blackFadingScreen.color.g, blackFadingScreen.color.b, alpha);
            yield return null;
        }
        
        yield return new WaitForSeconds(1f);

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * speed;
            blackFadingScreen.color = new Color(blackFadingScreen.color.r, blackFadingScreen.color.g, blackFadingScreen.color.b, alpha);
            yield return null;
        }

        yield return null;
    }
}