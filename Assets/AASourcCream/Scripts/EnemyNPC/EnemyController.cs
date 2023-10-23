using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


/* Note: animations are called via the controller for both the character and capsule using animator null checks
*/
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(EnemyAnimationCtrl))]

public class EnemyController : MonoBehaviour
{

    private StateGraph stateGraph;

    [SerializeField]
    private string currentState;
    private NavMeshAgent navMeshAgent;

    private Animator animator;
    private Vector2 velocity;

    [SerializeField]
    bool overrideRootRotations = false;
    private Vector2 smoothDeltaPosition;

    [SerializeField]
    LayerMask layerMask;

    /* Variables To handle enemy detection*/
    [SerializeField]
    private float sightRange;
    [SerializeField]
    private float middlePointAdjustment;
    private float middlePoint;
    private bool playerInSightRange;

    [SerializeField]
    private LayerMask attackMask;

    /* Variables to help for walking points*/
    private Vector3 startingPoint;

    private Vector3 distanceFromStartingPoint;

    [SerializeField]
    private float maxDistanceFromStartingPoint;

    [SerializeField]
    private float maxWalkDistance;

    private Vector3 randomWalkablePoint;

    [SerializeField]
    private float IdlingTimeSeconds;

    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;


    /*Variables for chasing and attacking*/
    private WeightController weightController;

    private AbilityController abilityController;

    [SerializeField]
    private GameObject player;
    private EnemyActions nextAction;

    private bool isCasting;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updatePosition = false;

        if (overrideRootRotations) {
            navMeshAgent.updateRotation = false;
        }

