using UnityEngine;

public class PassengerController : MonoBehaviour
{
    [HideInInspector] public PassengerManager.PassengerRequest assignedRequest;
    [HideInInspector] public PassengerSO passengerSO;

    private bool assigned;

    public void AssignRequest(PassengerManager.PassengerRequest req)
    {
        assignedRequest = req;
        passengerSO = req.passengerSO;
        assigned = true;
    }

    // When the player "picks up", the PassengerManager destroys this object.
    // Optionally could play a sound or particle before disappearing.
}
