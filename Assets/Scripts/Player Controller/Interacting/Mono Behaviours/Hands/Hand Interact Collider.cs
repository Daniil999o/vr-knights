using UnityEngine;

public class HandInteractCollider : MonoBehaviour
{
    [SerializeField] private PlayerHand myHand;
    [SerializeField] private PlayerInventory playerInventory;
    
    private void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out IGrabbable grabbable)) return;
        
        playerInventory.InventoryHandler.GrabItem(myHand, grabbable);
    }
}
