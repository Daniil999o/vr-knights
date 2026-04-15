using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private MovementSettingsSO settings;
    
    public MovementSettingsSO Settings => settings;

    private CharacterController _controller;
    private float _currentSpeed;
    private float _currentHeight;
    private bool _isGrounded;
    private float _verticalVelocity;
    private bool _isCrouching;

    private Vector3 _currentVelocity;

    public Vector2 InputDirection { get; set; }
    public Vector3 MoveDirection { get; set; }
    public bool Sprint { get; set; }
    public bool Jump { get; set; }
    public bool Crouch { get; set; }

    public bool Grounded => _isGrounded;

    public float TargetSpeed => _currentSpeed;

    public float RealSpeed {
        get {
            Vector3 vel = _controller.velocity;

            vel.y = 0f;

            return vel.magnitude;
        }
    }

    private void Awake() {
        _controller = GetComponent<CharacterController>();
        _currentHeight = settings.standingHeight;
        _controller.height = _currentHeight;
    }

    private void Update() {
        HandleGroundCheck();
        HandleCrouch();
        HandleGravity();
        HandleMovement();
    }

    private void HandleGroundCheck() {
        _isGrounded = _controller.isGrounded || Physics.CheckSphere(
            transform.position + Vector3.down * settings.groundCheckYDistance,
            settings.groundCheckRadius,
            settings.groundMask
        );
    }

    private void HandleCrouch() {
        float targetHeight = Crouch ? settings.crouchHeight : settings.standingHeight;
        _currentHeight = Mathf.Lerp(_currentHeight, targetHeight, settings.crouchTransitionSpeed * Time.deltaTime);
        _controller.height = _currentHeight;

        Vector3 center = _controller.center;
        center.y = _currentHeight * 0.5f;
        _controller.center = center;

        _isCrouching = Crouch;
    }

    private void HandleGravity() {
        if (_isGrounded && _verticalVelocity < 0) _verticalVelocity = -2f;

        if (Jump && _isGrounded && !_isCrouching)
            _verticalVelocity = Mathf.Sqrt(settings.jumpHeight * -2f * settings.gravity);

        _verticalVelocity += settings.gravity * Time.deltaTime;
    }

    private void HandleMovement() {
        Vector2 input = InputDirection;
        Vector3 move = MoveDirection.normalized;

        bool isBack = move.magnitude < 0.01f;

        float accel = isBack ? settings.deceleration : settings.acceleration;

        float currentAcceleration = _isGrounded ? accel : accel * settings.airControl;

        float targetSpeed = Sprint ? settings.sprintSpeed : Crouch ? settings.crouchSpeed : settings.walkSpeed;

        if (Sprint) {
            if (Mathf.Abs(input.x) > 0.3f || input.y < 0f) targetSpeed = settings.runSpeed;
        }

        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, currentAcceleration * Time.deltaTime);

        float velocityAccel = isBack ? settings.velocityDeceleration : settings.velocityAcceleration;
        _currentVelocity = Vector3.Lerp(_currentVelocity, move, velocityAccel * Time.deltaTime);

        Vector3 finalMove = _currentVelocity * _currentSpeed + Vector3.up * _verticalVelocity;
        _controller.Move(finalMove * Time.deltaTime);
    }

    private void OnDrawGizmosSelected() {
        if (!_controller) _controller = GetComponent<CharacterController>();

        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * settings.groundCheckYDistance,
            settings.groundCheckRadius
        );
    }
}