        startingPoint = transform.position;
        SetUpStateGraph();
    }

    private void Start()
    {
        weightController = GetComponent<WeightController>();
        abilityController = GetComponent<AbilityController>();

    }

    private void Update()
    {
        SyncAnimatorAndAgent();
        currentState = stateGraph?.currentState?.id;
    }

    private void FixedUpdate()
    {
        UpdatePlayerInSight();
        UpdateDistanceFromStartingPoint();
    }

    ///<summary>
    ///Manages the relation between navMeshAgent(saying where can go) and animations (moving throuhg root motion)
    ///I thought this would be good, but it still sucks ass
    ///</summary>
    private void SyncAnimatorAndAgent()
    {
        Vector3 worldDeltaPosition = navMeshAgent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;

        float deltaX = Vector3.Dot(transform.right, worldDeltaPosition);
        float deltaY = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(deltaX, deltaY);

        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);
        velocity = smoothDeltaPosition / Time.deltaTime;
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
            velocity = Vector2.Lerp(
                Vector2.zero,
                velocity,
                navMeshAgent.remainingDistance / navMeshAgent.stoppingDistance
            );
        }

        bool movementState = stateGraph.currentState.id == "walkState"
            || stateGraph.currentState.id == "retreatState"
            || stateGraph.currentState.id == "chaseState";

        bool shouldMove = velocity.magnitude > 0.5f
            && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance
            && movementState;


        animator.SetBool("move", shouldMove);
        animator.SetBool("run", stateGraph.currentState?.id == "walkState" ? false : true);
        float fakeVal = velocity.magnitude == float.NaN ? 0.0f : velocity.magnitude;

        animator.SetFloat("velocity", fakeVal);

        float deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > navMeshAgent.radius / 2.5f) {
            transform.position = Vector3.Lerp(
                animator.rootPosition,
                navMeshAgent.nextPosition,
                smooth
            );
        }
    }
    ///<summary>
    ///Overrides a method from animator that happens when an animation play to move the NPC
    ///Part of syncing navMesh and animator 
    ///</summary>
    private void OnAnimtorMove()
    {
        Vector3 rootPosition = animator.rootPosition;
        rootPosition.y = navMeshAgent.nextPosition.y;

        if (overrideRootRotations) {
            transform.rotation = animator.rootRotation;
        }

        navMeshAgent.nextPosition = rootPosition;
    }

    private void UpdatePlayerInSight()
    {
        middlePoint = transform.position.y + middlePointAdjustment;
        Vector3 heightAdjustedPosition = new Vector3(transform.position.x, middlePoint, transform.position.z);
        playerInSightRange = Physics.CheckSphere(heightAdjustedPosition, sightRange, attackMask);

        if (playerInSightRange && (stateGraph.currentState.id == "walkState" || stateGraph.currentState.id == "idleState")) {
            stateGraph.currentState.enterConnectedState("chaseState");
        }
    }

    private void UpdateDistanceFromStartingPoint()
    {
        var distance = Vector3.Distance(startingPoint, transform.position);

        if (distance > maxDistanceFromStartingPoint && stateGraph.currentState.id == "chaseState") {
            stateGraph.currentState.enterConnectedState("retreatState");
        }
    }

    private void SetUpStateGraph()
    {
        //ON spawn set up state graph
        stateGraph = new StateGraph();

        var idleState = new State("idleState", onIdleStateEnter, onIdleStateExit, stateGraph);
        var walkState = new State("walkState", onWalkStateEnter, onWalkStateExit, stateGraph);
        var chaseState = new State("chaseState", onChaseStateEnter, onChaseStateExit, stateGraph);
        var retreatState = new State("retreatState", onRetreatStateEnter, onRetreatStateExit, stateGraph);
        var attackState = new State("attackState", onAttackStateEnter, onAttackStateExit, stateGraph);

        idleState.setConnectedStates(walkState, chaseState);
        walkState.setConnectedStates(idleState, chaseState);
        chaseState.setConnectedStates(retreatState, attackState);
        retreatState.setConnectedStates(idleState);
        attackState.setConnectedStates(chaseState);

        idleState.enterInitialState();
    }

    private void onChaseStateEnter()
    {
        navMeshAgent.speed = runSpeed;
        nextAction = weightController.nextAction;
        var playerLocation = player.transform.position;
        navMeshAgent.SetDestination(playerLocation);
        StartCoroutine(continueslyChase());
    }

    IEnumerator continueslyChase()
    {
        for (; ; ) {
            if (proximityCheck()) {
                stateGraph.currentState.enterConnectedState("attackState");
                yield break;
            };
            yield return new WaitForSeconds(.2f);
        }
    }

    bool proximityCheck()
    {
        var distance = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log(distance);
        navMeshAgent.SetDestination(player.transform.position);
        if (distance < nextAction.CombatAction.range) {
            return true;
        }

        return false;
    }

    private void onChaseStateExit()
    {
        StopCoroutine(continueslyChase());
    }

    private void onAttackStateEnter()
    {
        navMeshAgent.updateRotation = false;
        transform.LookAt(player.transform);
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        abilityController.CallCombatAction(nextAction.CombatAction, player);
        isCasting = true;
        nextAction = weightController.GetNextPreferedAction();
        GenericMonoHelper.Instance.CallMethodWithDelay(nextAction.CombatAction.castTime, () => {
            isCasting = false;
            if (nextAction.CombatAction.range >= Vector3.Distance(transform.position, player.transform.position)) {
                stateGraph.currentState.enterConnectedState("attackState");
            } else {
                navMeshAgent.updateRotation = true;
                navMeshAgent.isStopped = false;
                stateGraph.currentState.enterConnectedState("chaseState");
            }
        });
    }

    private void onAttackStateExit()
    {

    }

    private void onRetreatStateEnter()
    {
        navMeshAgent.speed = runSpeed;
        navMeshAgent.SetDestination(startingPoint);
        StartCoroutine("checkIfDestinationReached", checkIfDestinationReached());

    }

    private void onRetreatStateExit()
    {
        StopCoroutine("checkIfDestinationReached");
    }

    private void onWalkStateEnter()
    {
        navMeshAgent.SetDestination(randomWalkablePoint);
        navMeshAgent.speed = walkSpeed;
        StartCoroutine("checkIfDestinationReached", checkIfDestinationReached());
    }


    private void onWalkStateExit()
    {
        StopCoroutine(checkIfDestinationReached());

    }

    IEnumerator checkIfDestinationReached()
    {
        yield return new WaitUntil(() => destinationReached() || stateGraph.currentState.id != "walkState");
        if (stateGraph.currentState.id != "walkState" || stateGraph.currentState.id != "idleState") {
            stateGraph.currentState.enterConnectedState("idleState");
        }
    }

    private bool destinationReached()
    {
        if (!navMeshAgent.pathPending) {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f) {
                    return true;// Done
                }
            }
        }
        return false;
    }

    private void onIdleStateEnter()
    {
        randomWalkablePoint = GetRandomPoint(startingPoint, maxWalkDistance);

        StartCoroutine(DelayedMethod(IdlingTimeSeconds, () => { if (stateGraph.currentState.id == "idleState") startWalkState("walkState"); }));
    }

    private void onIdleStateExit()
    {

    }

    private void startWalkState(string stateId)
    {
        if (stateGraph.currentState.id == "idleState") {
            stateGraph.currentState.enterConnectedState(stateId);
        } else {
            StopCoroutine("DelayedMethod");
        }
    }

    public Vector3 GetRandomPoint(Vector3 center, float maxDistance)
    {
        // Get Random Point inside Sphere which position is center, radius is maxDistance
        Vector3 randomPos = UnityEngine.Random.insideUnitSphere * maxDistance + center;

        NavMeshHit hit; // NavMesh Sampling Info Container

        bool foundPosition = NavMesh.SamplePosition(
            randomPos,
            out hit,
            maxDistance,
            NavMesh.AllAreas
        );

        if (!foundPosition) {
            //TODO should try again, but that can create infinite loop?
            return Vector3.zero;
        } else {
            NavMeshPath path = new NavMeshPath();
            navMeshAgent.CalculatePath(hit.position, path);
            var canReachPoint = path.status == NavMeshPathStatus.PathComplete;
            if (canReachPoint) {
                return hit.position;
            }
        }
        return Vector3.zero;
    }

    private IEnumerator DelayedMethod(float delayInSeconds, System.Action methodToCall)
    {
        yield return new WaitForSeconds(delayInSeconds);
        methodToCall.Invoke();
    }
}
