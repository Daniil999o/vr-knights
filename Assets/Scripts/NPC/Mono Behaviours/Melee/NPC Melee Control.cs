using UnityEngine;

public class NPC_MeleeControl : MonoBehaviour
{
    [SerializeField] private NPC_AI controller;
    
    [Space]
    
    [SerializeField] private Joint myJoint;
    
    [Space]
    
    [SerializeField] private NPC_MeleeWeapon myWeapon;

    public bool Blocking {
        set {
            if (!myWeapon) return;

            myWeapon.Blocking = value;
        }
    }

    private void Start() {
        myWeapon.ControlLive = controller.GetComponent<LiveInfo>();
    }
    
    public void StartAttack() {
        if (!myWeapon) return;

        myWeapon.CanAttack = true;
    }
    
    public void CancelAttack() {
        if (!myWeapon) return;

        myWeapon.CanAttack = false;
    }

    private void OnDisable() {
        if (!myWeapon) return;

        myJoint.connectedBody = null;
        
        myWeapon.Unpin();
        
        CancelAttack();
        
        Destroy(myWeapon.gameObject, 30f);
    }

    public void ExitAttack() {
        if (!controller) return;
        
        controller.CancelAttack();
    }
}