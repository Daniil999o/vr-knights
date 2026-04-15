using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "EffectConfig", menuName = "Pooling/Effect Config")]
public class EffectConfig : ScriptableObject
{
    [Header("Blood Effects")]
    public BloodEffect slashEffects;
    public BloodEffect arrowEffects;

    [Space]

    [Header("Surfaces")]
    [SerializeField] private SerializedDictionary<SurfaceType, LayerMask> surfacesByType;

    [Header("Impact Effects by Surface")]
    public GameObject impactDirtPrefab;
    public GameObject impactStonePrefab;
    public GameObject impactConcretePrefab;
    public GameObject impactMetalPrefab;
    public GameObject impactWoodPrefab;
    
    [Space]

    [Header("Pool Settings")]
    public int initialPoolSize = 5;
    public int maxPoolSize = 20;
    public float autoDespawnTime = 30f;

    public SurfaceType GetSurfaceType(GameObject obj) =>
        surfacesByType.FirstOrDefault(kvp => (kvp.Value & (1 << obj.layer)) != 0).Key;
}

[System.Serializable]
public struct BloodEffect
{
    public GameObject low;
    public GameObject medium;
    public GameObject high;
}