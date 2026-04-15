using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour, IGrabbable
{
    [field: SerializeField] public bool IsGrabbed { get; set; }

    [field: Space]

    [SerializeField] private GrabbableObjectSO settings;
    
    public GrabbableObjectSO Settings => settings;

    [Space]

    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;

    public Transform RightHand => rightHand;
    public Transform LeftHand => leftHand;

    [Space]

    [Header("Events")]
    [SerializeField] private UnityEvent<bool> onSetGrabbed;
    [SerializeField] private UnityEvent<bool> onSetGrabbedInverse;
    [SerializeField] private UnityEvent<bool> onSetFreeze;

    [Space]

    [SerializeField] private bool test;

    public bool Freeze {
        get => _rb.isKinematic;
        set {
            _rb.isKinematic = value;
            _coll.enabled = !value;
            
            onSetFreeze?.Invoke(value);
        }
    }

    public Rigidbody Rigid => _rb;

    private Rigidbody _rb;
    private Collider _coll;
    
    private Joint _myJoint;

    public void SetControlLive(LiveInfo liveInfo) {
        if (!TryGetComponent(out HandWeapon wp)) return;

        wp.ControlLive = liveInfo;
    }

    private void Awake() {
        _rb = GetComponent<Rigidbody>();

        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rb.interpolation = RigidbodyInterpolation.None;

        _coll = GetComponent<Collider>();

        _rb.mass = settings.weight;
    }

    public void Grab(Joint joint) {
        if (IsGrabbed) return;

        IsGrabbed = true;

        _rb.isKinematic = true;
        
        _myJoint = joint;
        joint.connectedBody = _rb;

        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        joint.autoConfigureConnectedAnchor = true;

        if (joint is ConfigurableJoint cJoint) {
            InHandPoint handPoint = settings.handPoint;

            cJoint.configuredInWorldSpace = false;
            
            cJoint.targetPosition = handPoint.Pos;
            cJoint.targetRotation = Quaternion.Euler(handPoint.Rot);
        }

        _rb.isKinematic = false;
        
        onSetGrabbed?.Invoke(true);
        onSetGrabbedInverse?.Invoke(false);
    }
    
    public void EndGrab() {
        if (!IsGrabbed) return;

        if (_myJoint) _myJoint.connectedBody = null;

        _rb.interpolation = RigidbodyInterpolation.None;
        
        onSetGrabbed?.Invoke(false);
        onSetGrabbedInverse?.Invoke(true);
    }

    public void DropAtPoint(Transform point) {
        transform.position = point.position;
        transform.rotation = point.rotation;
        
        EndGrab();
    }

    private void Update() {
        if (!test) return;

        if (!_myJoint || _myJoint is not ConfigurableJoint cJoint) return;
        
        InHandPoint handPoint = settings.handPoint;
            
        cJoint.targetPosition = handPoint.Pos;
        cJoint.targetRotation = Quaternion.Euler(handPoint.Rot);
    }
}