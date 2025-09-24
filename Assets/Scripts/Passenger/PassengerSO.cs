using UnityEngine;

[CreateAssetMenu(menuName = "Passengers/PassengerSO")]
public class PassengerSO : ScriptableObject
{
    public Sprite portrait;
    public GameObject prefab; // prefab that contains PassengerController
    public int baseFare = 100;
    public float timeBonus = 8f; // seconds added on successful drop-off
}
