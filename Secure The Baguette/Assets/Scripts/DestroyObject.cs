using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public AudioSource recipeAudio;
    public MeshRenderer recipeMesh;
    public GameObject recipeParticles;
    private bool grabbed;

    void Start()
    {
        recipeAudio = gameObject.GetComponent<AudioSource>();
        recipeMesh = gameObject.GetComponent<MeshRenderer>();
        recipeParticles = gameObject.transform.GetChild(0).gameObject;
    }

    // Destroys the game object when touched.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !grabbed)
        {
            grabbed = true;
            recipeMesh.enabled = false;
            Destroy(recipeParticles);
            recipeAudio.Play();
            StartCoroutine(WaitBeforeDestroy());
        }
    }

    IEnumerator WaitBeforeDestroy()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}