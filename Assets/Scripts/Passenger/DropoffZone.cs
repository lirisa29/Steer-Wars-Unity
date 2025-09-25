using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DropoffZone : MonoBehaviour
{
    public Transform dropoffTransform;

    private void Reset()
    {
        if (dropoffTransform == null) dropoffTransform = transform;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("PlayerCar")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        // require car to be stopped
        if (rb.linearVelocity.magnitude <= 1.5f &&
            PassengerManager.Instance.ActivePassenger &&
            PassengerManager.Instance.pickedUp) // must have actually picked up
        {
            if (PassengerManager.Instance.IsActiveDropoff(dropoffTransform))
            {
                PassengerManager.Instance.OnDroppedOff();
            }
        }
    }
}
