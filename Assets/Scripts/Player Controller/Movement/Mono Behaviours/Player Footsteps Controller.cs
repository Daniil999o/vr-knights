using UnityEngine;
using Player_System.Movement.FootstepsSystem;

public class PlayerFootstepsController : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Footsteps footsteps;
    
    [Space]

    [Header("Step Distances")]
    [SerializeField] private float walkStepDistance = 0.75f;
    [SerializeField] private float runStepDistance = 1.2f;
    [SerializeField] private float sprintStepDistance = 1.8f;

    private MovementSettingsSO _settings;
    
    private float _distanceSinceLastStep;
    private Vector3 _lastPosition;

    private Transform _transform;

    private void Start() {
        _transform = GetComponent<Transform>();
        _settings = movement.Settings;
        _lastPosition = _transform.position;
    }
    
    private void Update() {
        float speed = movement.RealSpeed;
        
        footsteps.ControllerSpeed = speed;

        Vector3 currentPosition = _transform.position;
        float deltaDistance = Vector3.Distance(currentPosition, _lastPosition);
        _distanceSinceLastStep += deltaDistance;
        _lastPosition = currentPosition;

        if (!(_distanceSinceLastStep > 0f)) return;
        
        StepType stepType = GetCurrentStepType(speed);
        
        float requiredDistance = GetStepDistanceForType(stepType, speed);

        while (_distanceSinceLastStep >= requiredDistance && requiredDistance > 0f) {
            PlayStepSound(stepType);
            
            _distanceSinceLastStep -= requiredDistance;
            
            stepType = GetCurrentStepType(speed);
            requiredDistance = GetStepDistanceForType(stepType, speed);
        }
    }

    private StepType GetCurrentStepType(float speed) =>
        speed > _settings.walkSpeed ? StepType.Run : StepType.Walk;

    private float GetStepDistanceForType(StepType type, float speed) {
        if (speed < 0.1f) return float.MaxValue;

        return type switch {
            StepType.Walk => walkStepDistance, StepType.Run => runStepDistance,
            var _ => sprintStepDistance
        };
    }

    private void PlayStepSound(StepType type) {
        if (!movement.Grounded) return;
        
        footsteps.PlayFootStep(type);
    }
}