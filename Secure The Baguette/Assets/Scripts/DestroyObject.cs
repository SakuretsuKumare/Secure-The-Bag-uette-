using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public AudioSource recipeAudio;
    public MeshRenderer recipeMesh;
    public ParticleSystem recipeParticles;
    public bool grabbed;

    void Start()
    {
        recipeAudio = gameObject.GetComponent<AudioSource>();
        recipeMesh = gameObject.GetComponent<MeshRenderer>();
        recipeParticles = gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    // Destroys the game object when touched.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !grabbed)
        {
            grabbed = true;
            recipeMesh.enabled = false;
            recipeParticles.Stop();
            recipeAudio.Play();
            other.gameObject.GetComponent<CharacterMovement>().levelRecipesCollected.Add(gameObject);
        }
    }
}