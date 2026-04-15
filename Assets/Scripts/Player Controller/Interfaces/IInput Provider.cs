using UnityEngine;

public interface IInputProvider
{
    Vector3 GetMoveDirection();
    Vector2 GetInputDirection();
    
    bool GetSprint();
    bool GetJump();
    bool GetCrouch();
}