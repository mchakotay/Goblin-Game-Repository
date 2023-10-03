using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float movementSpeed = 5f;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    //initializing variables for rigid body, move speed and jump force
    Rigidbody rb;
    public Transform orientation;
    Vector3 moveDirection;
    float horizontalInput;
    float verticalInput;

<<<<<<< Updated upstream
<<<<<<< Updated upstream
    // Start is called before the first frame update
=======
=======
    private Animator animator;

>>>>>>> Stashed changes
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    //animation
    private Animator animator;

    // Start is called before the first frame update 
>>>>>>> Stashed changes
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        readyToJump = true;
<<<<<<< Updated upstream
=======

        startYScale = transform.localScale.y;

        //getting animator
        animator = GetComponentInChildren<Animator>();
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        
        MyInput();
        SpeedControl();

        //handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        //movement animations between walking/idle and running
        if (moveDirection == Vector3.zero)
<<<<<<< Updated upstream
        { 
=======
        {
>>>>>>> Stashed changes
            //idle
            animator.SetFloat("Speed", 0f);
        }
        if (state == MovementState.sprinting)
        {
            //running
            animator.SetFloat("Speed", .5f);
        }
<<<<<<< Updated upstream
        if (state == MovementState.walking)      
=======
        if (state == MovementState.walking)
>>>>>>> Stashed changes
        {
            //walking
            animator.SetFloat("Speed", 0f);
        }
    }


    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        //every frame we get the input values. GetAxis will return a value between 0 and 1
        //if you're using keyboard it will be 0 or 1, on controller it will depend on how far you push the joystick
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        //this function just calculates movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);
        }
        //in air you get a little more speed 
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    //This function basically just ensures that we can only move at the max speed
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocity if needed
        if(flatVel.magnitude > movementSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    
    private void ResetJump()
    {
        readyToJump = true;
    }
    
}
