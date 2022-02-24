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
    public Vector3 playerLastPosition;
    public Vector3 playerSpawnPoint;
    public Quaternion playerSpawnRotation;
    private GameObject player;
    private GameObject playerModel;
    private NavMeshAgent navMeshAgent;
    public Renderer playerRend;
    public Light detectionLight;
    public float visionRange;
    public float extendedVisionRange;
    public float visionConeAngle;
    public float chaseRadius;
    public float chaseSpeed;
    public float speed;
    public float rotSpeed;
    public bool IsIdle;
    public bool alerted;
    public bool suspicious;
    public bool playerObstructed;
    public bool transitionBack;

    // Use this for initialization
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

    // Update is called once per frame
    void Update()
    {
        if (currentWayPoint < this.wayPointList.Length && !IsIdle && !alerted && !suspicious)
        {
            if (targetWayPoint == null)
                targetWayPoint = wayPointList[currentWayPoint];
            walk();
        }

        detectionLight.color = new Color32(248, 175, 0, 255);

        RaycastHit hit;
        if (Vector3.Distance(transform.position, player.transform.position) <= visionRange * extendedVisionRange && Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, visionRange * extendedVisionRange) && !hit.transform.CompareTag("Player"))
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

        if (Vector3.Distance(transform.position, player.transform.position) <= visionRange * extendedVisionRange && Vector3.Distance(transform.position, player.transform.position) > visionRange && !playerObstructed)
        {
            if (Vector3.Angle(transform.forward, player.transform.position - transform.position) <= visionConeAngle || alerted)
            {
                if (!alerted && !playerObstructed)
                {
                    if (!suspicious)
                    {
                        suspicious = true;
                        playerLastPosition = player.transform.position;
                    }
                }
            }
        }

        if (suspicious && !playerObstructed && !alerted && navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete && !IsIdle)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, playerLastPosition - transform.position, rotSpeed * Time.deltaTime, 0.0f);
            transform.position = Vector3.MoveTowards(transform.position, playerLastPosition, chaseSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, player.transform.position) <= visionRange * extendedVisionRange && Vector3.Distance(transform.position, player.transform.position) > visionRange && !playerObstructed)
            {
                if (Vector3.Angle(transform.forward, player.transform.position - transform.position) <= visionConeAngle || alerted)
                {
                    if (Vector3.Distance(transform.position, targetWayPoint.position) <= chaseRadius)
                    {
                        playerLastPosition = player.transform.position;
                        transform.forward = Vector3.RotateTowards(transform.forward, playerLastPosition - transform.position, rotSpeed * Time.deltaTime, 0.0f);
                        transform.position = Vector3.MoveTowards(transform.position, playerLastPosition, chaseSpeed * Time.deltaTime);
                    }
                    else
                    {
                        suspicious = false;
                        transitionBack = true;
                        StartCoroutine(Idle());
                    }
                }
            }

        }
        else
        {
            suspicious = false;
        }

        if (Vector3.Distance(transform.position, playerLastPosition) < 0.1f)
        {
            transitionBack = true;
            StartCoroutine(Idle());
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
        if (!suspicious)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, targetWayPoint.position - transform.position, rotSpeed * Time.deltaTime, 0.0f);

            if (!transitionBack)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position, speed * Time.deltaTime);
            }
            else
            {
                navMeshAgent.destination = targetWayPoint.position;
            }

            if (transform.position == targetWayPoint.position)
            {
                if (navMeshAgent.enabled == true)
                {
                    transitionBack = false;
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
    }

    IEnumerator Idle()
    {
        IsIdle = true;
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        IsIdle = false;
        if (transitionBack)
        {
            navMeshAgent.enabled = true;
            navMeshAgent.destination = targetWayPoint.position;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Caught());
        }
        else
        {
            if (other.gameObject.CompareTag("Waypoint") && transitionBack)
            {
                transitionBack = false;
                navMeshAgent.enabled = false;
            }
            if (other.gameObject.CompareTag("IdleExclusion") && transitionBack)
            {
                transitionBack = false;
                navMeshAgent.enabled = false;
            }
        }
    }
}
