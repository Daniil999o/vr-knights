using EasyPoolKit;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class EffectsPooler : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private EffectConfig config;

    [Space]

    [Header("Information")]
    [SerializeField][ReadOnly] private int activeBloodSlashEffects;
    [SerializeField][ReadOnly] private int activeBloodArrowEffects;
    [SerializeField][ReadOnly] private int activeImpactEffects;

    private static EffectsPooler _instance;

    public static EffectsPooler Instance {
        get {
            if (!_instance) _instance = FindFirstObjectByType<EffectsPooler>(FindObjectsInactive.Include);

            return _instance;
        }
    }

    public float AutoDespawnTime => config.autoDespawnTime;

    private void Awake() {
        if (config)
            RegisterPools();
        else
            Debug.LogError("EffectConfig is not assigned!");
    }

    private void RegisterPools() {
        RegisterPrefabWithConfig(config.slashEffects.low, "Blood_Slash_Low");
        RegisterPrefabWithConfig(config.slashEffects.medium, "Blood_Slash_Medium");
        RegisterPrefabWithConfig(config.slashEffects.high, "Blood_Slash_High");

        RegisterPrefabWithConfig(config.arrowEffects.low, "Blood_Arrow_Low");
        RegisterPrefabWithConfig(config.arrowEffects.medium, "Blood_Arrow_Medium");
        RegisterPrefabWithConfig(config.arrowEffects.high, "Blood_Arrow_High");

        RegisterPrefabWithConfig(config.impactDirtPrefab, "Impact_Dirt");
        RegisterPrefabWithConfig(config.impactStonePrefab, "Impact_Stone");
        RegisterPrefabWithConfig(config.impactConcretePrefab, "Impact_Concrete");
        RegisterPrefabWithConfig(config.impactMetalPrefab, "Impact_Metal");
        RegisterPrefabWithConfig(config.impactWoodPrefab, "Impact_Wood");
    }

    private void RegisterPrefabWithConfig(GameObject prefab, string poolId) {
        if (!prefab) {
            Debug.LogWarning($"Prefab for {poolId} is null, skipping registration.");
            return;
        }

        RecyclablePoolConfig poolConfig = new RecyclablePoolConfig {
            ObjectType = RecycleObjectType.RecyclableGameObject,
            ReferenceType = typeof(PoolableEffect),
            PoolId = poolId,
            InitCreateCount = config.initialPoolSize,
            MaxSpawnCount = config.maxPoolSize,
            ReachMaxLimitType = PoolReachMaxLimitType.RecycleOldest,
            DespawnDestroyType = PoolDespawnDestroyType.DestroyToLimit,
            MaxDespawnCount = config.maxPoolSize,
            ClearType = PoolClearType.ClearToLimit,
            AutoClearTime = 0f,
            IfIgnoreTimeScale = false
        };

        RecyclableGOPoolKit.Instance.RegisterPrefab(prefab, poolConfig);
    }

    public void SpawnBlood(HitType type, HitStrength strength, PoolInfo info) {
        GameObject prefab = GetBloodPrefab(type, strength);
        
        if (!prefab) {
            Debug.LogWarning($"No blood prefab assigned for strength: {strength}");
            return;
        }

        PoolableEffect effect = RecyclableGOPoolKit.Instance.SimpleSpawn<PoolableEffect>(prefab);
        
        if (!effect) return;
        
        effect.Initialize(strength, info.Pos, info.Rot, info.Parent);


        switch (type) {
            case HitType.Slash:
                activeBloodSlashEffects++;
                break;
            case HitType.Arrow:
                activeBloodArrowEffects++;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void SpawnImpact(HitStrength strength, PoolInfo info) {
        SurfaceType surface = info.ForcedSurfaceType ?? config.GetSurfaceType(info.Obj);
        
        GameObject prefab = GetImpactPrefab(surface);
        
        if (!prefab) {
            Debug.LogWarning($"No impact prefab assigned for surface: {surface}");
            return;
        }

        PoolableEffect effect = RecyclableGOPoolKit.Instance.SimpleSpawn<PoolableEffect>(prefab);
        
        if (!effect) return;
        
        effect.Initialize(strength, info.Pos, info.Rot, info.Parent);
        
        activeImpactEffects++;
    }

    public void ClearUnusedPools() {
        RecyclableGOPoolKit.Instance.ClearAllUnusedObjects();
    }

    private GameObject GetBloodPrefab(HitType type, HitStrength strength) {
        return (type, strength) switch {
            (HitType.Slash, HitStrength.Low) => config.slashEffects.low,
            (HitType.Slash, HitStrength.Medium) => config.slashEffects.medium,
            (HitType.Slash, HitStrength.High) => config.slashEffects.high,
            
            (HitType.Arrow, HitStrength.Low) => config.arrowEffects.low,
            (HitType.Arrow, HitStrength.Medium) => config.arrowEffects.medium,
            (HitType.Arrow, HitStrength.High) => config.arrowEffects.high,
            
            var _ => config.slashEffects.medium
        };
    }

    private GameObject GetImpactPrefab(SurfaceType surface) {
        return surface switch {
            SurfaceType.Dirt => config.impactDirtPrefab,
            SurfaceType.Stone => config.impactStonePrefab,
            SurfaceType.Metal => config.impactMetalPrefab,
            SurfaceType.Concrete => config.impactConcretePrefab,
            SurfaceType.Wood => config.impactWoodPrefab,
            var _ => config.impactStonePrefab
        };
    }
}

public class PoolInfo
{
    public GameObject Obj;

    public SurfaceType? ForcedSurfaceType = null;
    
    public Vector3 Pos;
    public Quaternion Rot;
    public Transform Parent;
}