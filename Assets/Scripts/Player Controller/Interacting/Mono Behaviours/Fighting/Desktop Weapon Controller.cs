using UnityEngine;
using UnityEngine.InputSystem;

public class DesktopWeaponController : MonoBehaviour
{
    [SerializeField] private Transform wpTarget;
    [SerializeField] private Joint joint;
    
    [Space]
    
    [SerializeField] private Transform shieldTarget;
    [SerializeField] private Joint shieldJoint;

    [Space]
    
    [SerializeField] private DesktopHandImitator rightHand;
    [SerializeField] private DesktopHandImitator leftHand;

    [Space]
    
    [SerializeField] private InputActionReference mouseLookAction;
    [SerializeField] private InputActionReference leftAction;
    [SerializeField] private InputActionReference rightAction;
    [SerializeField] private InputActionReference altAttackAction;

    [Space]
    
    [SerializeField] private DesktopCameraController desktopCameraController;

    [Space]
    
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private WeaponPoseData defaultPoseData;
    [SerializeField] private WeaponAttackSettings defaultAttackSettings;
    
    private WeaponPoseData _poseData;
    private WeaponAttackSettings _attackSettings;
    
    private HandWeapon _handWeapon;
    private Transform _weaponT;

    private Shield _shield;

    private enum SlashDirection { None, Left, Right, Up }

    private SlashDirection _currentAttack = SlashDirection.None;
    private SlashDirection _currentWindup = SlashDirection.None;
    
    private RuntimePose _currentPose;
    private Vector2 _currentInput;

    private PoseDef _attackPose;

    private RuntimePose _currentShieldPose;

    private void OnEnable() {
        mouseLookAction.action.Enable();
        leftAction.action.Enable();
        rightAction.action.Enable();
        altAttackAction.action.Enable();
    }

    private void OnDisable() {
        mouseLookAction.action.Disable();
        leftAction.action.Disable();
        rightAction.action.Disable();
        altAttackAction.action.Disable();
    }

    public void SetGrabbable(GrabbableObject grabbable) {
        _handWeapon = null;
        _weaponT = null;

        _poseData = defaultPoseData;
        _attackSettings = defaultAttackSettings;

        grabbable.Rigid.isKinematic = true;

        Transform grabT = grabbable.transform;

        grabT.SetParent(wpTarget);

        grabT.localPosition = Vector3.zero;
        grabT.localRotation = Quaternion.identity;

        grabT.SetParent(null);

        grabbable.Rigid.isKinematic = false;
        
        if (grabbable.Settings.armedHand is ArmedHand.Right or ArmedHand.Both) {
            rightHand.ResetAll();
            rightHand.ArmHand(grabbable.RightHand);
        }

        if (grabbable.Settings.armedHand is ArmedHand.Left or ArmedHand.Both) {
            leftHand.ResetAll();
            leftHand.ArmHand(grabbable.LeftHand);
        }

        if (grabbable.TryGetComponent(out HandWeapon handWeapon)) {
            _handWeapon = handWeapon;
            _weaponT = handWeapon.transform;

            _handWeapon.CanAttack = false;

            _attackSettings = handWeapon.Settings.attackSettings;
            _poseData = handWeapon.Settings.poseData;
        }

        grabbable.Grab(joint);
    }

    public void SetShield(GrabbableObject grabbable) {
        _shield = null;

        grabbable.Rigid.isKinematic = true;

        Transform grabT = grabbable.transform;

        grabT.SetParent(shieldTarget);

        grabT.localPosition = Vector3.zero;
        grabT.localRotation = Quaternion.identity;

        grabT.SetParent(null);

        grabbable.Rigid.isKinematic = false;
        
        if (grabbable.Settings.armedHand is ArmedHand.Right or ArmedHand.Both) {
            rightHand.ResetAll();
            rightHand.ArmHand(grabbable.RightHand);
        }

        if (grabbable.Settings.armedHand is ArmedHand.Left or ArmedHand.Both) {
            leftHand.ResetAll();
            leftHand.ArmHand(grabbable.LeftHand);
        }

        if (grabbable.TryGetComponent(out Shield shield)) {
            _shield = shield;
        }

        grabbable.Grab(shieldJoint);
    }

