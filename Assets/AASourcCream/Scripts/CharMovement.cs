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


    private CharacterController charController;



    // ------------------------------- ANIMATION ------------------------------- //
    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    int forward;
    int right;
    int inAir;
    int isMoving;

    private float nextActionTime = 0.0f;
    private float period = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
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
        HandleAttackAction();
        HandleMovementAnims();
    }

    public void HandleAttackAction()
    {
        if (Time.time > nextActionTime)
        { //TODO: FEEL FREE TO UNDO THIS DUMB SHIT also remove nextActionTime += period; in OnFire
            nextActionTime += period;
            if (isAttacking)
            {
                isAttacking = animator.GetCurrentAnimatorStateInfo(1).IsName("attack");
            }
        }
    }

    void HandleMovementAnims()
    {

        animator.SetBool(inAir, !movementController.trackedIsGrounded);
        if (charController.isGrounded && !movementController.inDash && movementController.movementInput)
        {
            animator.SetBool(isMoving, true);
            var multiplier = isAttacking ? 3 : 10;
            //TODO: => smooth value update / lerp? 
            var tempForward = Vector3.Dot(transform.forward, new Vector3(movementController.movementInputVelocity.x, 0.0f, movementController.movementInputVelocity.y)) * multiplier;
            var tempRight = Vector3.Dot(transform.right, new Vector3(movementController.movementInputVelocity.x, 0.0f, movementController.movementInputVelocity.y)) * multiplier;

            animator.SetFloat(forward, tempForward);
            animator.SetFloat(right, tempRight);
        }
        else if (!movementController.movementInput)
        {
            animator.SetBool(isMoving, false);
        }
    }


    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (!isAttacking)
            {
                isAttacking = true;
                nextActionTime += period;
                animator.SetTrigger("Attack");
            }
        }
    }
    //MOVE THIS to attack related
    public void onMbR(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Vector3 mousePosition = Input.mousePosition;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            LayerMask mask = LayerMask.GetMask("Terrain");
            if (Physics.Raycast(ray, out hit, mask))
            {
                float yPositon = hit.point.y + projectileStartLocation.localPosition.y + 1.0f;

                if (yPositon > projectileStartLocation.transform.position.y + 1.0f)
                {
                    yPositon = projectileStartLocation.transform.position.y + 1.0f;
                }
                else if (yPositon < projectileStartLocation.transform.position.y - 1.0f)
                {
                    yPositon = projectileStartLocation.transform.position.y - 2.0f;
                }

                var hitLocation = new Vector3(hit.point.x, yPositon, hit.point.z);
                gizmoThing = hitLocation;
                ProjectileSpawner.shootSimpleProjectile(hitLocation, projectileStartLocation.position, projectile, 10.0f);

            }
        }

    }
    //MOVE THIS
    public void onColide(Collider colider)
    {
        //Debug.Log(colider.transform.gameObject.layer);
        if (colider.transform.gameObject.layer == 6 && isAttacking)
        {
            var stats = colider.GetComponent<StatsComponent>();
            stats.Damage(new DamageInformation(DamageTypes.Physical, 10.0f));
        }

    }
    //MOVE THIS
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(246, 182, 215, 0.4f);
        Gizmos.DrawSphere(gizmoThing, 0.2f);
    }

    public void OnAbility1(InputAction.CallbackContext ctx)
    {
        CommonParams commonParams = new CommonParams();

        var mousePos = GenericHelper.GetMousePostion();
        var rotation = new Vector3(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z);
        var mousePosWithY = new Vector3(mousePos.x, mousePos.y + 0.5f, mousePos.z);

        //Setting the world parameters for the skill
        commonParams.SetValues(IndicatorShape.Circle, new Vector3(10.0f, 10.0f, 10.0f), mousePosWithY, rotation);

        //Currently being used in order to click and drag indicator, and do skill upon release rather than hold 
        if (ctx.phase == InputActionPhase.Started)
        {
            skillsController.UseAOESkill(commonParams, Phase.Start);
        }
        if (ctx.phase == InputActionPhase.Canceled)
        {
            skillsController.UseAOESkill(commonParams, Phase.End);
        }
    }

    public void Ability2(InputAction.CallbackContext ctx)
    {
        Debug.Log("2");
    }

    public void Ability3(InputAction.CallbackContext ctx)
    {
        Debug.Log("3");
    }

    public void Ability4(InputAction.CallbackContext ctx)
    {
        Debug.Log("4");
    }
}

