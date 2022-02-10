using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public CharacterController characterController;
    public CharacterMovement characterMovementScript;
    public Transform[] wayPointList;
    public int currentWayPoint = 0;
    Transform targetWayPoint;
    public Vector3 playerSpawnPoint;
    public Quaternion playerSpawnRotation;
    private GameObject player;
    private GameObject playerModel;
    public Renderer playerRend;
    public Light detectionLight;
    public float visionRange;
    public float visionConeAngle;
    public float speed = 4f;
    public float rotSpeed = 10f;
    public bool IsIdle;
    public bool alerted;
    public bool playerObstructed;

    // Use this for initialization
    void Start()
    {
        characterMovementScript = GameObject.Find("Player").GetComponent<CharacterMovement>();
        alerted = false;
        player = GameObject.Find("Player");
        playerModel = GameObject.Find("PlayerModel");
        playerRend = playerModel.GetComponent<MeshRenderer>();
        characterController = player.GetComponent<CharacterController>();
        playerSpawnPoint = player.transform.position;
        playerSpawnRotation = player.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWayPoint < this.wayPointList.Length && !IsIdle && !alerted)
        {
            if (targetWayPoint == null)
                targetWayPoint = wayPointList[currentWayPoint];
            walk();
        }

        detectionLight.color = new Color32(248, 175, 0, 255);

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

        if (Vector3.Distance(transform.position, player.transform.position) <= visionRange && !playerObstructed)
        {
            if (Vector3.Angle(transform.forward, player.transform.position - transform.position) <= visionConeAngle || alerted)
            {
                Vector3 playerPositionAtOurHeight = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                if (!alerted && !playerObstructed)
                {
                    StartCoroutine(Caught());
                }
                if (alerted)
                {
                    detectionLight.color = Color.red;
                    transform.forward = Vector3.RotateTowards(transform.forward, playerPositionAtOurHeight - transform.position, rotSpeed * Time.deltaTime, 0.0f);
                }
            }
        }

        if (Vector3.Distance(transform.position, player.transform.position) <= visionRange && !playerObstructed && !characterMovementScript.isCrouching && characterController.velocity != Vector3.zero)
        {
            Vector3 playerPositionAtOurHeight = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            if (!alerted && !playerObstructed)
            {
                StartCoroutine(Caught());
            }
            if (alerted)
            {
                detectionLight.color = Color.red;
                transform.forward = Vector3.RotateTowards(transform.forward, playerPositionAtOurHeight - transform.position, rotSpeed * Time.deltaTime, 0.0f);
            }
        }
    }

    void walk()
    {
        transform.forward = Vector3.RotateTowards(transform.forward, targetWayPoint.position - transform.position, rotSpeed * Time.deltaTime, 0.0f);
        transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position, speed * Time.deltaTime);

        if (transform.position == targetWayPoint.position)
        {
            if (!targetWayPoint.CompareTag("IdleExclusion"))
            {
                StartCoroutine(Idle());
            }
            if (currentWayPoint != this.wayPointList.Length - 1)
            {
                currentWayPoint++;
            }
            else
            {
                currentWayPoint = 0;
            }

            targetWayPoint = wayPointList[currentWayPoint];
        }
    }

    IEnumerator Idle()
    {
        IsIdle = true;
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        IsIdle = false;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Caught());
        }
    }
}
