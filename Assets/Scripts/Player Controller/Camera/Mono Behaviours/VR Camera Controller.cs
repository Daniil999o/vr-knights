using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VRCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform stickRotateBody;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputActionReference turnAction;

    [Header("Turning")]
    [SerializeField] private float turnSpeed = 60f;
    [SerializeField] private bool snapTurn = false;
    [SerializeField] private float snapAngle = 45f;
    [SerializeField] private float turnDeadzone = 0.1f;

    [Header("Auto Follow")]
    [SerializeField] private bool autoFollow = true;
    [SerializeField] private float followSpeed = 30f;
    [SerializeField] private float followDeadzoneAngle = 5f;

    private void OnEnable()
    {
        if (turnAction != null)
            turnAction.action.Enable();
    }

    private void OnDisable()
    {
        if (turnAction != null)
            turnAction.action.Disable();
    }

    private void Update()
    {
        if (Keyboard.current?.f5Key.wasPressedThisFrame == true)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        HandleTurnInput();
        HandleAutoFollow();
    }

    private void HandleTurnInput()
    {
        if (turnAction == null) return;

        Vector2 turnInput = turnAction.action.ReadValue<Vector2>();
        float turnAmount = turnInput.x;

        if (Mathf.Abs(turnAmount) < turnDeadzone)
            return;

        if (snapTurn)
        {
            if (Mathf.Abs(turnAmount) > 0.5f)
            {
                float snap = Mathf.Sign(turnAmount) * snapAngle;
                stickRotateBody.Rotate(0, snap, 0);
            }
        }
        else
        {
            float rotation = turnAmount * turnSpeed * Time.deltaTime;
            stickRotateBody.Rotate(0, rotation, 0);
        }
    }

    private void HandleAutoFollow()
    {
        if (!autoFollow || cameraTransform == null || playerBody == null)
            return;

        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;

        if (cameraForward.sqrMagnitude < 0.001f)
            return;

        cameraForward.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);

        float angle = Quaternion.Angle(playerBody.rotation, targetRotation);
        if (angle < followDeadzoneAngle)
            return;

        playerBody.rotation = Quaternion.RotateTowards(
            playerBody.rotation,
            targetRotation,
            followSpeed * Time.deltaTime
        );
    }
}