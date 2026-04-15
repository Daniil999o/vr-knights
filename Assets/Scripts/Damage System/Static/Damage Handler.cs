using UnityEngine;

public static class DamageHandler
{
    private static HitStrength GetHitStrength(float damage) =>
        damage switch { > 70 => HitStrength.High, > 20 => HitStrength.Medium, var _ => HitStrength.Low };
    
    public static void SpawnImpact(float damage, PoolInfo poolInfo) =>
        EffectsPooler.Instance.SpawnImpact(GetHitStrength(damage), poolInfo);
    
    public static void ApplyDamage(DamageInfo damageInfo) {
        damageInfo.Damageable.ApplyDamage(damageInfo);

        GameObject obj = damageInfo.Damageable is MonoBehaviour mb ? mb.gameObject : null;

        PoolInfo poolInfo = new PoolInfo {
            Obj = obj,
            Pos = damageInfo.Point,
            Rot = Quaternion.LookRotation(damageInfo.Normal, Vector3.up),
            Parent = obj ? obj.transform : null
        };

        HitStrength strength = GetHitStrength(damageInfo.DamageAmount);

        if (damageInfo.Damageable is ArmorPart or BodyPart { FullyClosed: true }) {
            if (damageInfo.Damageable is BodyPart { FullyClosed: true })
                poolInfo.ForcedSurfaceType = SurfaceType.Metal;
            
            EffectsPooler.Instance.SpawnImpact(GetHitStrength(damageInfo.DamageAmount), poolInfo);
        } else {
            EffectsPooler.Instance.SpawnBlood(damageInfo.HitType, strength, poolInfo);
        }

#if UNITY_EDITOR

        Debug.Log($"Damage (Type={damageInfo.HitType}, Strength={strength}) {damageInfo.ToString()}");

#endif
    }
}

public struct DamageInfo
{
    public HitType HitType;
    
    public GameObject From;
    public IDamageable Damageable;

    public Vector3 Point;
    public Vector3 Normal;
    public Vector3 Direction;
    
    public float DamageAmount;
    public float ForceAmount;

    public float ArmorPenetration;

    public override string ToString() => $"(DMG={DamageAmount}, FRC={ForceAmount}) FROM={From}, TO={Damageable}";
}