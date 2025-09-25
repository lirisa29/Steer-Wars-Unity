using UnityEngine;
using UnityEngine.AI;

public class NPCCarAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private AIWaypoints currentWaypoint;
    
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    [SerializeField] private float stuckThreshold = 0.1f; // how little movement counts as stuck
    [SerializeField] private float stuckTime = 2f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Just in case – make sure NPC doesn't rotate weirdly
        agent.updateRotation = true;
        agent.updateUpAxis = true;
        agent.avoidancePriority = Random.Range(20, 80);
    }

    public void Initialize(AIWaypoints startWaypoint)
    {
        currentWaypoint = startWaypoint;
        MoveToNextWaypoint();
    }

    private void MoveToNextWaypoint()
    {
        if (currentWaypoint == null) return;

        // Randomize target within a small radius to prevent congestion
        Vector3 randomOffset = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        );

        agent.SetDestination(currentWaypoint.transform.position + randomOffset);
    }

    void Update()
    {
        if (currentWaypoint == null) return;

        // Check if stuck
        if (Vector3.Distance(transform.position, lastPosition) < stuckThreshold)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckTime)
            {
                HandleStuck();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;

        // Waypoint switching (your existing code)
        if (!agent.pathPending && agent.remainingDistance < 2f)
        {
            currentWaypoint = currentWaypoint.GetNext();
            MoveToNextWaypoint();
        }
    }
    
    private void HandleStuck()
    {
        // Pick a random offset near the current waypoint
        Vector3 safeOffset = new Vector3(Random.Range(-1f,1f), 0f, Random.Range(-1f,1f));
    
        // Set destination slightly offset
        agent.SetDestination(currentWaypoint.transform.position + safeOffset);
    }
}
