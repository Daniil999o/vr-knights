using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
public class DesktopInventoryHandler : MonoBehaviour, IInventoryHandler
{
    [SerializeField] private DesktopWeaponController desktopWeaponController;

    private PlayerInventory _playerInventory;

    private void Awake() {
        _playerInventory = GetComponent<PlayerInventory>();
    }
    
    public void GrabItem(PlayerHand hand, IGrabbable grabbable) {
        if (grabbable.IsGrabbed) return;
        
        GrabbableObject grabObject = grabbable as GrabbableObject;
        
        if (grabObject && hand.IsBusy) return;
        
        if (!grabObject) return;
        
        _playerInventory.PickupGrabbable(grabObject);
    }

    public void SetGrabbableInHand(GrabbableObject grabbable) {
        if (grabbable.GetComponent<HandWeapon>())
            desktopWeaponController.SetGrabbable(grabbable);
        else if (grabbable.GetComponent<Shield>())
            desktopWeaponController.SetShield(grabbable);
    }
}