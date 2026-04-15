using UnityEngine;

public class ArmorPart : MonoBehaviour, IDamageable
{
    [SerializeField][Range(0f, 1f)] private float damageModifier = 1f;
    [SerializeField] private bool isFullyClosed;
    [SerializeField] private BodyPart bodyPart;

    private void Start() {
        bodyPart.FullyClosed = isFullyClosed;
        bodyPart.ArmorPart = this;
    }
    
    public void ApplyDamage(DamageInfo damageInfo) {
        float effectiveModifier = Mathf.Lerp(damageModifier, 1f, damageInfo.ArmorPenetration);
        
        damageInfo.DamageAmount *= effectiveModifier;
        bodyPart.ApplyDamageArmored(damageInfo);
    }
}