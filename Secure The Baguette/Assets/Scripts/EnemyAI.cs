using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public AudioSource guardAudio;
    public AudioClip[] clips;
    public Transform[] wayPointList;
    Transform targetWayPoint;
    public RawImage suspicionSign;
    public Light detectionLight;
    public float visionRange;
    public float extendedVisionRangeMultiplier;
    public float outOfViewRange;
    public float visionConeAngle;
    //public float chaseRadius;
    public float chaseSpeed;
    public float speed;
    public float rotSpeed;
    public bool IsIdle;
    public bool alerted;
    public bool suspicious;
    public bool idleSuspicious;
    public bool playerObstructed;
    public bool noticed;
    private GameObject player;
    private NavMeshAgent navMeshAgent;
    private int currentWayPoint;
    private Vector3 lastSeenPlayerPosition;
    private CharacterController characterController;
    private CharacterMovement characterMovementScript;

    void Start()
    {
        guardAudio = gameObject.GetComponent<AudioSource>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        characterMovementScript = GameObject.Find("Player").GetComponent<CharacterMovement>();
        alerted = false;
        player = GameObject.Find("Player");
        characterController = player.GetComponent<CharacterController>();
    }

    void Update()
    {
        //Patrol
        if (currentWayPoint < this.wayPointList.Length && !IsIdle && !alerted && navMeshAgent.enabled == false && !noticed)
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
        if (Vector3.Distance(transform.position, player.transform.position) <= outOfViewRange && !playerObstructed && !characterMovementScript.isCrouching && characterController.velocity != Vector3.zero && !noticed)
        {
            if (navMeshAgent.enabled == true)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
                lastSeenPlayerPosition = player.transform.position;
                navMeshAgent.destination = lastSeenPlayerPosition;
            }
            StartCoroutine(Notice());
        }

        if (noticed)
        {
            if (navMeshAgent.enabled == true)
            {
                suspicionSign.enabled = true;
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
                lastSeenPlayerPosition = player.transform.position;
                navMeshAgent.destination = lastSeenPlayerPosition;
                navMeshAgent.speed = chaseSpeed;
            }
            Vector3 playerPositionAtOurHeight = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.forward = Vector3.RotateTowards(transform.forward, playerPositionAtOurHeight - transform.position, rotSpeed * Time.deltaTime, 0.0f);
        }

        //Suspicion
        if (Vector3.Distance(transform.position, player.transform.position) <= visionRange * extendedVisionRangeMultiplier && Vector3.Distance(transform.position, player.transform.position) > visionRange && !playerObstructed)
        {
            if (Vector3.Angle(transform.forward, player.transform.position - transform.position) <= visionConeAngle)
            {
                suspicionSign.enabled = true;
                idleSuspicious = false;
                suspicious = true;
                lastSeenPlayerPosition = player.transform.position;
                navMeshAgent.enabled = true;
                navMeshAgent.speed = chaseSpeed;
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();
                navMeshAgent.destination = lastSeenPlayerPosition;
            }
        }

        if (navMeshAgent.enabled == true && Vector3.Distance(transform.position, lastSeenPlayerPosition) < 0.3f && !idleSuspicious)
        {
            idleSuspicious = true;
            StartCoroutine(IdleSuspicious());
        }

        //Distance Check
        /*if (Vector3.Distance(transform.position, targetWayPoint.position) > chaseRadius && navMeshAgent.enabled == true)
        {
            suspicious = false;
            navMeshAgent.speed = speed;
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            navMeshAgent.destination = targetWayPoint.position;
        }*/
    }

    void walk()
    {
        if (!noticed)
        {
            Vector3 lookDirection = (targetWayPoint.position - transform.position).normalized;
            lookDirection.y = 0;
            transform.forward = Vector3.RotateTowards(transform.forward, lookDirection, rotSpeed * Time.deltaTime, 0.0f);
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

    // If the enemy notices the player
    IEnumerator Notice()
    {
        noticed = true;
        yield return new WaitForSeconds(0.5f);
        noticed = false;
    }
    IEnumerator Idle()
    {
        IsIdle = true;
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        IsIdle = false;

        if (navMeshAgent.enabled == true)
        {
            suspicionSign.enabled = false;
            suspicious = false;
            navMeshAgent.speed = speed;
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            navMeshAgent.destination = targetWayPoint.position;
        }
    }

    IEnumerator IdleSuspicious()
    {
        //Debug.Log("Where did he go?");
        if (navMeshAgent.enabled == true)
        {
            navMeshAgent.speed = 0;
        }

        yield return new WaitForSeconds(2f);

        if (navMeshAgent.enabled == true && idleSuspicious)
        {
            suspicionSign.enabled = false;
            suspicious = false;
            navMeshAgent.speed = speed;
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            navMeshAgent.destination = targetWayPoint.position;
            yield return new WaitForSeconds(0.2f);
            idleSuspicious = false;
        }

        else
        {
            suspicious = false;
        }
    }

    IEnumerator Caught()
    {
        if (navMeshAgent.enabled == true)
        {
            navMeshAgent.speed = 0;
        }

        var caughtAudio = clips[0];
        guardAudio.clip = caughtAudio;
        guardAudio.Play();
        suspicionSign.enabled = true;
        alerted = true;
        characterMovementScript.speed = 0f;
        yield return new WaitForSeconds(5f);
        characterController.enabled = false;
        player.transform.position = characterMovementScript.playerSpawnPoint;
        player.transform.rotation = characterMovementScript.playerSpawnRotation;
        characterController.enabled = true;
        characterMovementScript.speed = 16f;
        //Scene scene = SceneManager.GetActiveScene(); 
        //SceneManager.LoadScene(scene.name);
        alerted = false;
        suspicionSign.enabled = false;
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