using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    [Header("Arrow Settings")]
    public MeshRenderer arrowRenderer;
    public Color pickupColor = Color.green;
    public Color dropoffColor = Color.cyan;

    private Transform target; // pickup or dropoff
    private bool active = false;

    void Update()
    {
        if (!active || target == null) return;

        // rotate arrow towards target (ignore vertical tilt)
        Vector3 direction = target.position - transform.position;
        direction.y = 0; // keep only horizontal rotation
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public void PointToPickup(Transform pickupPoint)
    {
        target = pickupPoint;
        active = true;

        if (arrowRenderer != null)
        {
            arrowRenderer.enabled = true;
            arrowRenderer.material.color = pickupColor;
        }
    }

    public void PointToDropoff(Transform dropoffPoint)
    {
        target = dropoffPoint;
        active = true;

        if (arrowRenderer != null)
        {
            arrowRenderer.enabled = true;
            arrowRenderer.material.color = dropoffColor;
        }
    }

    public void DisableArrow()
    {
        active = false;
        target = null;

        if (arrowRenderer != null)
            arrowRenderer.enabled = false;
    }
}
