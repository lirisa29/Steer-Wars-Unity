using UnityEngine;
using UnityEngine.InputSystem;
using static InputActions;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    private InputActions inputActions;

    public Vector2 Player1Move => inputActions.Player.MoveP1.ReadValue<Vector2>();
    public Vector2 Player2Move => inputActions.Player.MoveP2.ReadValue<Vector2>();

    void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputActions();
            inputActions.Player.SetCallbacks(this);
        }

        inputActions.Enable();
    }

    public void Enable()
    {
        inputActions?.Enable();
    }

    public void OnDisable()
    {
        inputActions?.Disable();
    }

    // Required by IPlayerActions
    public void OnMoveP1(InputAction.CallbackContext context) {}
    public void OnMoveP2(InputAction.CallbackContext context) {}
}
