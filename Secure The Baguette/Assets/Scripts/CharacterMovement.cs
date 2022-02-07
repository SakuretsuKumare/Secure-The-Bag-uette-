using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This code was used from Brackeys https://www.youtube.com/watch?v=4HpC--2iowE
// Code was also used from ZeveonHD https://www.youtube.com/watch?v=D2LqcIxAQpY&t=65s

public class CharacterMovement : MonoBehaviour
{
    // Declaring and initiating variables.

    public CharacterController controller;
    public Transform cam;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float speed = 16f;
    private float turnSmoothTime = 0.1f;
    [SerializeField] float gravity = -19.62f;
    private float groundDistance = 0.2f;
    public float jumpHeight = 3f;
    float turnSmoothVelocity;
    public bool isSprinting = false;
    public bool isCrouching = false;
    public float sprintingMultiplier = 1.5f;
    public float crouchingMultiplier = 0.5f;
    Vector3 velocity;
    bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Checks if the player is touching the ground or not.
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Getting user input for movement.
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Regular walking movespeed, no crouching or sprinting.
            if ((isSprinting == false) && (isCrouching == false))
            {
                controller.Move(moveDirection.normalized * speed * Time.deltaTime);
            }

            // The character is sprinting
            else if ((isSprinting == true) && (isCrouching == false))
            {
                controller.Move(moveDirection.normalized * speed * sprintingMultiplier * Time.deltaTime);
            }

            // The character is crouching.
            else if ((isSprinting == false) && (isCrouching == true))
            {
                controller.Move(moveDirection.normalized * speed * crouchingMultiplier * Time.deltaTime);
            }

            else
            {
                controller.Move(moveDirection.normalized * speed * Time.deltaTime);
            }
        }

        // The player can jump with the space bar.
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Checks if the player is sprinting or not (Left Shift to sprint)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
        }

        else
        {
            isSprinting = false;
        }

        // Checks if the player is crouching or not (Left CTRL to crouch)
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
        }

        else
        {
            isCrouching = false;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}