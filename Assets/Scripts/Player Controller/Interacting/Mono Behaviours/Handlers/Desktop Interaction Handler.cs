using UnityEngine;
using UnityEngine.InputSystem;

public class DesktopInteractionHandler : MonoBehaviour
{
    [SerializeField] private InteractOptionsSO settings;
    
    public InteractOptionsSO InteractOptions => settings;

    [Space]
    
    [SerializeField] private InputActionReference grabRightAction;
    [SerializeField] private InputActionReference grabLeftAction;

    [Space]
    
    [SerializeField] private Hand rightHand;
    [SerializeField] private Hand leftHand;

    [Space]

    [SerializeField] private DesktopCameraController desktopCamera;

    [System.Serializable]
    private struct Hand
    {
        public bool isRight;
        public PlayerHand hand;
        public DesktopHandImitator imitator;
    }

    private void OnEnable() {
        grabRightAction.action.Enable();
        grabLeftAction.action.Enable();
    }

    private void OnDisable() {
        grabRightAction.action.Disable();
        grabLeftAction.action.Disable();
    }

    private void Update() {
        bool grabRight = grabRightAction.action.IsPressed();
        bool grabLeft = grabLeftAction.action.IsPressed();

        rightHand.imitator.TryGrab = grabRight;
        leftHand.imitator.TryGrab = grabLeft;
    }
}