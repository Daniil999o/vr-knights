using UnityEngine;

public class MeleeCollideEvent : MonoBehaviour
{
    private NPC_MeleeWeapon _weapon;

    private void Awake() {
        _weapon = GetComponentInParent<NPC_MeleeWeapon>();
    }

    private void OnTriggerEnter(Collider other) {
        
    }
}