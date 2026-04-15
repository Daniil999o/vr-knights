using UnityEngine;

public class HandWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] private HandWeaponProfileSO weaponProfile;

    public HandWeaponProfileSO Settings => weaponProfile;

    [Space]
    
    [SerializeField] private Collider coll;
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

    public bool CanAttack {
        set => collideEvent.CanAttack = value;
    }
    
    public bool EnabledCollision {
        set {
            CanAttack = false;
            coll.enabled = value;
        }
    }

    private void Start() {
        Blocking = false;
    }

    public void ApplyDamage(IDamageable damageable, Vector3 contactPoint, Vector3 normal, Vector3 direction, float force) {
        float damageAmount = weaponProfile.defaultDamage * force;
        float forceAmount = weaponProfile.defaultForce * force;
        
        damageAmount = Mathf.Clamp(damageAmount, 0f, weaponProfile.maxDamage);
        forceAmount = Mathf.Clamp(forceAmount, 0f, weaponProfile.maxForce);

        DamageInfo damageInfo = new DamageInfo {
            HitType = HitType.Slash,
            From = gameObject,
            Damageable = damageable,
            Point = contactPoint,
            Normal = normal,
            Direction = direction,
            DamageAmount = damageAmount,
            ForceAmount = forceAmount,
            ArmorPenetration = weaponProfile.armorPenetration
        };
        
        DamageHandler.ApplyDamage(damageInfo);
    }
}