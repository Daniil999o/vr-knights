using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Profile", menuName = "Player Controller/Weapons/Weapon Profile")]
public class HandWeaponProfileSO : ScriptableObject
{
    [Header("Damages")]
    public float defaultDamage = 30f;
    public float stubbingModifier = 1f;
    public float defaultForce = 10f;
    [Range(0f, 1f)] public float armorPenetration = 0f;

    [Space]

    [Header("Max Damage")]
    public float maxDamage = 50f;
    public float maxForce = 50f;

    [Space]

    public int maxIntersectionsCount = 1;

    [Space]

    public WeaponPoseData poseData;
    public WeaponAttackSettings attackSettings;
}

[System.Serializable]
public class WeaponAttackSettings
{
    [Header("Input Sensitivity")]   
    public float windupVerticalZone = 0.8f;
    public float windupHorizontalZone = 0.6f;
    public float inputSmoothTime = 5f;
    
    [Space]

    [Header("Weapon Speeds")]
    public float windupSpeed = 10f;
    public float strikeSpeed = 30f;
    public float thrustWindupSpeed = 10f;
    public float thrustStrikeSpeed = 25f;
    public float returnToIdleSpeed = 15f;
    public float blockSpeed = 5f;
}