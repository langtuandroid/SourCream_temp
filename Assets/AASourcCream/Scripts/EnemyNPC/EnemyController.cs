using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ActionState
{
    Default,
    Patrolling,
    Chasing,
    inAbility,

}

public enum PatrollingBehaviors
{
    InvokeMoveEveryXSeconds,
    StandStill
}

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private PatrollingBehaviors patrollingBehaviorPref;
    [SerializeField]
    private float middlePointAdjustment;
    [SerializeField]
    private LayerMask whatIsPlayer;
    [SerializeField]
    private float walkPointRange;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float sightRange;
    private float middlePoint;
    private bool playerInSightRange, playerInAttackRange;
    private PatrollingHandler patrollingHandler;
    private Vector3 spawnLocation;
    public NavMeshAgent agent;

    public ActionState currentState;

    private EventListCtrl eventListCtrl;

    private EventCustom currentEvent;

    private int nextBehaviorRange;

    //needs to be generic
    private FighterController fighterController;

    private AbilityController abilityController;

    private DashMovement dashMovement;

    private IEnumerator movementCoroutine;

    public float destinationUpdateDelay = 1.0f; // Delay between SetDestination calls

    [SerializeField]
    public float chaseDistance = 10.0f;

    [SerializeField]
    public float maxDistanceFromSpawn = 20.0f; // Maximum allowed distance from spawning position

    private Vector3 initialPosition;
    private bool isChasing = false;



    // Start is called before the first frame update
    void Start()
    {
        fighterController = GetComponent<FighterController>();
        dashMovement = GetComponent<DashMovement>();
        abilityController = GetComponent<AbilityController>();
        currentState = ActionState.Default;
        patrollingHandler = new PatrollingHandler();
        eventListCtrl = new EventListCtrl();
        // Initialize the position using NavMeshAgent.Warp
        agent.Warp(transform.position);
    }

    private void Awake()
    {
        spawnLocation = transform.position;
        agent = GetComponent<NavMeshAgent>();
        if (agent) {
            agent.autoBraking = true;
        }
    }

    void FixedUpdate()
    {
        if (!IsEnemyInSight()) {
            SetActionState(ActionState.Patrolling);
        } else if (!abilityController.abilityInProgress) {
            SetActionState(ActionState.Chasing);
        }
        // if (patrollingHandler.isTravelingToDestination) {
        //     if (patrollingHandler.checkIsDestinationReached(transform.position)) {
        //         StartCoroutine(GenericMonoHelper.Instance.GenericWait(0.3f, () => PatrollingStateAction()));
        //     }
        // }
    }


    bool IsEnemyInSight()
    {
        middlePoint = transform.position.y + middlePointAdjustment;
        Vector3 heightAdjustedPosition = new Vector3(transform.position.x, middlePoint, transform.position.z);
        playerInSightRange = Physics.CheckSphere(heightAdjustedPosition, sightRange, whatIsPlayer);
        return playerInSightRange;
    }


    void InvokePatrollingAction()
    {
        if (GetPatrollingBehavior() == PatrollingBehaviors.InvokeMoveEveryXSeconds) {
            patrollingHandler.SearchWalkPoint(walkPointRange, spawnLocation);
            walkTowards(patrollingHandler.currentDestination);
            StartCoroutine(DestinationReached());
        }
    }

    void InvokeInAbilityAction()
    {
        Debug.Log("INVOKE ABILTIY ACTION");
        agent.isStopped = true;
        StartCoroutine(WaitForCondition(!abilityController.abilityInProgress, () => {
            //NEVER GETS CCALLED CRINGE
            Debug.Log("ConditionMet");
            fighterController.getNextPreferedAction();
            SetActionState(ActionState.Chasing);
        }));

    }


    void SetActionState(ActionState value)
    {
        if (value != currentState) {
            //Coroutines here are actions specific to a state i.e moving to a patroll destination thus on change stop them
            //TODO Check if this messes up the variables used for them when just stoping in the middle. Perhaps needs a full on cleanup function too
            StopAllCoroutines();
            switch (value) {
                case ActionState.Patrolling:
                    currentState = ActionState.Patrolling;
                    //I am a bit uncertain why I need to delete this event to use it twice, yet it can go to patrolling without the event getting deleted
                    eventListCtrl.DeleteEvent("chasingEvent");
                    var patrollingEvent = new EventCustom("patrollingEvent", InvokePatrollingAction);
                    eventListCtrl.AddEvent(patrollingEvent);
                    eventListCtrl.GetEvent("patrollingEvent")?.InvokeEvent();
                    break;
                case ActionState.Chasing:
                    Debug.Log("CHASING CHASING CHASIN");
                    currentState = ActionState.Chasing;
                    if (fighterController.nextAction.CombatAction != null) {
                        Debug.Log(fighterController.nextAction.CombatAction.range);
                        nextBehaviorRange = fighterController.nextAction.CombatAction.range;
                    } else {
                        //TODO Figure out what needs to be done with first attack range when it is a movement action
                        nextBehaviorRange = 1;
                    }
                    var chasingEvent = new EventCustom("chasingEvent", InvokeChasingAction);
                    eventListCtrl.AddEvent(chasingEvent);
                    eventListCtrl.GetEvent("chasingEvent")?.InvokeEvent();
                    break;
                case ActionState.inAbility:
                    currentState = ActionState.inAbility;
                    Debug.Log("INABILITy");
                    //I am a bit uncertain why I need to delete this event to use it twice, yet it can go to patrolling without the event getting deleted
                    eventListCtrl.DeleteEvent("chasingEvent");
                    var inAbilityEvent = new EventCustom("inAbilityEvent", InvokeInAbilityAction);
                    eventListCtrl.AddEvent(inAbilityEvent);
                    eventListCtrl.GetEvent("inAbilityEventnt")?.InvokeEvent();
                    break;
                default:
                    break;
            }

        }
    }

    private IEnumerator WaitForCondition(bool conditionMet, Action methodToExecute)
    {
        // Keep looping until the condition is met
        while (!conditionMet) {
            Debug.Log(conditionMet);
            yield return null; // Wait for the next frame
        }

        // Condition is met, execute the method
        methodToExecute();
    }

    private IEnumerator DestinationReached()
    {
        Debug.Log("Destination started");
        yield return new WaitWhile(() => patrollingHandler.checkIsDestinationReached(transform.position, 5.0f) == true);
        EventCustom patrollingEvent = eventListCtrl.GetEvent("patrollingEvent");
        patrollingEvent?.ResetInvoken();
        yield return new WaitForSeconds(patrollingHandler.waitTimer);
        patrollingEvent?.InvokeEvent();

    }


    private PatrollingBehaviors GetPatrollingBehavior()
    {
        return patrollingBehaviorPref;
    }

    Vector3 CheckForPatrollingDestination()
    {
        if (patrollingHandler.currentDestination == null) {
            patrollingHandler.SearchWalkPoint(walkPointRange, spawnLocation);
            CheckForPatrollingDestination();
            return Vector3.zero;
        } else {
            return patrollingHandler.currentDestination;
        }
    }

    void walkTowards(Vector3 location)
    {
        var walkpointEvent = new EventCustom("walkingtoPoint", () => { });
        agent.SetDestination(location);
    }

    void OnDrawGizmos()
    {
        Vector3 heightAdjustedPosition = new Vector3(transform.position.x, middlePoint, transform.position.z);
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(246, 182, 215, 0.4f);
        Gizmos.DrawWireSphere(heightAdjustedPosition, sightRange);
        Gizmos.color = new Color(100, 100, 100, 0.4f);
        // Gizmos.DrawWireSphere(heightAdjustedPosition, 5.0f); //attackRange
        //Gizmos.DrawSphere(patrollingHandler.currentDestination, 2.0f);
    }

    // CHASING


    void InvokeChasingAction()
    {
        Debug.Log("INVOKING CHASING ACTION");
        isChasing = true;
        movementCoroutine = MoveTowardsPlayer();
        StartCoroutine(movementCoroutine);
    }

    private IEnumerator MoveTowardsPlayer()
    {
        while (true) {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (isChasing) {
                if (distance < nextBehaviorRange) {
                    // Distance is below threshold, stop moving
                    agent.isStopped = true;

                    // Call the method when destination is reached
                    callBehavior();

                    // Stop the coroutine
                    StopCoroutine(movementCoroutine);
                    yield break;
                } else {
                    // Continue chasing the player
                    agent.isStopped = false;
                    agent.SetDestination(player.transform.position);

                }
            }
            yield return null;
        }
    }

    //GETS CALLED AFTER CHASING THE PLAYER WHEN IN RANGE
    private void callBehavior()
    {
        var nextAction = fighterController.nextAction;
        Debug.Log(nextAction.CombatAction.name);
        if (nextAction.CombatAction != null) {
            var actionToCall = nextAction.CombatAction;
            invokeCombatAction(actionToCall);
            SetActionState(ActionState.inAbility);
        } else {
            // var movementToCall = fighterController.nextAction.MovementAction;
            // invokeMovementAction(movementToCall);
            // SetActionState(ActionState.inAbility);
        }
    }

    private void invokeMovementAction(EnemyMovementAction movementAction)
    {
        Debug.Log(movementAction.direction);
        dashMovement.Dash(movementAction.direction, movementAction.distance);
    }

    private void invokeCombatAction(EnemyCombatAction combatAction)
    {
        Debug.Log(combatAction.name);
        abilityController.callCombatAction(combatAction, player);
    }
}

