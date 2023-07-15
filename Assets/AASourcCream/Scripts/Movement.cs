using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    // ------------------------------- Controllers ------------------------------- //
    private CharacterController charController;
    private SkillsController skillsController;
    // <------------------------------- ROTATION ------------------------------- //
    [SerializeField]
    private float rotationSpeed = 10.0f;
    // ------------------------------- MOVEMENT ------------------------------- //
    private Vector3 movementVector;
    // ------------------------------- PHYSICS ------------------------------- //
    [SerializeField]
    private float gravity = -5.0f;

    private float groundedGravity = -1.5f;

    private float verticalVelocity;

    public bool trackedIsGrounded { get; private set; }

    private float lastUngroundedTime = 0.0f;

    private Stopwatch stopwatch;

    //----------------> JUMP
    [SerializeField]
    private float maxJumpHeight = 2.6f;
    private float maxJumpTime = 0.7f;

    private float initialJumpVelocity;

    // ------------------------------- INPUT HANDLING ------------------------------- //
    public Vector2 movementInputVelocity { get; private set; }
    public bool movementInput { get; private set; }

    private Vector2 lastMovementInputVelocity;

    private bool jumpInput;

    private bool dodgeInput;

    public bool inDash { get; private set; }
    [SerializeField]
    private float movementSpeed;
    private float dashTime = 0.1f;
    private float dashSpeed = 35f;
    private Vector3 dashVelocity;

    // Start is called before the first frame update
    void Start()
    {
        stopwatch = new Stopwatch();

    }

    void Awake()
    {
        charController = GetComponent<CharacterController>();
        skillsController = GetComponent<SkillsController>();
        setupJumpVars();
    }

    private void setupJumpVars()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    // Update is called once per frame
    void Update()
    {
        SetTrackedIsGrounded();
        UpdateRotation();
        HandleMovement();
        UpdateGravity();
    }

    void FixedUpdate()
    {
        SetTrackedIsGrounded();
    }

    private void SetTrackedIsGrounded()
    {
        if (!charController.isGrounded && !stopwatch.IsRunning) {
            stopwatch.Start();
        }
        if (stopwatch.ElapsedMilliseconds > 180) {
            trackedIsGrounded = false;
        }
        if (charController.isGrounded) {
            trackedIsGrounded = true;
            stopwatch.Reset();
        }
        //UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);

    }

    private void HandleMovement()
    {
        movementVector = new Vector3(0.0f, verticalVelocity, 0.0f);
        if (inDash) {
            movementVector.x = dashVelocity.x;
            movementVector.z = dashVelocity.y;
        } else if (!trackedIsGrounded) {
            var lateralMovement = new Vector2(movementInputVelocity.x * movementSpeed, movementInputVelocity.y * movementSpeed);
            if (lastMovementInputVelocity != Vector2.zero) {
                var lerpedMovement = Vector2.Lerp(lastMovementInputVelocity, lateralMovement, 0.8f);
                movementVector = new Vector3(lerpedMovement.x, verticalVelocity, lerpedMovement.y);
            } else {
                movementVector.x = lateralMovement.x;
                movementVector.z = lateralMovement.y;
            }
        } else if (movementInput) {
            movementVector.x = movementInputVelocity.x * movementSpeed;
            movementVector.z = movementInputVelocity.y * movementSpeed;
        }

        charController.Move(movementVector * Time.deltaTime);

    }

    public void OnMovementPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.started) {
            movementInput = true;
        } else if (ctx.canceled) {
            movementInput = false;
            movementInputVelocity = ctx.ReadValue<Vector2>().normalized;
        } else if (ctx.performed) {
            movementInputVelocity = ctx.ReadValue<Vector2>().normalized;
            lastMovementInputVelocity = Vector2.zero;

        }

    }

    public void OnJumpPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.started && trackedIsGrounded && !jumpInput) {
            //UnityEngine.Debug.Log("STARTED");

            jumpInput = true;
            if (verticalVelocity < 5.0f) { //Disabled double jump if you click twice faster than trackedIsGrounded :(
                verticalVelocity = initialJumpVelocity;
            }
            if (movementInput) {
                lastMovementInputVelocity = new Vector2(movementVector.x, movementVector.z);
            } else {
                lastMovementInputVelocity = Vector2.zero;
            }
        } else if (ctx.canceled) {
            //UnityEngine.Debug.Log("CANCELED");
            jumpInput = false;
        }
    }

    public void OnDodgePressed(InputAction.CallbackContext ctx)
    {
        if (ctx.started) {
            StartCoroutine(DashCoroutine());
            dodgeInput = true;
        } else if (ctx.canceled) {
            dodgeInput = false;
        }
    }



    //TODO FIND A WAY TO DO THIS WHILE NOT ALSO PRESSING A MOVEMENT KEY I don't even know what this means anymore
    private IEnumerator DashCoroutine()
    {
        float startTime = Time.time;
        inDash = true;
        var forward = movementInput ? movementInputVelocity : new Vector2(transform.forward.x, transform.forward.z);

        while (startTime + dashTime > Time.time) {
            Vector3 moveDirection = forward;
            dashVelocity = moveDirection * dashSpeed;
            yield return null;
        }
        inDash = false;
    }

    void UpdateRotation()
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
        var roatation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, -angle + 90, 0)), Time.deltaTime * rotationSpeed);
        target.rotation = roatation;
    }

    public void UpdateGravity()
    {
        if (charController.isGrounded) {
            verticalVelocity = groundedGravity * Time.deltaTime;
        } else {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
}

