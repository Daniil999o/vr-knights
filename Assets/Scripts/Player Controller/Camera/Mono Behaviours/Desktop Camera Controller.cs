using UnityEngine;
using UnityEngine.InputSystem;

public class DesktopCameraController : MonoBehaviour
{
    [SerializeField] private CameraSettingsSO settings;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Transform playerBody;

    public float LookAbsoluteModifier { get; set; } = 1f;

    private InputActionMap _gameplayMap;
    private InputAction _lookAction;
    private float _xRotation;

    private void Awake()
    {
        _gameplayMap = inputActions.FindActionMap("Player");
        _lookAction = _gameplayMap.FindAction("Look");
        _gameplayMap.Enable();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Vector2 lookInput = _lookAction.ReadValue<Vector2>() * LookAbsoluteModifier;

        float mouseX = lookInput.x * settings.horizontalSensitivity;
        float mouseY = lookInput.y * settings.verticalSensitivity * (settings.invertVertical ? -1 : 1);

        playerBody.Rotate(Vector3.up * mouseX);

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -settings.maxVerticalAngle, settings.maxVerticalAngle);
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0);
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}