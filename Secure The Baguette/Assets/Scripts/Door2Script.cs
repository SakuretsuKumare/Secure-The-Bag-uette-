using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door2Script : MonoBehaviour
{
    public float doorSpeed;
    public float doorLiftedAmount;
    public float openingDistance;
    public float closingDistance;
    public bool goingUp;
    public bool stayOpen;
    public bool moving;
    private bool cooledDown;
    private bool callOnce;
    private Vector3 movedUp;
    private Vector3 origin;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        origin = transform.position;
        movedUp = transform.position + new Vector3(0, doorLiftedAmount, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= openingDistance)
        {
            moving = true;
            cooledDown = false;
            goingUp = true;
        }

        if (Vector3.Distance(transform.position, player.transform.position) >= closingDistance && Vector3.Distance(transform.position, origin) > Mathf.Epsilon)
        {
            if (!moving)
            {
                goingUp = false;
            }
            
            if (!callOnce && !cooledDown && Vector3.Distance(transform.position, origin) > Mathf.Epsilon)
            {
                callOnce = true;
                StartCoroutine(CoolDown());
            }
        }

        if (goingUp && !stayOpen)
        {
            transform.position = Vector3.MoveTowards(transform.position, movedUp, doorSpeed * Time.deltaTime);
        }
        
        else
        {
            if (goingUp && moving)
            {
                transform.position = Vector3.MoveTowards(transform.position, movedUp, doorSpeed * Time.deltaTime);
            }
        }

        if (!goingUp && !stayOpen)
        {
            transform.position = Vector3.MoveTowards(transform.position, origin, doorSpeed * Time.deltaTime);
        }

        else
        {
            if (!goingUp && moving)
            {
                transform.position = Vector3.MoveTowards(transform.position, origin, doorSpeed * Time.deltaTime);
            }
        }

        if (Vector3.Distance(transform.position, origin) < Mathf.Epsilon)
        {
            cooledDown = false;
        }

        if (Vector3.Distance(transform.position, movedUp) < Mathf.Epsilon)
        {
            moving = false;
        }

        else
        {
            if (Vector3.Distance(transform.position, origin) > Mathf.Epsilon)
            {
                moving = true;
            }
        }

        if (Vector3.Distance(transform.position, origin) < Mathf.Epsilon)
        {
            moving = false;
        }

        else
        {
            if (Vector3.Distance(transform.position, movedUp) > Mathf.Epsilon)
            {
                moving = true;
            }
        }
    }

    IEnumerator CoolDown()
    {
        stayOpen = true;
        yield return new WaitForSeconds(2f);
        stayOpen = false;
        callOnce = false;
        cooledDown = true;
    }
}