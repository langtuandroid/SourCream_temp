using UnityEngine;
using UnityEngine.AI;

public class DashMovement : MonoBehaviour
{
    public float dashDuration = 0.5f;
    public float dashCooldown = 2f;
    private NavMeshAgent agent;
    private bool isDashing;
    private Vector3 dashDirection;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Dash(MovementDirections dashDirection, float dashDistance)
    {
        if (isDashing)
            return;

        switch (dashDirection) {
            case MovementDirections.FORWARD:
                this.dashDirection = transform.forward * dashDistance;
                break;
            case MovementDirections.BACKWARD:
                this.dashDirection = -transform.forward * dashDistance;
                break;
            case MovementDirections.SIDEWAY:
                Vector3 right = transform.right;
                right.y = 0f;
                this.dashDirection = right.normalized * dashDistance;
                break;
        }

        // Start the dash
        PerformDash();
    }

    private void PerformDash()
    {
        // Set the destination for the dash
        Vector3 dashDestination = transform.position + dashDirection;

        // Disable the agent while dashing to prevent interference with manual movement
        agent.enabled = false;

        // Initialize the position using NavMeshAgent.Warp
        agent.Warp(transform.position);

        // Start the coroutine for dashing
        StartCoroutine(DashCoroutine(dashDestination));
    }

    private System.Collections.IEnumerator DashCoroutine(Vector3 destination)
    {
        isDashing = true;

        // Move towards the destination in a straight line over the specified duration
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < dashDuration) {
            float t = elapsedTime / dashDuration;
            transform.position = Vector3.Lerp(startingPosition, destination, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set the final position to the destination
        transform.position = destination;

        // Re-enable the agent
        agent.enabled = true;

        // Start the cooldown
        StartCoroutine(DashCooldownCoroutine());

        isDashing = false;
    }

    private System.Collections.IEnumerator DashCooldownCoroutine()
    {
        yield return new WaitForSeconds(dashCooldown);

        // Code to handle dash cooldown if needed
    }
}