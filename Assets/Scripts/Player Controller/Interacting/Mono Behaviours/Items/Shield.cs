using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private ShieldProfileSO shieldProfile;

    public ShieldProfileSO Settings => shieldProfile;

    [Space]
    
    [SerializeField] private GameObject blockCollider;

    public bool Blocking {
        get => blockCollider.activeSelf;
        set {
            if (Blocking == value) return;
            
            blockCollider.SetActive(value);
        }
    }
    
    private void Start() {
        Blocking = false;
    }
}