using System.Collections.Generic;
using UnityEngine;

public class NPCCarSpawner : MonoBehaviour
{
    [Header("NPC Car Settings")]
    [SerializeField] private GameObject[] npcCarPrefabs; // multiple prefabs
    [SerializeField] private int npcCount = 5; // how many cars to spawn

    private List<AIWaypoints> availableWaypoints;

    void Start()
    {
        availableWaypoints = new List<AIWaypoints>(FindObjectsByType<AIWaypoints>(FindObjectsSortMode.None));

        for (int i = 0; i < npcCount; i++)
        {
            SpawnNPC();
        }
    }

    private void SpawnNPC()
    {
        if (availableWaypoints.Count == 0 || npcCarPrefabs.Length == 0) return;

        // Pick random car prefab
        GameObject chosenPrefab = npcCarPrefabs[Random.Range(0, npcCarPrefabs.Length)];

        // Pick random waypoint and remove it from the list
        int index = Random.Range(0, availableWaypoints.Count);
        AIWaypoints startWaypoint = availableWaypoints[index];
        availableWaypoints.RemoveAt(index); // ensures no duplicates

        // Spawn NPC
        GameObject npc = Instantiate(chosenPrefab, startWaypoint.transform.position, Quaternion.identity);

        // Initialize AI
        NPCCarAI ai = npc.GetComponent<NPCCarAI>();
        ai.Initialize(startWaypoint.GetNext());
    }
}
