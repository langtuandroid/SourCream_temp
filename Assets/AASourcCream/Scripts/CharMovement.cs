using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CharMovement : MonoBehaviour
{
    private IndicatorController indicatorController;
    private SkillsController skillsController;
    private Movement movementController;
    // <------------------------------- SETTINGS ------------------------------- //
    [SerializeField]
    private bool lookAtMouse = true;

    // <------------------------------- ATTACK ------------------------------- //
    private bool isAttacking;

    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private Transform projectileStartLocation;

    private Vector3 gizmoThing;

    private Dictionary<string, Action> abilityList;


    private bool isJumpPressed;
    [SerializeField] private float rotationSpeed = 10.0f;
    [SerializeField] private float jumpVelocity = 10.0f;

    private Vector2 lateralAirVelocity; // Current velocity in the X,Z plane
    private Vector3 inputVelocity; // Movement keys input in X,Z plane (0-1f)
    private CharacterController charController;



    // ------------------------------- ANIMATION ------------------------------- //
    Animator animator;

    int forward;
    int right;
    int inAir;
    int isMoving;

    private float nextActionTime = 0.0f;
    private float period = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        forward = Animator.StringToHash("forward");
        right = Animator.StringToHash("right");
        inAir = Animator.StringToHash("inAir");
        isMoving = Animator.StringToHash("isMoving");
    }

    void Awake()
    {
        charController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        skillsController = GetComponent<SkillsController>();
        movementController = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementAnims();
    }

    // Example animation usage
    // public void HandleAttackAction()
    // {
    //     if (Time.time > nextActionTime) { //TODO: FEEL FREE TO UNDO THIS DUMB SHIT also remove nextActionTime += period; in OnFire
    //         nextActionTime += period;
    //         if (isAttacking) {
    //             isAttacking = animator.GetCurrentAnimatorStateInfo(1).IsName("attack");
    //         }
    //     }
    // }

    void HandleMovementAnims()
    {
        animator.SetBool(inAir, !movementController.trackedIsGrounded);
        if (charController.isGrounded && !movementController.inDash && movementController.movementInput) {
            animator.SetBool(isMoving, true);
            var multiplier = isAttacking ? 3 : 10;
            //TODO: => smooth value update / lerp? 
            var tempForward = Vector3.Dot(transform.forward, new Vector3(movementController.movementInputVelocity.x, 0.0f, movementController.movementInputVelocity.y)) * multiplier;
            var tempRight = Vector3.Dot(transform.right, new Vector3(movementController.movementInputVelocity.x, 0.0f, movementController.movementInputVelocity.y)) * multiplier;

            animator.SetFloat(forward, tempForward);
            animator.SetFloat(right, tempRight);
        } else if (!movementController.movementInput) {
            animator.SetFloat(forward, 0);
            animator.SetFloat(right, 0);
            animator.SetBool(isMoving, false);
        }
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.started) {
            if (!isAttacking) {
                isAttacking = true;
                nextActionTime += period;
                animator.SetTrigger("Attack");
            }
        }
    }

    //MOVE THIS
    public void onColide(Collider colider)
    {
        //Debug.Log(colider.transform.gameObject.layer);
        if (colider.transform.gameObject.layer == 6 && isAttacking) {
            var stats = colider.GetComponent<StatsComponent>();
            stats.Damage(new DamageInformation(ScalingTypes.PHYSICAL, 10.0f));
        }

    }
    //MOVE THIS
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(246, 182, 215, 0.4f);
        Gizmos.DrawSphere(gizmoThing, 0.2f);
    }
}
