using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Walk : MonoBehaviour
{
    // IK & Walk
    private UpdateLegIK[] legs;
    private Vector3 lastInput = new Vector3();

    // Input Movement Controls & Parameters
    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private float jumpVelocity = 10.0f;
    [SerializeField, Range(.0f, 1.0f)] private float airTurnSpeed = 0.5f;

    private CharacterController charCtrl;
    private Vector2 lateralAirVelocity; // Current velocity in the X,Z plane
    private Vector3 inputVelocity; // Movement keys input in X,Z plane (0-1f)
    private bool movementKeyPressed = false;
    private bool isJumpPressed = false;

    private float currentRotation;
    [SerializeField] private float rotationSpeed = 10.0f;

    // Physics
    [SerializeField] private float gravity = -40.0f;
    private float groundedGravity = -0.5f;

    void Awake()
    {
        charCtrl = this.GetComponent<CharacterController>();
        legs = this.GetComponentsInChildren<UpdateLegIK>();
        if (legs.Length == 2)
        {
            legs[0].Init(transform, 1.0f);
            legs[1].Init(transform, 0.0f);
        }
    }

    void Update()
    {
        HandleRotation();
        HandleGravity();
        HandleMovement();
        HandleStep();
    }

    void HandleStep()
    {
        if (legs.Length == 2)
        {
            legs[0].UpdateVars(inputVelocity);
            legs[1].UpdateVars(inputVelocity);
        }
        lastInput = inputVelocity;
    }

    void HandleGravity()
    {    
        if (charCtrl.isGrounded) {
            if (inputVelocity.y > -20.0f) {
                inputVelocity.y += groundedGravity * Time.deltaTime;
            }
        } else {
            inputVelocity.y += gravity * Time.deltaTime;
        }
    }

    void HandleRotation()
    {
        Vector3 mousePosition;
        Vector3 objPosition;
        float angle;
        mousePosition = Input.mousePosition;
        mousePosition.z = 10.0f;
        objPosition = Camera.main.WorldToScreenPoint(transform.position);
        mousePosition.x = mousePosition.x - objPosition.x;
        mousePosition.y = mousePosition.y - objPosition.y;
        angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, -angle + 90, 0)), Time.deltaTime * rotationSpeed);
        transform.rotation = rotation;
    }

    public void HandleMovement()
    {
        var lateralVelocity = new Vector2(inputVelocity.x, inputVelocity.z).normalized;
        if (charCtrl.isGrounded) {   
            lateralAirVelocity = new Vector2(inputVelocity.x, inputVelocity.z).normalized;
            //Stop movement after key canceled
            if (!movementKeyPressed) {
                lateralVelocity = new Vector2(0.0f, 0.0f); 
            }
        } else {
            lateralVelocity = Vector2.Lerp(lateralAirVelocity, lateralVelocity, airTurnSpeed);
        }

        lateralVelocity *= movementSpeed;
        charCtrl.Move(new Vector3(lateralVelocity.x, inputVelocity.y, lateralVelocity.y) * Time.deltaTime);
    }

    public void OnMovement(InputAction.CallbackContext ctx) // TODO: these events run 2-3 times on click consider doing only once through logic
    {   
        var incVelocity = ctx.ReadValue<Vector2>().normalized;
        if(ctx.canceled) {
            movementKeyPressed = false;
        } 
        if(ctx.started) {
           movementKeyPressed = true; 
        }
        // If in the air (!grounded), and not moving (x,y both less than 0.2) -> Don't do anything
        if (!(incVelocity.x == 0 && incVelocity.y == 0 && !charCtrl.isGrounded))
        {
            Vector3 velocity = new Vector3(incVelocity.x, inputVelocity.y, incVelocity.y);

            inputVelocity = velocity;
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isJumpPressed = true;
        }
        if (ctx.canceled)
        {
            isJumpPressed = false;
        }
        if (ctx.performed && charCtrl.isGrounded)
        {
            inputVelocity.y = jumpVelocity;
        }
    }
}
