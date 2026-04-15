using UnityEngine;

public class BodyPart : MonoBehaviour, IDamageable
{
    [SerializeField][Min(0f)] private float damageModifier = 1f;

    public bool IsKinematic {
        get => _rb.isKinematic;
        set {
            _rb.isKinematic = value;
            _rb.collisionDetectionMode = value ? CollisionDetectionMode.Discrete : CollisionDetectionMode.Continuous;
            _coll.enabled = true;
        }
    }
    
    public bool FullyClosed { get; set; }
    public ArmorPart ArmorPart { get; set; }
    
    public Rigidbody Rb => _rb;
    
    private Rigidbody _rb;
    private Collider _coll;

    private ILivable _livable;

    public bool CompareLive(ILivable other) => other == _livable;
    
    private void Awake() {
        _livable = GetComponentInParent<ILivable>();
        
        _rb = GetComponent<Rigidbody>();
        _coll = GetComponent<Collider>();
    }
    
    public void ApplyDamage(DamageInfo damageInfo) {
        if (FullyClosed) {
            ArmorPart.ApplyDamage(damageInfo);
            return;
        }
        
        damageInfo.DamageAmount *= damageModifier;
        _rb.AddForceAtPosition(damageInfo.Direction * damageInfo.ForceAmount, damageInfo.Point, ForceMode.Impulse);

        _livable?.ApplyDamage(damageInfo);
    }
    
    public void ApplyDamageArmored(DamageInfo damageInfo) {
        damageInfo.DamageAmount *= damageModifier;
        _rb.AddForceAtPosition(damageInfo.Direction * damageInfo.ForceAmount, damageInfo.Point, ForceMode.Impulse);

        _livable?.ApplyDamage(damageInfo);
    }
}