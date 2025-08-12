using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -10f);
    [SerializeField] private float followSmoothness = 5f;
    [SerializeField] private float rotationSmoothness = 5f;

    private void LateUpdate()
    {
        if (target == null) return;
        
        Vector3 desiredPosition = target.position + offset;
        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmoothness * Time.deltaTime);

        Vector3 lookTarget = target.position + Vector3.up * 2f;
        
        Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothness * Time.deltaTime);
    }
}
