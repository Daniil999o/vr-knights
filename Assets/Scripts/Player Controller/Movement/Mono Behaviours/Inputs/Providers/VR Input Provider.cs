using UnityEngine;
using UnityEngine.InputSystem;

public class VRInputProvider : MonoBehaviour, IInputProvider
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Camera xrCamera;

    private InputActionMap _leftHandMap;
    private InputActionMap _rightHandMap;
    private InputAction _leftMove;
    private InputAction _sprintAction;
    private InputAction _jumpAction;
    private InputAction _crouchAction;

    private void Awake() {
        _leftHandMap = inputActions.FindActionMap("XRI Left Locomotion");
        _rightHandMap = inputActions.FindActionMap("XRI Right Locomotion");

        _leftMove = _leftHandMap.FindAction("Move");
        _sprintAction = _leftHandMap.FindAction("Sprint");
        _jumpAction = _rightHandMap.FindAction("Jump");
        _crouchAction = _rightHandMap.FindAction("Crouch");

        _leftHandMap.Enable();
        _rightHandMap.Enable();

        if (xrCamera == null) xrCamera = Camera.main;
    }

    public Vector3 GetMoveDirection() {
        Vector2 input = _leftMove.ReadValue<Vector2>();
        if (input.sqrMagnitude < 0.01f) return Vector3.zero;

        Vector3 forward = xrCamera.transform.forward;
        Vector3 right = xrCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        return (forward * input.y + right * input.x).normalized;
    }

    public Vector2 GetInputDirection() => _leftMove.ReadValue<Vector2>();

    public bool GetSprint() => _sprintAction?.IsPressed() ?? false;
    public bool GetJump() => _jumpAction?.WasPressedThisFrame() ?? false;
    public bool GetCrouch() => _crouchAction?.IsPressed() ?? false;
}