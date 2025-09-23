using UnityEngine;

public class CarController : MonoBehaviour, ICarController
{
    [Header("Axle Info")] 
    [SerializeField] public Transform frontLeftTire;
    [SerializeField] public Transform frontRightTire;
    [SerializeField] public Transform backLeftTire;
    [SerializeField] public Transform backRightTire;

    [Header("Motor Attributes")] 
    [SerializeField] private float maxSpeed;
    private float realSpeed; // Actual speed
    private float currentSpeed = 0; // Applied speed
    
    [Header("Steer Attributes")]
    private float steerDirection;
    
    [Header("Refs")]
    [SerializeField] private InputReader input;
    Rigidbody rb;
   
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        input.Enable();
    }

    void FixedUpdate()
    {
        HandleMotor(); // Controls movement
        HandleSteering(); // Controls turning
        TireSteer(); // Adjusts tire rotation visually
    }

    private void HandleMotor()
    {
        Vector2 p1Input = input.Player1Move;
        Vector2 p2Input = input.Player2Move;

        // Require BOTH players to press the same direction for movement
        bool bothForward = p1Input.y > 0.5f && p2Input.y > 0.5f;
        bool bothBackward = p1Input.y < -0.5f && p2Input.y < -0.5f;

        realSpeed = transform.InverseTransformDirection(rb.linearVelocity).z;

        if (bothForward)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime * 0.5f);
        }
        else if (bothBackward)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, -maxSpeed / 1.75f, Time.deltaTime * 1f);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 1.5f);
        }

        Vector3 vel = transform.forward * currentSpeed;
        vel.y = rb.linearVelocity.y;
        rb.linearVelocity = vel;
    }

    private void HandleSteering()
    {
        Vector2 p1Input = input.Player1Move;
        Vector2 p2Input = input.Player2Move;

        // Require both players to steer in same direction
        if (Mathf.Sign(p1Input.x) == Mathf.Sign(p2Input.x) && Mathf.Abs(p1Input.x) > 0.5f && Mathf.Abs(p2Input.x) > 0.5f)
        {
            steerDirection = (p1Input.x + p2Input.x) / 2f; // average input
        }
        else
        {
            steerDirection = 0f; // no steering if inputs don't match
        }

        float steerAmount = (realSpeed > 30) ? (realSpeed / 3) * steerDirection : (realSpeed / 1.4f) * steerDirection;
        Vector3 steerDirVect = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + steerAmount, transform.eulerAngles.z);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, steerDirVect, 10 * Time.deltaTime);
    }
    
    private void TireSteer()
    {
        Vector2 p1Input = input.Player1Move;
        Vector2 p2Input = input.Player2Move;

        // --- FRONT TIRES (Player 1) ---
        float frontTargetAngle = Mathf.Lerp(0f, 25f, Mathf.Abs(p1Input.x));
        if (p1Input.x < 0) frontTargetAngle *= -1f;

        frontLeftTire.localRotation = Quaternion.Lerp(
            frontLeftTire.localRotation,
            Quaternion.Euler(0, frontTargetAngle, 0),
            5 * Time.deltaTime
        );
        frontRightTire.localRotation = Quaternion.Lerp(
            frontRightTire.localRotation,
            Quaternion.Euler(0, frontTargetAngle, 0),
            5 * Time.deltaTime
        );

        // --- REAR TIRES (opposite steer) ---
        float backTargetAngle = 0f;
        if (Mathf.Abs(p2Input.x) > 0.1f)
        {
            // Opposite direction of P2 input
            if (p2Input.x < 0) backTargetAngle = 25f;   // P2 left → tires right
            else if (p2Input.x > 0) backTargetAngle = -25f; // P2 right → tires left
        }

        backLeftTire.localRotation = Quaternion.Lerp(
            backLeftTire.localRotation,
            Quaternion.Euler(0, backTargetAngle, 0),
            5 * Time.deltaTime
        );
        backRightTire.localRotation = Quaternion.Lerp(
            backRightTire.localRotation,
            Quaternion.Euler(0, backTargetAngle, 0),
            5 * Time.deltaTime
        );

        // --- Tire Spin Animation ---
        float spinSpeed = (currentSpeed > 30) ? currentSpeed : realSpeed;
        frontLeftTire.GetChild(0).Rotate(-90 * Time.deltaTime * spinSpeed * 0.5f, 0, 0);
        frontRightTire.GetChild(0).Rotate(-90 * Time.deltaTime * spinSpeed * 0.5f, 0, 0);
        backLeftTire.GetChild(0).Rotate(-90 * Time.deltaTime * spinSpeed * 0.5f, 0, 0);
        backRightTire.GetChild(0).Rotate(-90 * Time.deltaTime * spinSpeed * 0.5f, 0, 0);
    }
}
