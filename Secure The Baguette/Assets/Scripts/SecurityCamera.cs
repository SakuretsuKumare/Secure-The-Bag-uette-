using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecurityCamera : MonoBehaviour
{
    public AudioSource cameraAudio;
    public AudioClip[] clips;
    public float speed;
    public float rotAngleYMin;
    public float rotAngleYMax;
    public Light detectionLight;
    public bool alerted;
    public bool playerObstructed;
    public RawImage suspicionSign;
    public float visionRange;
    public float visionConeAngle;
    private float resetCameraAngle;
    private float originRotation;
    private GameObject player;
    private CharacterController characterController;
    private CharacterMovement characterMovementScript;
    void Start()
    {
        cameraAudio = gameObject.GetComponent<AudioSource>();
        resetCameraAngle = transform.localEulerAngles.x;
        originRotation = transform.localEulerAngles.y;
        characterMovementScript = GameObject.Find("Player").GetComponent<CharacterMovement>();
        alerted = false;
        player = GameObject.Find("Player");
        characterController = player.GetComponent<CharacterController>();
        visionRange = detectionLight.range - 2;
        visionConeAngle = detectionLight.spotAngle / 2 + 1;
    }

    void Update()
    {
        if (!alerted)
        {
            float rX = Mathf.SmoothStep(rotAngleYMin + originRotation, rotAngleYMax + originRotation, Mathf.PingPong(Time.time * speed, 1));
            transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, rX, 0);
            detectionLight.color = new Color32(0, 27, 248, 255);
        }

        //Raycasting
        RaycastHit hit;
        if (Vector3.Distance(transform.position, player.transform.position) <= visionRange && Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, visionRange) && !hit.transform.CompareTag("Player"))
        {
            Debug.DrawRay(transform.position, (player.transform.position - transform.position), Color.red, 0.01f, false);
            playerObstructed = true;
        }
        else
        {
            playerObstructed = false;
        }

        //Detected
        if (Vector3.Distance(transform.position, player.transform.position) <= visionRange && !playerObstructed)
        {
            if (Vector3.Angle(transform.forward, player.transform.position - transform.position) <= visionConeAngle || alerted)
            {
                if (!alerted)
                {
                    StartCoroutine(Caught());
                }
                if (alerted)
                {
                    detectionLight.color = Color.green;
                    transform.forward = Vector3.RotateTowards(transform.forward, player.transform.position - transform.position, 10f * Time.deltaTime, 0.0f);
                }
            }
        }

        IEnumerator Caught()
        {
            if (!characterMovementScript.isCaught)
            {
                if (!characterMovementScript.isCaught)
                {
                    characterMovementScript.isCaught = true;
                }

                var caughtAudio = clips[0];
                cameraAudio.clip = caughtAudio;
                cameraAudio.Play();
                suspicionSign.enabled = true;
                alerted = true;
                characterMovementScript.speed = 0f;
                yield return new WaitForSeconds(5f);
                characterController.enabled = false;
                characterMovementScript.isCaught = false;
                player.transform.position = characterMovementScript.playerSpawnPoint;
                player.transform.rotation = Quaternion.identity;
                characterController.enabled = true;
                characterMovementScript.speed = 16f;
                //Scene scene = SceneManager.GetActiveScene(); 
                //SceneManager.LoadScene(scene.name);
                alerted = false;
                transform.rotation = Quaternion.Euler(resetCameraAngle, 0, 0);
                suspicionSign.enabled = false;
            }
        }
    }
}
