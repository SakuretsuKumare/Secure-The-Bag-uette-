using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    public CharacterController characterController;
    public CharacterMovement characterMovementScript;
    public Vector3 playerSpawnPoint;
    public Vector3 lastSeenPlayerPosition;
    public Quaternion playerSpawnRotation;
    public float speed = 1;
    public float rotAngleYMin;
    public float rotAngleYMax;
    public float originRotation;
    public float resetCameraAngle;
    public float extendedVisionRangeMultiplier;
    public Renderer playerRend;
    public Light detectionLight;
    public bool alerted;
    public bool playerObstructed;
    public float visionRange;
    public float visionConeAngle;
    private GameObject player;
    private GameObject playerModel;
    void Start()
    {
        resetCameraAngle = transform.localEulerAngles.x;
        originRotation = transform.localEulerAngles.y;
        characterMovementScript = GameObject.Find("Player").GetComponent<CharacterMovement>();
        alerted = false;
        player = GameObject.Find("Player");
        playerModel = GameObject.Find("PlayerModel");
        playerRend = playerModel.GetComponent<MeshRenderer>();
        characterController = player.GetComponent<CharacterController>();
        playerSpawnPoint = player.transform.position;
        playerSpawnRotation = player.transform.rotation;
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
        if (Vector3.Distance(transform.position, player.transform.position) <= visionRange * extendedVisionRangeMultiplier && Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, visionRange * extendedVisionRangeMultiplier) && !hit.transform.CompareTag("Player"))
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
            alerted = true;
            playerRend.material.color = new Color32(5, 192, 236, 255);
            characterMovementScript.speed = 0f;
            yield return new WaitForSeconds(0.5f);
            characterController.enabled = false;
            playerRend.material.color = new Color32(5, 96, 236, 255);
            player.transform.rotation = playerSpawnRotation;
            player.transform.position = playerSpawnPoint;
            characterController.enabled = true;
            characterMovementScript.speed = 16f;
            //Scene scene = SceneManager.GetActiveScene(); 
            //SceneManager.LoadScene(scene.name);
            alerted = false;
            transform.rotation = Quaternion.Euler(resetCameraAngle, 0, 0);
        }
    }
}
