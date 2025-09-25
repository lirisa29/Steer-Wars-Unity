using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupZone : MonoBehaviour
{
    public Transform pickupTransform; // optional; default to this.transform if null
    public float stopSpeedThreshold = 1.5f; // how slow the car must be to count as stopped
    public bool hasAssignedPassenger = false; // read-only, PassengerManager manages real assignment

    private void Reset()
    {
        if (pickupTransform == null) pickupTransform = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerCar")) return;
        // If there's an assigned passenger for this pickup point:
        // We will rely on PassengerManager to manage matching; this just checks velocity and calls manager
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("PlayerCar")) return;
        if (PassengerManager.Instance.pickedUp == true) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        // Require the car to be stopped
        if (rb.linearVelocity.magnitude <= stopSpeedThreshold && PassengerManager.Instance.ActivePassenger == true)
        {
            // Ask the manager if THIS pickup zone is the active one
            if (PassengerManager.Instance != null && PassengerManager.Instance.IsActivePickup(pickupTransform))
            {
                PassengerManager.Instance.OnPickedUp();
            }
        }
    }
}
