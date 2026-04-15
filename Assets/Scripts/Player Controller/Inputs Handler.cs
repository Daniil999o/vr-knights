using UnityEngine;
using UnityEngine.XR;

public class InputsHandler : MonoBehaviour
{
    [SerializeField] private bool testXR;

    [Space]
    [SerializeField] private PlayerMovement playerMovement;

    [Space]
    [SerializeField] private DesktopCameraController desktopCamera;

    [SerializeField] private VRCameraController xrCamera;

    private IInputProvider _currentProvider;

    private void Awake() {
        if (testXR || XRSettings.isDeviceActive)
            SetVRProvider();
        else
            SetDesktopProvider();
    }

    private void Update() {
        if (_currentProvider == null) return;

        playerMovement.InputDirection = _currentProvider.GetInputDirection();
        playerMovement.MoveDirection = _currentProvider.GetMoveDirection();
        playerMovement.Sprint = _currentProvider.GetSprint();
        playerMovement.Jump = _currentProvider.GetJump();
        playerMovement.Crouch = _currentProvider.GetCrouch();
    }

    public void SetVRProvider() {
        _currentProvider = GetComponent<VRInputProvider>();

        xrCamera.enabled = true;
        desktopCamera.enabled = false;
    }

    public void SetDesktopProvider() {
        _currentProvider = GetComponent<DesktopInputProvider>();

        xrCamera.enabled = false;
        desktopCamera.enabled = true;
    }
}