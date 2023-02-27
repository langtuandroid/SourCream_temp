using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WalkIK : MonoBehaviour
{
    // IK Controls & Parameters
    public LayerMask terrainLayer;
    public float footSpacing = 0.5f;
    public Vector3 stepDistance = new Vector3(1.0f, 0.0f ,0.5f);

    private Vector3 oldPosL;
    private Vector3 oldPosR;

    private Transform charT;
    private Transform root;
    private Transform legL;
    private Transform legR;

    // Input Movement Controls & Parameters
    [SerializeField]
    private float movementSpeed = 1.0f;
    [SerializeField]
    private float jumpVelocity = 10.0f;
    [SerializeField, Range(.0f, 1.0f)]
    private float airTurnSpeed = 0.5f;

    private CharacterController charCtrl;
    private Vector2 lateralAirVelocity; // Current velocity in the X,Z plane
    private Vector3 inputVelocity; // Movement keys input in X,Z plane (0-1f)
    private bool movementKeyPressed = false;
    private bool isJumpPressed = false;

    private float currentRotation;
    [SerializeField]
    private float rotationSpeed = 10.0f;

    // Physics
    [SerializeField]
    private float gravity = -40.0f;

    private float groundedGravity = -0.5f;

    public void Init(Transform legL, Transform legR, Transform root)
    {
        this.legL = legL;
        this.legR = legR;
        this.root = root;
    }

    void Awake()
    {
        charT = this.GetComponent<Transform>();
        charCtrl = this.GetComponent<CharacterController>();
    }

    void Start()
    {
        oldPosL = legL.position;
        oldPosR = legR.position;
    }

    void Update()
    {
        HandleRotation();
        HandleGravity();
        HandleMovement();

        // Get input walking direction
        var walkingDir = new Vector3();

        this.UpdateLeg(charT.position, ref oldPosL, ref legL, walkingDir, -1);
        this.UpdateLeg(charT.position, ref oldPosR, ref legR, walkingDir, 1);
    }

    void UpdateLeg(Vector3 origin, ref Vector3 oldPos, ref Transform currTrans, Vector3 direction, int side)
    {
        currTrans.position = oldPos;

        Ray ray = new Ray(new Vector3(origin.x, origin.y+5, origin.z) + (charT.right * side * footSpacing), Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer.value)) {
            float inEllipseX = ((info.point.x - oldPos.x)*(info.point.x - oldPos.x)) / (stepDistance.x*stepDistance.x);
            float inEllipseZ = ((info.point.z - oldPos.z)*(info.point.z - oldPos.z)) / (stepDistance.z*stepDistance.z);

            bool inEllipse = (inEllipseX + inEllipseZ) <= 1.0f;
            Debug.Log("Point: "+  info.point + ", X " + inEllipseX + ", Y: " + inEllipseZ);

            if(!inEllipse) {
                oldPos = info.point;
                currTrans.position = info.point;
            }
        }
    }

    public void HandleGravity()
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
        objPosition = Camera.main.WorldToScreenPoint(root.position);
        mousePosition.x = mousePosition.x - objPosition.x;
        mousePosition.y = mousePosition.y - objPosition.y;
        angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.Slerp(root.rotation, Quaternion.Euler(new Vector3(0, -angle + 90, 0)), Time.deltaTime * rotationSpeed);
        root.rotation = rotation;
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

    void OnDrawGizmos()
    {
        if(this.oldPosL != null) {
            Gizmos.DrawSphere(oldPosL, .2f);
        }
        if(this.oldPosR != null) {
            Gizmos.DrawSphere(oldPosR, .2f);
        }
        if(this.legL != null) {
            Gizmos.DrawSphere(legL.position, .25f);
        }
        if(this.legR != null) {
            Gizmos.DrawSphere(legR.position, .25f);
        }
    }
}
