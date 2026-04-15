using UnityEngine;

[CreateAssetMenu(fileName = "Movement Settings", menuName = "Player Controller/Movement Settings")]
public class MovementSettingsSO : ScriptableObject
{
    [Header("Speeds")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 1.5f;
    
    [Space]
    
    [Header("Accelerations")]
    public float acceleration = 10f;
    public float deceleration = 5f;
    public float velocityAcceleration = 10f;
    public float velocityDeceleration = 10f;
    public float airControl = 0.3f;
    
    [Space]
    
    [Header("Jump")]
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Space]
    
    [Header("Heights")]
    public float crouchHeight = 1f;
    public float standingHeight = 1.8f;
    public float crouchTransitionSpeed = 10f;

    [Space]
    
    [Header("Ground Check")]
    public float groundCheckYDistance = 0.1f;
    public float groundCheckRadius = 0.28f;
    public LayerMask groundMask = ~0;

    [Space]

    [Header("Movement Animations")]
    public float smoothVelocitySpeed = 0.5f;
    public float smoothWalkTypeSpeed = 0.5f;
}