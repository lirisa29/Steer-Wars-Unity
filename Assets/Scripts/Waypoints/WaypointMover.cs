using UnityEngine;
using System.Collections;

public class WaypointMover : MonoBehaviour
{
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float distanceThreshold = 1f;

    [Header("Idle Settings")]
    [SerializeField] private float minIdleTime = 2f; // minimum time to idle
    [SerializeField] private float maxIdleTime = 5f; // maximum time to idle
    [SerializeField] private float minWalkTime = 3f; // minimum walking before idling again
    [SerializeField] private float maxWalkTime = 8f; // maximum walking before idling again

    private Transform currentWaypoint;
    private Animator animator;
    private bool isIdle = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        // Set first waypoint
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;

        // Set next waypoint
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.LookAt(currentWaypoint);

        // Start random walk/idle cycle
        StartCoroutine(RandomIdleRoutine());
    }

    private void Update()
    {
        if (isIdle) return; // stop moving if idle

        // Movement
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold)
        {
            currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
            transform.LookAt(currentWaypoint);
        }
    }

    private IEnumerator RandomIdleRoutine()
    {
        while (true)
        {
            // Walk for random time
            float walkTime = Random.Range(minWalkTime, maxWalkTime);
            animator.SetBool("isWalking", true);
            isIdle = false;
            yield return new WaitForSeconds(walkTime);

            // Idle for random time
            float idleTime = Random.Range(minIdleTime, maxIdleTime);
            animator.SetBool("isWalking", false);
            isIdle = true;
            yield return new WaitForSeconds(idleTime);
        }
    }
}