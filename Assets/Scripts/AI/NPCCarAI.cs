using UnityEngine;
using UnityEngine.AI;

public class NPCCarAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private AIWaypoints currentWaypoint;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Just in case – make sure NPC doesn't rotate weirdly
        agent.updateRotation = true;
        agent.updateUpAxis = true;
    }

    public void Initialize(AIWaypoints startWaypoint)
    {
        currentWaypoint = startWaypoint;
        MoveToNextWaypoint();
    }

    private void MoveToNextWaypoint()
    {
        if (currentWaypoint == null) return;

        agent.SetDestination(currentWaypoint.transform.position);
    }

    void Update()
    {
        if (currentWaypoint == null) return;

        // Check if close enough to switch waypoint
        if (!agent.pathPending && agent.remainingDistance < 2f)
        {
            currentWaypoint = currentWaypoint.GetNext(); // pick one of its connected waypoints
            MoveToNextWaypoint();
        }
    }
}