public class PatrollingHandler
{
    public Vector3 currentDestination;

    public bool isDestinationReached;

    public bool isResting;

    public bool isTravelingToDestination;

    public float waitTimer;

    public PatrollingHandler()
    {
        //Constructor
    }


    public void SearchWalkPoint(float walkPointRange, Vector3 spawnLocation)
    {
        isDestinationReached = false;
        isResting = false;
        isTravelingToDestination = false;
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        waitTimer = UnityEngine.Random.Range(1.5f, 6.0f);

        currentDestination = new Vector3(spawnLocation.x + randomX, spawnLocation.y, spawnLocation.z + randomZ);

    }

    public bool checkIsDestinationReached(Vector3 currentPosition, float acceptableDistancee)
    {
        float dist = Vector3.Distance(currentPosition, currentDestination);
        if (dist <= acceptableDistancee && isDestinationReached == false) {
            isDestinationReached = true;
            isResting = true;
            currentDestination = Vector3.zero;
            return isDestinationReached;
        } else {
            isDestinationReached = false;
            return isDestinationReached;
        }
    }

    void PatrollingDestinationReachedEvent()
    {

    }


}

public class EventListCtrl
{
    public EventListCtrl()
    {
        eventList = new List<EventCustom>();
    }
    private List<EventCustom> eventList;

