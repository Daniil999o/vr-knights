using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCamera : MonoBehaviour
{
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference flyAction;
    [SerializeField] private InputActionReference runAction;
    
    [Space]
    
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private Vector2 lookClamp = new Vector2(-90f, 90f);
    
    [Space]
    
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float acceleration = 2f;

    private float _currentSpeed;

    private Transform _transform;

    private void Awake() {
        _transform = GetComponent<Transform>();
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void OnEnable() {
        lookAction.action.Enable();
        moveAction.action.Enable();
        flyAction.action.Enable();
        runAction.action.Enable();
    }

    private void OnDisable() {
        lookAction.action.Disable();
        moveAction.action.Disable();
        flyAction.action.Disable();
        runAction.action.Disable();
    }

    private void Update() {
        Vector2 look = lookAction.action.ReadValue<Vector2>();
        
        Vector3 lookDirection = new Vector3(-look.y, look.x, 0f);
        
        _transform.Rotate(lookDirection * lookSensitivity * Time.deltaTime);

        Vector3 rot = _transform.eulerAngles;
        float angleX = rot.x > 180 ? rot.x - 360 : rot.x;
        rot.x = Mathf.Clamp(angleX, lookClamp.x, lookClamp.y);
        rot.z = 0f;
        _transform.rotation = Quaternion.Euler(rot);
        
        Vector2 move = moveAction.action.ReadValue<Vector2>();
        float fly = flyAction.action.ReadValue<float>();
        
        bool isRun = runAction.action.IsPressed();

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, isRun ? runSpeed : moveSpeed, acceleration * Time.deltaTime);
        
        Vector3 moveDirection = new Vector3(move.x, fly, move.y);
        moveDirection.Normalize();
        
        _transform.Translate(moveDirection * _currentSpeed * Time.deltaTime);
    }
}