using UnityEngine;

public class AIWaypoints : MonoBehaviour
{
    [Tooltip("Possible next waypoints from here (for branches)")]
    public AIWaypoints[] nextWaypoints;

    public AIWaypoints GetNext()
    {
        if (nextWaypoints.Length == 0) return null;
        return nextWaypoints[Random.Range(0, nextWaypoints.Length)];
    }
}