    public void InvokeEvent(string eventName)
    {
        eventList.Find(o => o.eventName == eventName)?.InvokeEvent();
    }

    public void CleanupEvent(string eventName, bool remove)
    {
        var eventItem = eventList.Find(o => o.eventName == eventName);
        eventItem?.CleanupMethod();
        if (remove) {
            eventList.Remove(eventItem);
        }

    }

    public void ResetInvokation(string eventName)
    {
        eventList.Find(o => o.eventName == eventName).ResetInvoken();
    }

    public void AddEvent(EventCustom eventItem)
    {
        if (eventList.Find(o => o.eventName == eventItem.eventName) == null) {
            eventList.Add(eventItem);
        }
    }

    public void DeleteEvent(string eventName)
    {
        eventList.Remove(eventList.Find(o => o.eventName == eventName));
    }

    public EventCustom GetEvent(string eventName)
    {
        return eventList.Find(o => o.eventName == eventName);
    }

}

public class EventCustom// : MonoBehaviour
{

    public EventCustom() { }
    public string eventName;

    public Action InvokenMethod;
    // <summary>
    // Not Implemented
    // </summary>
    public Action CleanupMethod;

    //I made thihs fuckin confusing rename
    public WhileInvokeSettings whileInvokeSettings;

    public bool invoken;

    public EventCustom(string nameVal, Action invokationMethod, Action cleanupMethod = null, WhileInvokeSettings awaitInvokeSettings = null)
    {
        eventName = nameVal;
        invoken = false;
        InvokenMethod = invokationMethod;
        whileInvokeSettings = awaitInvokeSettings;
        CleanupMethod = cleanupMethod;
    }

    public void InvokeEvent()
    {
        if (invoken == false) {
            invoken = true;
            //Debug.Log(InvokenMethod);
            InvokenMethod();
        }
    }

    //Am I retarded?
    public void ResetInvoken()
    {
        if (invoken == true) {
            invoken = false;
        }
    }

    // <summary>
    // Not Implemented
    // </summary>
    public void InvokeCleanup()
    {
        //invoken = false;
        //CleanupMethod?.Invoke();
    }

    IEnumerator Example()
    {
        yield return new WaitWhile(() => whileInvokeSettings.conditionWith == whileInvokeSettings.conditionAgainst);
        InvokeEvent();
    }
}


public record WhileInvokeSettings
{
    public bool conditionWith;
    public bool conditionAgainst;

    //public Action conditionInvoke;

}