using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerManager : MonoBehaviour
{
    public static PassengerManager Instance { get; private set; }

    [Header("Configuration")]
    public PassengerSO[] passengerTypes;
    public Transform[] pickupLocations;
    public Transform[] dropoffLocations;

    [Header("Request Flow")]
    public float requestInterval = 6f; // time between requests spawning (incremental)
    public int maxConcurrentRequests = 1;

    // runtime
    private List<PassengerRequest> activeRequests = new List<PassengerRequest>();
    private bool activePassenger = false;
    private PassengerSO currentPassengerSO;
    private Transform currentPickup;
    private Transform currentDropoff;
    private GameObject spawnedPassengerGO;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        StartCoroutine(RequestSpawner());
    }

    private IEnumerator RequestSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(requestInterval);
            
            // if there's no active request AND we don't already have a passenger in car
            if (activeRequests.Count < maxConcurrentRequests && !activePassenger)
            {
                GeneratePassengerRequest();
            }
        }
    }

    public void GeneratePassengerRequest()
    {
        // pick random passenger, pickup and dropoff
        currentPassengerSO = passengerTypes[Random.Range(0, passengerTypes.Length)];
        currentPickup = pickupLocations[Random.Range(0, pickupLocations.Length)];
        currentDropoff = dropoffLocations[Random.Range(0, dropoffLocations.Length)];
        
        // calculate distance to player
        GameObject player = GameObject.FindGameObjectWithTag("PlayerCar");
        float dist = 0f;
        if (player) dist = Vector3.Distance(player.transform.position, currentPickup.position);

        int reward = GameManager.Instance.GetEffectiveFare(currentPassengerSO.baseFare);

        // create request data
        PassengerRequest req = new PassengerRequest()
        {
            passengerSO = currentPassengerSO,
            pickupPoint = currentPickup,
            dropoffPoint = currentDropoff,
            reward = reward,
            distance = dist
        };

        activeRequests.Add(req);
        
        Debug.Log($"[PassengerManager] Request generated for passenger '{currentPassengerSO.name}' | Pickup: {currentPickup.name} | Dropoff: {currentDropoff.name} | Reward: {reward} | Distance: {dist:F1}m");

        // show UI (first request only)
        if (activeRequests.Count == 1)
            UIManager.Instance.ShowPassengerRequest(req);
    }

    public void AcceptRequest()
    {
        if (activeRequests.Count == 0) return;

        PassengerRequest req = activeRequests[0];
        
        // spawn passenger prefab at pickup point
        if (req.passengerSO.prefab != null)
        {
            spawnedPassengerGO = Instantiate(req.passengerSO.prefab, req.pickupPoint.position, Quaternion.identity);
            PassengerController pc = spawnedPassengerGO.GetComponent<PassengerController>();
            if (pc != null) pc.AssignRequest(req);
        }

        // mark as active
        activePassenger = true;
        currentPassengerSO = req.passengerSO;
        currentPickup = req.pickupPoint;
        currentDropoff = req.dropoffPoint;

        // hide request from UI
        UIManager.Instance.HidePassengerRequest();

        // Remove from queue
        activeRequests.RemoveAt(0);
    }

    public void DeclineRequest()
    {
        if (activeRequests.Count == 0) return;
        activeRequests.RemoveAt(0);
        UIManager.Instance.HidePassengerRequest();
    }

    // called by PickupZone when car is stopped in pickup and passenger exists
    public void OnPickedUp(GameObject passengerGO = null)
    {
        // remove passenger object in world
        if (spawnedPassengerGO != null) Destroy(spawnedPassengerGO);
        spawnedPassengerGO = null;

        // Start rating decay timer while passenger is in car
        UIManager.Instance.SetPassengerInCar(true, currentPassengerSO);
    }

    // called by DropoffZone when dropped off successfully
    public void OnDroppedOff()
    {
        if (!activePassenger) return;

        activePassenger = false;
        
        // reward
        int pay = GameManager.Instance.GetEffectiveFare(currentPassengerSO.baseFare);
        GameManager.Instance.AddMoney(pay);
        GameManager.Instance.AddTime(currentPassengerSO.timeBonus);

        // play happy voiceline if any
        if (currentPassengerSO.happyVoicelines != null && currentPassengerSO.happyVoicelines.Length > 0)
        {
            AudioSource.PlayClipAtPoint(currentPassengerSO.happyVoicelines[Random.Range(0, currentPassengerSO.happyVoicelines.Length)], Camera.main.transform.position);
        }

        UIManager.Instance.SetPassengerInCar(false, null);
    }

    // called externally when player damages car while passenger present
    public void OnPassengerAnnoyedByDamage()
    {
        // immediate star loss
        GameManager.Instance.LoseStar(1);
        // play angry voiceline
        if (currentPassengerSO != null && currentPassengerSO.angryVoicelines.Length > 0)
            AudioSource.PlayClipAtPoint(currentPassengerSO.angryVoicelines[Random.Range(0, currentPassengerSO.angryVoicelines.Length)], Camera.main.transform.position);

        // if pay or request frequency should be penalized, GameManager handles via stars
    }
    
    public bool IsActivePickup(Transform pickup)
    {
        return currentPickup != null && pickup == currentPickup;
    }
    
    public bool IsActiveDropoff(Transform dropoff)
    {
        return currentDropoff != null && dropoff == currentDropoff;
    }

    // small data holder
    public class PassengerRequest
    {
        public PassengerSO passengerSO;
        public Transform pickupPoint;
        public Transform dropoffPoint;
        public int reward;
        public float distance;
    }
}