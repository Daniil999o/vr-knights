using Player_System.Extensions;
using UnityEngine;

public class CollideEvent : MonoBehaviour
{
    [SerializeField] private Rigidbody mainRb;

    [Space]

    [SerializeField] private AudioSource effectsSource;
    [SerializeField] private AudioClip[] stabClips;
    [SerializeField] private AudioClip[] hitClips;

    [System.Serializable]
    private struct HitClips
    {
        public HitStrength hitStrength;
        public AudioClip[] clips;
    }
    
    private HandWeapon _handWeapon;
    private NPC_MeleeWeapon _npcWeapon;
    
    private LiveInfo _live;

    [Space]
    
    [Header("Runtime")]
    [SerializeField] private bool canAttack;
    public bool CanAttack {
        get => canAttack;
        set {
            canAttack = value;

            if (canAttack) return;
            
            _intersectionsCount = _handWeapon ? _handWeapon.Settings.maxIntersectionsCount : 1;
            Stubbing = false;
        }
    }

    private LiveInfo ControlLive => _handWeapon ? _handWeapon.ControlLive : _npcWeapon ? _npcWeapon.ControlLive : null;
    
    public bool Stubbing { get; set; }

    private int _intersectionsCount;

    private void Awake() {
        CanAttack = false;
        
        _live = GetComponentInParent<LiveInfo>(true);
        
        _handWeapon = GetComponentInParent<HandWeapon>(true);
        _npcWeapon = GetComponentInParent<NPC_MeleeWeapon>(true);
    }
    
    private void OnCollisionEnter(Collision other) {
        if (!CanAttack) return;
        
        
        if (!_handWeapon && !_npcWeapon || other.contactCount <= 0) return;

        
        Vector3 myVelocity = mainRb.linearVelocity;
        //
        // Rigidbody otherRigidbody = other.rigidbody;
        // Vector3 otherVelocity = otherRigidbody ? otherRigidbody.linearVelocity : Vector3.zero;
        //
        Vector3 impulse = myVelocity; // - otherVelocity;
        float impulseMagnitude = _npcWeapon ? 1f : impulse.magnitude;

        if (Stubbing && _handWeapon) impulseMagnitude *= _handWeapon.Settings.stubbingModifier;
        
        if (impulseMagnitude < 0.1f) return;

        LiveInfo myLive = ControlLive;
        
        if (myLive && other.rigidbody
                   && other.rigidbody.TryGetComponent(out BodyPart bodyPart) && bodyPart.CompareLive(myLive))
            return;
        

        ContactPoint contact = other.GetContact(0);
        
        Vector3 contactNormal = contact.normal;
        Vector3 contactPoint = contact.point + contactNormal * 0.001f;

#if UNITY_EDITOR
        
        Debug.DrawRay(contactPoint, Vector3.right * 0.1f, Color.red, 3f);
        Debug.DrawRay(contactPoint, Vector3.left * 0.1f, Color.red, 3f);
        Debug.DrawRay(contactPoint, Vector3.up * 0.1f, Color.red, 3f);
        Debug.DrawRay(contactPoint, Vector3.down * 0.1f, Color.red, 3f);
        Debug.DrawRay(contactPoint, Vector3.forward * 0.1f, Color.red, 3f);
        Debug.DrawRay(contactPoint, Vector3.back * 0.1f, Color.red, 3f);
        
#endif

        if (other.gameObject.TryGetComponent(out IDamageable damageable)) {
            if (!CanAttack || _intersectionsCount <= 0) return;

            if (damageable is BodyPart { FullyClosed: false })
                PlayClip(stabClips);
            
            if (_handWeapon)
                _handWeapon.ApplyDamage(damageable, contactPoint, contactNormal, impulse, impulseMagnitude);
            else if (_npcWeapon)
                _npcWeapon.ApplyDamage(damageable, contactPoint, contactNormal, impulse, impulseMagnitude);
            
            _intersectionsCount--;
        } else {
            if (other.rigidbody && (other.rigidbody.GetComponent<CollideEvent>()
                || other.rigidbody.GetComponent<GrabbableObject>())) {
                // BLOCKING
                
                _intersectionsCount--;
            }
            
            PoolInfo poolInfo = new PoolInfo {
                Obj = other.gameObject,
                Parent = other.transform.parent,
                Pos = contactPoint,
                Rot = Quaternion.LookRotation(contactNormal, Vector3.up)
            };
            
            PlayClip(hitClips);
            DamageHandler.SpawnImpact(impulseMagnitude, poolInfo);
        }
    }

    private void PlayClip(AudioClip[] clips) {
        effectsSource.pitch = Random.Range(0.9f, 1.1f);
        effectsSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}