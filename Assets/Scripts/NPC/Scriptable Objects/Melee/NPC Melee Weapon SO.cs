using UnityEngine;

[CreateAssetMenu(fileName = "Melee Weapon", menuName = "NPC/Weapons/Melee")]
public class NPC_MeleeWeaponSO : ScriptableObject
{
    public float damage = 20f;
    public float force = 30f;
    [Range(0f, 1f)] public float armorPenetration = 0f;
}