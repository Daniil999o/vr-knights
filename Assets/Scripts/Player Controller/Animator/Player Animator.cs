using UnityEngine;

public class PlayerAnimator : MonoBehaviour{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private MovementSettingsSO movementSettings;

    private int _horizontalHash;
    private int _verticalHash;
    private int _walkTypeHash;

    private float _horizontalVelocity;
    private float _verticalVelocity;
    private float _horizontalSmooth;
    private float _verticalSmooth;

    private float _walkType;

    private void Awake() {
        _horizontalHash = Animator.StringToHash("Horizontal");
        _verticalHash = Animator.StringToHash("Vertical");
        _walkTypeHash = Animator.StringToHash("Walk Type");
    }

    private void Update() {
        float speed = playerMovement.RealSpeed;

        float walkType = GetWalkType(speed);

        _walkType = Mathf.MoveTowards(_walkType, walkType, movementSettings.smoothWalkTypeSpeed * Time.deltaTime);

        Vector2 input = playerMovement.InputDirection;
        float horizontal = input.x;
        float vertical = input.y;

        _horizontalVelocity = Mathf.MoveTowards(_horizontalVelocity, horizontal,
            movementSettings.smoothVelocitySpeed * Time.deltaTime
        );
        _verticalVelocity = Mathf.MoveTowards(_verticalVelocity, vertical,
            movementSettings.smoothVelocitySpeed * Time.deltaTime
        );

        animator.SetFloat(_horizontalHash, _horizontalVelocity);
        animator.SetFloat(_verticalHash, _verticalVelocity);
        animator.SetFloat(_walkTypeHash, _walkType);

        animator.SetFloat("Speed", speed);
    }

    private float GetWalkType(float speed) {
        float walkType;
        
        float walkThreshold = movementSettings.walkSpeed * 1.1f;
        float runThreshold = movementSettings.runSpeed * 1.1f;
        float sprintThreshold = movementSettings.sprintSpeed;
        
        if (speed <= walkThreshold)
            walkType = 0f;
        else if (speed >= sprintThreshold)
            walkType = 2f;
        else if (speed < runThreshold)
            walkType = Mathf.InverseLerp(walkThreshold, runThreshold, speed);
        else
            walkType = 1f + Mathf.InverseLerp(runThreshold, sprintThreshold, speed);

        walkType = Mathf.Clamp(walkType, 0f, 2f);

        return walkType;
    }
}