    private void Update() {
        bool rightPressed = rightAction.action.IsPressed();
        
        if (_shield) {
            PoseDef shieldPose = rightPressed ? _shield.Settings.shieldUp : _shield.Settings.shieldIdle;
            float shieldSpeed = rightPressed ? _shield.Settings.shieldUpSpeed : _shield.Settings.shieldIdleSpeed;

            _currentShieldPose =
                RuntimePose.Lerp(_currentShieldPose, shieldPose.ToRuntime(), shieldSpeed * Time.deltaTime);

            _shield.Blocking = rightPressed;
            
            shieldTarget.localPosition = _currentShieldPose.pos;
            shieldTarget.localRotation = _currentShieldPose.rot;
        }
        
        if (!_handWeapon) return;

        if (_attackSettings == null || !_poseData) return;

        bool leftPressed = leftAction.action.IsPressed();
        bool altAttack = altAttackAction.action.IsPressed();
        Vector2 mouseDelta = mouseLookAction.action.ReadValue<Vector2>();

        desktopCameraController.LookAbsoluteModifier = leftPressed ? 0.05f : 1f;

        _handWeapon.Blocking = false;

        if (rightPressed) {
            if (!_shield) {
                RuntimePose pose = _poseData.blockPose.ToRuntime();

                _currentPose = RuntimePose.Lerp(_currentPose,
                    pose,
                    _attackSettings.blockSpeed * Time.deltaTime
                );

                _handWeapon.Blocking = true;

                SetWeaponTransform(_currentPose.pos, _currentPose.rot);
            }
        } else if (leftPressed) {
            float inputSpeed;

            float attackSpeed = mouseDelta.magnitude;

            _currentInput += mouseDelta * mouseSensitivity * _attackSettings.inputSmoothTime * Time.deltaTime;
            _currentInput = Vector2.ClampMagnitude(_currentInput, 1f);
            
            if (altAttack) {
                _currentWindup = SlashDirection.None;
                _currentAttack = SlashDirection.None;

                bool attacking = mouseDelta.y > 0f && _currentInput.y > 0f;
                
                _handWeapon.CanAttack = attacking;
                
                inputSpeed = attacking
                    ? _attackSettings.thrustStrikeSpeed
                    : _attackSettings.thrustWindupSpeed;
                
                RuntimePose targetPose = RuntimePose.Lerp(
                    _poseData.thrustWindup.ToRuntime(),
                    _poseData.thrustStrike.ToRuntime(),
                    _currentInput.y
                    );
                
                _currentPose = RuntimePose.Lerp(_currentPose, targetPose,
                    inputSpeed * Time.deltaTime
                );
            } else {
                _handWeapon.CanAttack = _currentAttack != SlashDirection.None;
                
                if (attackSpeed > 1f && _currentAttack == SlashDirection.None) {
                    (_attackPose, _currentAttack) = GetAttackState(_currentWindup, mouseDelta);

                    if (_currentAttack == SlashDirection.None) _attackPose = null;
                }
                
                inputSpeed = _currentAttack == SlashDirection.None
                    ? _attackSettings.windupSpeed
                    : _attackSettings.strikeSpeed;

                PoseDef myPose;

                if (_attackPose == null)
                    (myPose, _currentWindup) = GetWindupPose(_currentInput);
                else
                    myPose = _attackPose;

                RuntimePose currentPose = myPose.ToRuntime();
                
                _currentPose = RuntimePose.Lerp(_currentPose, currentPose,
                    inputSpeed * Time.deltaTime
                );

                if (_attackPose != null) {
                    if (PoseIsOver(currentPose)) {
                        _attackPose = null;
                        _currentAttack = SlashDirection.None;

                        (_, _currentWindup) = GetWindupPose(_currentInput);
                    }
                }
            }

            SetWeaponTransform(_currentPose.pos, _currentPose.rot);
        } else {
            RuntimePose pose = _poseData.idlePose.ToRuntime();
            SetWeaponTransform(pose.pos, pose.rot,
                _attackSettings.returnToIdleSpeed * 50f * Time.deltaTime
            );
            
            _handWeapon.CanAttack = false;
            _currentInput = Vector2.zero;
            _currentPose = _poseData.idlePose.ToRuntime();
            
            _currentWindup = SlashDirection.None;
            _currentAttack = SlashDirection.None;
        }
    }

    private bool PoseIsOver(PoseDef checkPose) => PoseIsOver(checkPose.ToRuntime(), 0.3f, 30f);

    private bool PoseIsOver(RuntimePose checkPose, float posT = 0.1f, float rotT = 1f) =>
        RuntimePose.Approximately(_currentPose, checkPose, posT, rotT)
        && RuntimePose.Approximately(
            wpTarget.position,
            wpTarget.rotation,
            _weaponT.position,
            _weaponT.rotation,
            posT,
            rotT
        );

    private (PoseDef, SlashDirection) GetWindupPose(Vector2 dir) {
        if (dir.y >= _attackSettings.windupVerticalZone) return (_poseData.windupTop, SlashDirection.Up);

        if (dir.x >= _attackSettings.windupHorizontalZone) return (_poseData.windupRight, SlashDirection.Right);
        
        if (dir.x <= -_attackSettings.windupHorizontalZone) return (_poseData.windupLeft, SlashDirection.Left);
        
        return (_poseData.idlePose, SlashDirection.None);
    }

    private (PoseDef, SlashDirection) GetAttackState(SlashDirection windUp, Vector2 dir) {
        // if (dir.x >= _attackSettings.windupHorizontalZone * 0.5f && windUp == SlashDirection.Left)
        //     return (_poseData.strikeLeft, SlashDirection.Left);
        //
        // if (dir.x <= -_attackSettings.windupHorizontalZone * 0.5f && windUp == SlashDirection.Right)
        //     return (_poseData.strikeRight, SlashDirection.Right);
        //
        // if (dir.y <= -_attackSettings.windupVerticalZone * 0.5f && windUp == SlashDirection.Up)
        //     return (_poseData.strikeTop, SlashDirection.Up);
        
        if (dir.x > 0 && windUp == SlashDirection.Left && PoseIsOver(_poseData.windupLeft))
            return (_poseData.strikeLeft, SlashDirection.Left);

        if (dir.x < 0 && windUp == SlashDirection.Right && PoseIsOver(_poseData.windupRight))
            return (_poseData.strikeRight, SlashDirection.Right);

        if (dir.y < 0 && windUp == SlashDirection.Up && PoseIsOver(_poseData.windupTop))
            return (_poseData.strikeTop, SlashDirection.Up);
        
        return (_poseData.idlePose, SlashDirection.None);
    }

    private void SetWeaponTransform(Vector3 pos, Quaternion rot) {
        wpTarget.localPosition = pos;
        wpTarget.localRotation = rot;
    }

    private void SetWeaponTransform(Vector3 pos, Quaternion rot, float smooth) {
        wpTarget.localPosition = Vector3.MoveTowards(wpTarget.localPosition, pos, smooth);
        wpTarget.localRotation = Quaternion.RotateTowards(wpTarget.localRotation, rot, 10f * smooth);
    }
}