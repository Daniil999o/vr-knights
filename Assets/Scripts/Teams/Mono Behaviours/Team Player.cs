using UnityEngine;

public class TeamPlayer : MonoBehaviour
{
    [SerializeField] private TeamDataSO myTeam;
    
    [field: Space]
    
    [field: SerializeField] public bool IsDead { get; set; }
    
    public TeamDataSO MyTeam => myTeam;

    [Space]
    
    [SerializeField] private UniformType uniformType = UniformType.Armor;
    [SerializeField] private YG_IndexedMaterial[] uniformMaterials;
    [SerializeField] private YG_IndexedMaterial[] uniformLowMaterials;
        
    
    public void SetTeam(TeamDataSO team) {
        myTeam = team;
        UpdateSkin();
    }

    private void OnEnable() {
        TeamPlayHandler.RegisterPlayer(this);
    }

    private void OnDisable() {
        TeamPlayHandler.UnregisterPlayer(this);
    }

    private void Start() {
        UpdateSkin();
    }

    private void UpdateSkin() {
        if (uniformType is UniformType.Armor or UniformType.Both) {
            foreach (var x in uniformMaterials) x.StaticMaterial = myTeam.uniformMaterial;
        }

        if (uniformType is UniformType.Low or UniformType.Both) {
            foreach (var x in uniformLowMaterials) x.StaticMaterial = myTeam.uniformLowMaterial;
        }
    }
}