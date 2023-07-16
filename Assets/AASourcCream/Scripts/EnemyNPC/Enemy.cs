using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


/*TODOS:
    - Make animations apply on states
    - Make movement speed stat (I think just connect to a stats class) 
*/

public class Enemy : SerializedMonoBehaviour
{
    public NavMeshAgent agent;

    [SerializeField]
    public Transform player;

    //Patrolling 
    public Vector3 walkPoint;
    bool walkPointSet;

    bool walkPointResting;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public LayerMask whatIsGround, whatIsPlayer, NPCitself;

    public bool once;

    public GameObject projectile;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public bool isAttacking;

    public EnemyState currentState;

    private float middlePoint;

    [SerializeField]
    public float middlePointAdjustment;

    private Vector3 spawnLocation;

    private Rigidbody rigidBody;

    private EnemyAnimationCtrl animationCtrl;

    [FoldoutGroup("Regular Meele", expanded: false)]
    public Dictionary<string, int> regularMeeles;
    [FoldoutGroup("Regular Range", expanded: false)]
    public Dictionary<string, int> regularRangeAnims;
    [FoldoutGroup("Special", expanded: false)]
    public Dictionary<string, int> specialSkillAnims;

    [FoldoutGroup("Get Hit", expanded: false)]
    public Dictionary<string, int> getHitAnims;

    [FoldoutGroup("Die", expanded: false)]
    public Dictionary<string, int> deathAnims;

    [SerializeField]
    public string walkAnimation;

    [SerializeField]
    public string runAnimation;

    private float[] attackRollsList = new float[4];

    private float nextActionTime = 0.0f;

    [SerializeField]
    public GameObject collisionObject;

    private float animationLength;

    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Engaged,
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent) {
            agent.autoBraking = true;
        }
        spawnLocation = transform.position;
        once = false;
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animationCtrl = GetComponent<EnemyAnimationCtrl>();
    }

    private void FixedUpdate()
    {
        middlePoint = transform.position.y + middlePointAdjustment;
        Vector3 heightAdjustedPosition = new Vector3(transform.position.x, middlePoint, transform.position.z);
        playerInSightRange = Physics.CheckSphere(heightAdjustedPosition, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(heightAdjustedPosition, attackRange, whatIsPlayer);

        //Seems kinda cringe to do this, could also just set it constantly unsure which to prefer
        if (!isAttacking) {
            if (!playerInSightRange) {
                nextActionTime = 0;
                Patroling();
                currentState = EnemyState.Patrolling;
            } else if (playerInAttackRange) {
                AttackPlayer();
                currentState = EnemyState.Engaged;
            } else if (playerInSightRange && !playerInAttackRange) {
                nextActionTime = 0;
                ChasePlayer();
                currentState = EnemyState.Chasing;
            }
        }
    }

    private void Update()
    {
        Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
    }

    private void Patroling()
    {
        agent.velocity = agent.desiredVelocity / 2f;
        //animationCtrl.PlayAnimation(walkAnimation);
        //If there is no point to walk towards and is not resting after reaching the point Search for a random location to walk towards and go after 1-4s
        if (!walkPointSet && !walkPointResting) {
            walkPointResting = true;
            agent.isStopped = false;
            StartCoroutine(GenericMonoHelper.Instance.GenericWait(Random.Range(1, 4), () => SearchWalkPoint()));
        }
        //If point to walk towards exists go there and check collision
        if (walkPointSet && currentState == EnemyState.Patrolling) {
            agent.SetDestination(walkPoint);
            if (Physics.CheckSphere(walkPoint, 2.0f, NPCitself)) {
                agent.isStopped = true;
                walkPointSet = false;
            }
        }
    }

    //TODO: Make this avoid tiny range locations
    private void SearchWalkPoint()
    {

        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(spawnLocation.x + randomX, spawnLocation.y, spawnLocation.z + randomZ);
        walkPointSet = true;
        walkPointResting = false;
    }

    private void ChasePlayer()
    {
        agent.velocity = agent.desiredVelocity / 1.2f;
        //animationCtrl.PlayAnimation(runAnimation);
        walkPointSet = false;
        walkPoint = Vector3.zero;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        //rigidBody.MovePosition(player.position);
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {

        agent.velocity = agent.desiredVelocity / 1.2f;
        //Make sure enemy doesn't move


        agent.SetDestination(transform.position);
        var targetUnrotated = player;

        MeeleAttack();
    }

    private void UpdateAlreadyAttacked()
    {
        alreadyAttacked = !alreadyAttacked;
    }

    //SET up weights for prefered attacks i.e only meele, prefers meele, only range, prefers rangee etc and then deeper for singulars
    private void MeeleAttack()
    {
        if (Time.time > nextActionTime) {
            nextActionTime += timeBetweenAttacks;
            WeightedList<string> weightedList = new();

            for (int i = 0; i < regularMeeles.Count; i++) {
                var item = regularMeeles.ElementAt(i);
                weightedList.Add(item.Key, item.Value);
            }

            var chosenAttack = weightedList.Next();
            //Debug.Log(chosenAttack);
            animationLength = animationCtrl.PlayAnimation(chosenAttack) - 0.3f;

            Debug.Log(GenericColliderHelper.Instance);
            StartCoroutine(GenericMonoHelper.Instance.GenericWait(0.3f, () => CallCollider()));

            Debug.Log(animationLength);
            isAttacking = true;
            StartCoroutine(GenericMonoHelper.Instance.GenericWait(Random.Range(animationLength + 0.2f, animationLength + 0.4f), () => ResetAttack()));
        } else {
            return;
        }
    }

    private void ResetAttack()
    {
        animationCtrl.PlayAnimation(runAnimation);
        isAttacking = false;
    }

    private void CallCollider()
    {
        var dmgInfo = new DamageInformation(ScalingTypes.PHYSICAL, 20);
        GenericColliderHelper.Instance.EnemySpawnCollider(ColliderTypes.SimpleCapsule, collisionObject ? collisionObject.transform : transform, animationLength, dmgInfo);
    }

    void OnDrawGizmos()
    {
        Vector3 heightAdjustedPosition = new Vector3(transform.position.x, middlePoint, transform.position.z);
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(246, 182, 215, 0.4f);
        Gizmos.DrawWireSphere(heightAdjustedPosition, sightRange);
        Gizmos.color = new Color(100, 100, 100, 0.4f);
        Gizmos.DrawWireSphere(heightAdjustedPosition, attackRange);
        Gizmos.DrawSphere(walkPoint, 2.0f);
    }

}
