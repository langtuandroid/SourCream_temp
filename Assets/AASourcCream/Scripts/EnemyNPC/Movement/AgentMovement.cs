using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AgentMovement : MonoBehaviour
{
    public Transform player;
    public float thresholdDistance = 1.0f;
    private NavMeshAgent agent;
    private Coroutine movementCoroutine;
    public float chaseDistance = 10.0f;

    private Vector3 initialPosition;
    private bool isChasing = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(transform.position); // Initialize agent's position on the NavMesh
        initialPosition = transform.position;
        movementCoroutine = StartCoroutine(MoveTowardsPlayer());
    }

    public IEnumerator MoveTowardsPlayer()
    {
        while (true) {
            float distance = Vector3.Distance(transform.position, player.position);
            if (isChasing) {
                if (distance < thresholdDistance) {
                    // Distance is below threshold, stop moving
                    agent.isStopped = true;

                    // Call the method when destination is reached
                    randomMethodName();

                    // Stop the coroutine
                    StopCoroutine(movementCoroutine);
                    yield break;
                } else if (distance > chaseDistance) {
                    // Distance is too far, stop chasing and return to initial position
                    isChasing = false;
                    agent.isStopped = false;
                    agent.SetDestination(initialPosition);
                } else {
                    // Continue chasing the player
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                }
            } else {
                // Check if the agent should start chasing the player
                if (distance <= chaseDistance) {
                    isChasing = true;
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                }
            }

            yield return null;
        }
    }

    private void randomMethodName()
    {
        // Your code here
        Debug.Log("Destination reached!");
    }
}
