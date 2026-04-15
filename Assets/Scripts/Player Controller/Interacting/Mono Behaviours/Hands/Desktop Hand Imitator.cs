using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(PlayerHand))]
public class DesktopHandImitator : MonoBehaviour
{
    [SerializeField] private DesktopInteractionHandler interactionHandler;
    
    [Space]
    
    [SerializeField] private TwoBoneIKConstraint ikConstraint;
    [SerializeField] private Transform interactPoint;
    [SerializeField] private Collider interactCollider;
    
    private Transform _transform;
    private PlayerHand _hand;

    public PlayerHand Hand => _hand;
    
    public bool TryGrab { get; set; }
    
    public Transform HandPoint { get; set; }

    public void ResetAll() {
        HandPoint = null;
        TryGrab = false;
        Hand.Grab = false;
        Hand.Pinch = false;
    }

    public void ArmHand(Transform point) {
        HandPoint = point;
        TryGrab = false;
        Hand.Grab = true;
        Hand.Pinch = true;
    }

    private float _handWeight;

    private Vector3 _needPosition;
    private Vector3 _needRotation;

    private void Awake() {
        _transform = GetComponent<Transform>();
        _hand = GetComponent<PlayerHand>();
    }

    private void Update() {
        if (HandPoint) {
            _needPosition = HandPoint.position;
            _needRotation = HandPoint.eulerAngles;

            ikConstraint.weight = 1f;
            
            interactCollider.enabled = false;
            
            _transform.position = HandPoint.position;
            _transform.rotation = HandPoint.rotation;
            
            return;
        }

        TryHandGrab();
        
        ikConstraint.weight = Mathf.MoveTowards(ikConstraint.weight, _handWeight, 2f * Time.deltaTime);
        
        _transform.position = Vector3.MoveTowards(_transform.position, _needPosition, 2f * Time.deltaTime);
        _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.Euler(_needRotation), 2f * Time.deltaTime);
    }

    private void TryHandGrab() {
        _hand.Grab = TryGrab;

        interactCollider.enabled = TryGrab;
        
        if (!TryGrab) {
            _handWeight = 0f;
            _needPosition = interactPoint.position;
            _needRotation = interactPoint.eulerAngles;
            return;
        }

        _handWeight = 1f;
        
        _needPosition = GetNearestItemPosition();
        _needRotation = Vector3.zero;
    }

    private Vector3 GetNearestItemPosition() {
        InteractOptionsSO options = interactionHandler.InteractOptions;

        Collider[] sphereOverlaps =
            Physics.OverlapSphere(_transform.position, options.interactRadius, options.interactLayerMask);

        if (sphereOverlaps.Length <= 0) return interactPoint.position;

        IGrabbable[] grabbables = sphereOverlaps
            .OrderBy(x => Vector3.Distance(x.ClosestPoint(_transform.position), _transform.position))
            .Select(x => x.GetComponent<IGrabbable>()).ToArray();

        if (grabbables.Length <= 0) return interactPoint.position;
        
        IGrabbable grabbable = grabbables.FirstOrDefault(x => x is MonoBehaviour && !x.IsGrabbed);
        
        return grabbable is not MonoBehaviour beh ? interactPoint.position : beh.transform.position;
    }
}