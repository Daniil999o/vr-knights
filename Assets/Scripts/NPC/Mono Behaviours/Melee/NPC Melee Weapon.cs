using UnityEngine;

public class NPC_MeleeWeapon : MonoBehaviour
{
    [SerializeField] private NPC_MeleeWeaponSO data;
    
    [Space]
    
    [SerializeField] private CollideEvent collideEvent;

    [Space]

    [SerializeField] private GameObject blockCollider;

    public bool Blocking {
        get => blockCollider.activeSelf;
        set {
            if (Blocking == value) return;
            
            collideEvent.gameObject.SetActive(!value);
            blockCollider.SetActive(value);
        }
    }
    
    public LiveInfo ControlLive { get; set; }

    private void Start() {
        Blocking = false;
    }

    public void Unpin() {
        if (!TryGetComponent(out Rigidbody rb)) return;

        transform.SetParent(null);
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.None;
    }
    
    public bool CanAttack {
        get => collideEvent.CanAttack;
        set => collideEvent.CanAttack = value;
    }
    
    public void ApplyDamage(IDamageable damageable, Vector3 contactPoint, Vector3 normal, Vector3 direction, float force) {
        DamageInfo damageInfo = new DamageInfo {
            HitType = HitType.Slash,
            From = gameObject,
            Damageable = damageable,
            Point = contactPoint,
            Normal = normal,
            Direction = direction,
            DamageAmount = data.damage * Random.Range(0.8f, 1.2f),
            ForceAmount = data.force * Random.Range(0.8f, 1.2f),
            ArmorPenetration = data.armorPenetration
        };
        
        DamageHandler.ApplyDamage(damageInfo);
    }
}