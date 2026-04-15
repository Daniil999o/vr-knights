using UnityEngine;

[CreateAssetMenu(fileName = "Team Data", menuName = "Teams/Team Data")]
public class TeamDataSO : ScriptableObject
{
    public string name;

    [Space]

    public Material uniformMaterial;
    public Material uniformLowMaterial;
    
    
}

public enum UniformType
{
    Armor,
    Low,
    Both
}