using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] public float walkSpeed = 2;
    [SerializeField] public float sprintspeed = 5;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("crouchin")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode interact = KeyCode.E;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public Transform feet;
    public float groundDistance;

    bool grounded = false;

 
    //initializing variables for rigid body, move speed and jump force
    Rigidbody rb;
    public Transform orientation;
    Vector3 moveDirection;
    float horizontalInput;
    float verticalInput;

    private Animator animator;
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    //grounded collision

    //calculating angle of player to ground
    float RoundedNormalVectorAngle(Vector3 normal, uint decimalAccuracy)
    {
        int accuracy = (int)Mathf.Pow(10, decimalAccuracy);
        return Mathf.RoundToInt(Vector3.Angle(normal, Vector3.up) * accuracy) / accuracy;

    }

    //to call index from unity must be 2^(index number)... no clue why
    bool CompareLayerIndex(Transform transform, LayerMask layer)
    {
        return Mathf.Pow(2, transform.gameObject.layer) == layer;
    }

    //seeing if your ass is still on the ground
    bool isStillTouchingGround(List<ContactPoint> contactPoints, int numberOfContacts)
    {
        for (int i = 0; i < numberOfContacts; i++)
        {
            if (RoundedNormalVectorAngle(contactPoints[i].normal, 3) <= 45)
            {
                return true;
            }
        }
        return false;
    }

    List<Collider> groundTouchPoints = new List<Collider>();

void OnCollisionStay(Collision collision)
    {   
        List<ContactPoint> contactPoints = new List<ContactPoint>();
        int numberOfContacts = collision.GetContacts(contactPoints);

        for (int i = 0; i < numberOfContacts; i++)
        {
            Collider collider = collision.collider;
            if (RoundedNormalVectorAngle(contactPoints[i].normal, 3) <= 45 && CompareLayerIndex(collision.transform, whatIsGround) && !groundTouchPoints.Contains(collider))

            {
                groundTouchPoints.Add(collider);
            }

            else if (!isStillTouchingGround(contactPoints, numberOfContacts) && groundTouchPoints.Contains(collider))
            {
                groundTouchPoints.Remove(collider);
            }
            }
        }
void OnCollisionExit(Collision collision)
    {   
            Collider collider = collision.collider;

        if (groundTouchPoints.Contains(collider)) 
        {
            groundTouchPoints.Remove(collider);
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        PickupObject.totalLoot = 0;
        rb = GetComponent<Rigidbody>();
        readyToJump = true;

        startYScale = transform.localScale.y;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //ground check
        grounded = groundTouchPoints.Count > 0;

        MyInput();
        SpeedControl();
        StateHandler();

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

        {
            //idle
            animator.SetFloat("Speed", 0f);
        }
        if (state == MovementState.sprinting)
        {
            //running
            animator.SetFloat("Speed", .5f);
        }

        if (state == MovementState.walking)
        {
            //walking
            animator.SetFloat("Speed", 0f);
        }

        //swiper be swipin'
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetBool("Swipe", true);
        }

        //swiper no swiping
        if (Input.GetKeyUp(KeyCode.E))
        {
            animator.SetBool("Swipe", false);
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
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //start crouchin yo ass
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        //stop crouchin yo ass
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        // Mode - Sneakymode
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            movementSpeed = crouchSpeed;
        }
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            movementSpeed = sprintspeed;
        }

        else if (grounded)
        {
            state = MovementState.walking;
            movementSpeed = walkSpeed;

        }
        else
        {
            state = MovementState.air;
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
        if (flatVel.magnitude > movementSpeed)
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
