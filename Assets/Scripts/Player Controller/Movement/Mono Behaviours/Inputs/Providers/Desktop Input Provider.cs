using UnityEngine;
using UnityEngine.InputSystem;

public class DesktopInputProvider : MonoBehaviour, IInputProvider
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Camera playerCamera;

    private InputActionMap _gameplayMap;
    private InputAction _moveAction;
    private InputAction _sprintAction;
    private InputAction _jumpAction;
    private InputAction _crouchAction;

    private void Awake() {
        _gameplayMap = inputActions.FindActionMap("Player");

        _moveAction = _gameplayMap.FindAction("Move");
        _sprintAction = _gameplayMap.FindAction("Sprint");
        _jumpAction = _gameplayMap.FindAction("Jump");
        _crouchAction = _gameplayMap.FindAction("Crouch");

        _gameplayMap.Enable();

        if (playerCamera == null) playerCamera = Camera.main;
    }

    public Vector3 GetMoveDirection() {
        Vector2 input = _moveAction.ReadValue<Vector2>();
        if (input.sqrMagnitude < 0.01f) return Vector3.zero;

        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        return (forward * input.y + right * input.x).normalized;
    }

    public Vector2 GetInputDirection() => _moveAction.ReadValue<Vector2>();

    public bool GetSprint() => _sprintAction.IsPressed();
    public bool GetJump() => _jumpAction.WasPressedThisFrame();
    public bool GetCrouch() => _crouchAction.IsPressed();
}