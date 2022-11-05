using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CharMovement : MonoBehaviour
{
    // <------------------------------- MOVEMENT ------------------------------- //
    [SerializeField]
    private float movementSpeed = 1.0f;
    [SerializeField]
    private float jumpVelocity = 10.0f;
    [SerializeField, Range(.0f, 1.0f)]
    private float airTurnSpeed = 0.5f;

    private bool movementPressed;
    private bool isJumpPresssed;

    private Vector2 lateralAirVelocity; // Current velocity in the X,Z plane
    private Vector3 inputVelocity; // Movement keys input in X,Z plane (0-1f)
    private CharacterController charController;

    private bool movementKeyPressed = false;


    // ------------------------------- PHYSICS ------------------------------- //
    [SerializeField]
    private float gravity = -5.0f;

    private float groundedGravity = -0.5f;


    // ------------------------------- ANIMATION ------------------------------- //
    Animator animator;
    int isWalkingHash;
    int isRunningHash;


    // Start is called before the first frame update
    void Start()
    {
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    void Awake()
    {
        charController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        HandleMovementAnims();
        HandleGravity();
        HandleMovement();
    }

    public void HandleMovement()
    {
        var lateralVelocity = new Vector2(inputVelocity.x, inputVelocity.z).normalized;
        if (charController.isGrounded) {   
            lateralAirVelocity = new Vector2(inputVelocity.x, inputVelocity.z).normalized;
            //Stop movement after key canceled
            if (!movementKeyPressed) {
                lateralVelocity = new Vector2(0.0f, 0.0f); 
            }
        } else {
            lateralVelocity = Vector2.Lerp(lateralAirVelocity, lateralVelocity, airTurnSpeed);
        }
        Debug.Log(charController.isGrounded);
        Debug.Log(inputVelocity.y);
        lateralVelocity *= movementSpeed;
        charController.Move(new Vector3(lateralVelocity.x, inputVelocity.y, lateralVelocity.y) * Time.deltaTime);
    }

    public void HandleGravity()
    {
        if (charController.isGrounded) {
            if (inputVelocity.y > -20.0f) {
                inputVelocity.y += groundedGravity * Time.deltaTime;
            }
        } else {
            inputVelocity.y += gravity * Time.deltaTime;
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        var incVelocity = ctx.ReadValue<Vector2>().normalized;
        if(ctx.canceled) {
            movementKeyPressed = false;
        } 
        if(ctx.started) {
           movementKeyPressed = true; 
        }
        //Debug.Log(incVelocity);
        // If in the air (!grounded), and not moving (x,y both less than 0.2) -> Don't do anything
        if (!(incVelocity.x == 0 && incVelocity.y == 0 && !charController.isGrounded))
        {
            Vector3 velocity = new Vector3(incVelocity.x, inputVelocity.y, incVelocity.y);
            inputVelocity = velocity;
        }
    }

    void HandleMovementAnims()
    {
        if (charController.isGrounded)
        {
            if (charController.velocity.magnitude > 3)
            {
                animator.SetBool(isWalkingHash, true);
            }
            else
            {
                animator.SetBool(isWalkingHash, false);
            }

            if (charController.velocity.magnitude > 7)
            {
                animator.SetBool(isRunningHash, true);
            }
            else
            {
                animator.SetBool(isRunningHash, false);
            }
        }
    }

    void HandleRotation()
    {
        Vector3 mousePosition;
        Vector3 objPosition;
        Transform target = transform;
        float angle;
        mousePosition = Input.mousePosition;
        mousePosition.z = 10.0f;
        objPosition = Camera.main.WorldToScreenPoint(target.position);
        mousePosition.x = mousePosition.x - objPosition.x;
        mousePosition.y = mousePosition.y - objPosition.y;
        angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
        target.rotation = Quaternion.Euler(new Vector3(0, -angle + 90, 0)); //MAKE THIS LOOK TOWARDS CURSOR
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isJumpPresssed = true;
        }
        if (ctx.canceled)
        {
            isJumpPresssed = false;
        }
        if (ctx.performed && charController.isGrounded)
        {
            inputVelocity.y = jumpVelocity;
        }
    }
}
