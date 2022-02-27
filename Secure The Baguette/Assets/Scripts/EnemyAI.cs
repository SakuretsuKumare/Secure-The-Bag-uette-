using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public CharacterController characterController;
    public CharacterMovement characterMovementScript;
    public Transform[] wayPointList;
    public int currentWayPoint;
    Transform targetWayPoint;
    public Vector3 playerSpawnPoint;
    public Vector3 lastSeenPlayerPosition;
    public Quaternion playerSpawnRotation;
    private GameObject player;
    private GameObject playerModel;
    private NavMeshAgent navMeshAgent;
    public Renderer playerRend;
    public Light detectionLight;
    public float visionRange;
    public float extendedVisionRangeMultiplier;
    public float visionConeAngle;
    public float chaseRadius;
    public float chaseSpeed;
    public float speed;
    public float rotSpeed;
    public bool IsIdle;
    public bool alerted;
    public bool suspicious;
    public bool playerObstructed;
    public bool noticed;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
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
        //Patrol
        if (currentWayPoint < this.wayPointList.Length && !IsIdle && !alerted && !suspicious && !noticed)
        {
            if (targetWayPoint == null)
                targetWayPoint = wayPointList[currentWayPoint];
            walk();
        }

        detectionLight.color = new Color32(248, 175, 0, 255);

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

        //Noticed
        if (Vector3.Distance(transform.position, player.transform.position) <= visionRange * extendedVisionRangeMultiplier && !playerObstructed && !characterMovementScript.isCrouching && characterController.velocity != Vector3.zero && !noticed)
        {
            StartCoroutine(Notice());
        }

        if (noticed && !playerObstructed)
        {
            Vector3 playerPositionAtOurHeight = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.forward = Vector3.RotateTowards(transform.forward, playerPositionAtOurHeight - transform.position, rotSpeed * Time.deltaTime, 0.0f);
        }

        //Suspicion
        if (Vector3.Distance(transform.position, player.transform.position) <= visionRange * extendedVisionRangeMultiplier && Vector3.Distance(transform.position, player.transform.position) > visionRange && !suspicious && !playerObstructed)
        {
            if (Vector3.Angle(transform.forward, player.transform.position - transform.position) <= visionConeAngle)
            {
                suspicious = true;
                lastSeenPlayerPosition = player.transform.position;
                navMeshAgent.enabled = true;
                navMeshAgent.speed = chaseSpeed;
                navMeshAgent.destination = lastSeenPlayerPosition;
            }
        }
        if (navMeshAgent.enabled == true && navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && suspicious)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            StartCoroutine(Suspicion());
        }
        if (navMeshAgent.enabled == true && suspicious)
        {
            navMeshAgent.destination = lastSeenPlayerPosition;
        }
    }

    void walk()
    {
        if (!suspicious && !noticed)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, targetWayPoint.position - transform.position, rotSpeed * Time.deltaTime, 0.0f);
            transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position, speed * Time.deltaTime);

            if (transform.position == targetWayPoint.position)
            {
                if (navMeshAgent.enabled == true)
                {
                    navMeshAgent.enabled = false;
                }
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
        else
        {
            navMeshAgent.destination = targetWayPoint.position;
        }
    }

    IEnumerator Suspicion()
    {
        yield return new WaitForSeconds(2f);
        suspicious = false;
        if (navMeshAgent.enabled == true)
        {
            navMeshAgent.speed = speed;
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            navMeshAgent.destination = targetWayPoint.position;
        }
        StartCoroutine(Idle());
    }
    IEnumerator Notice()
    {
        noticed = true;
        yield return new WaitForSeconds(0.5f);
        noticed = false;
    }
    IEnumerator Idle()
    {
        if (navMeshAgent.enabled == true)
        {
            navMeshAgent.speed = 0;
        }
        IsIdle = true;
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        IsIdle = false;
        if (navMeshAgent.enabled == true)
        {
            navMeshAgent.speed = speed;
        }
    }

    IEnumerator Caught()
    {
        if (navMeshAgent.enabled == true)
        {
            navMeshAgent.speed = 0;
        }
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
        StartCoroutine(Idle());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Caught());
        }
        else
        {
            if (other.gameObject.CompareTag("Waypoint") || other.gameObject.CompareTag("IdleExclusion"))
            {
                if (navMeshAgent.enabled == true)
                {
                    navMeshAgent.enabled = false;
                }
            }
        }
    }
}
