using UnityEngine;

[CreateAssetMenu(fileName = "NPC Settings", menuName = "NPC/NPC Settings")]
public class NPC_Settings : ScriptableObject
{
    [Header("Agent Settings")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 6f;
    public float acceleration = 12f;
    public float walkAngular = 60f;
    public float runAngular = 120f;
    public float speedSmooth = 6f;
    public float angularSmooth = 15f;
    public float rotateToAttackSpeed = 45f;

    [Space]

    [Header("Voice Settings")]
    public Vector2 playAttackMinMaxDistance = new Vector2(3f, 8f);
    
    [Space]

    [Header("Target Settings")]
    public float distanceToRun = 6f;
    public float distanceToAttack = 2f;
    public bool autoBraking;
    public float stoppingDistance = 0.5f;
    
    [Space]

    [Header("Attacks")]
    public int meleeAttacksCount = 2;
    [Range(0f, 1f)] public float blockChanceAfterAttack = 0.4f;
    public Vector2 minMaxBlockTimer = new Vector2(1f, 5f);
    
    [Space]

    [Header("Movement Patterns")]
    public float strafeDistance = 1.2f;
    public float backstepDistance = 2.5f;
    
    [Header("Movement Speeds (for patterns)")]
    public float strafeSpeed = 2.2f;
    public float backstepSpeed = 5f;
    public float lungeSpeed = 7f;
    
    [Header("Pattern Timings")]
    public float sidestepPhase1Time = 0.2f;
    public float pingPongBackstepTime = 0.3f;
    
    [Space]
    
    [Header("Follow Patterns")]
    public float followPatternChangeInterval = 4f;
    public float flankCircleRadius = 3.5f;
    public float sprintFlankRadius = 6f;
    public float dodgeStrafeDistance = 2f;
    public float dodgeStrafeSpeed = 4f;
    public float minDistanceToStrafes = 5f;
    public float flankSpeed = 3.5f;
    public float sprintFlankSpeed = 7f;
    
    [Space]
    
    [Header("Block Patterns")]
    public float blockBackstepSpeed = 1.5f;
    public float blockBackstepDistance = 1.8f;
    public float blockStrafeSpeed = 1.2f;
    public float blockStrafeDistance = 1.2f;
    public float blockStrafeChangeTime = 1.5f;
}