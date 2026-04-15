using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    private class InventoryPlace
    {
        public InventoryPlaceType placeType;
        public Transform holder;

        [Space]

        public GrabbableObject holdenItem;
    }

    [SerializeField] private LiveInfo liveInfo;
    
    [Space]
    
    [SerializeField] private InventoryPlace[] inventoryPlaces;
    
    [Space]
    
    [SerializeField] private Transform dropPoint;

    [Space]

    [SerializeField] private DesktopInventoryHandler desktopInventoryHandler;
    
    public IInventoryHandler InventoryHandler => desktopInventoryHandler;

    private InventoryPlace GetFirstPlaceByType(InventoryPlaceType placeType) {
        InventoryPlace firstFree = inventoryPlaces.FirstOrDefault(x => x.placeType == placeType && !x.holdenItem);
        
        return firstFree ?? inventoryPlaces.FirstOrDefault(x => x.placeType == placeType);
    }

    private InventoryPlace GetGrabbablePlace(GrabbableObject grabbable) =>
        inventoryPlaces.FirstOrDefault(x => x.holdenItem == grabbable);
    
    public void PickupGrabbable(GrabbableObject grabbable) {
        InventoryPlace place = GetFirstPlaceByType(grabbable.Settings.placeTypeInInventory);

        if (place == null) return;
        
        if (place.holdenItem) DropGrabbable(grabbable);

        place.holdenItem = grabbable;

        grabbable.SetControlLive(liveInfo);
        
        SelectWeapon(place);
    }

    private void DropGrabbable(GrabbableObject grabbable) {
        InventoryPlace objPlace = GetGrabbablePlace(grabbable);

        if (objPlace == null) return;
        
        grabbable.SetControlLive(null);
        
        grabbable.DropAtPoint(dropPoint);
        objPlace.holdenItem = null;
    }

    private void SelectWeapon(InventoryPlace place) {
        if (!inventoryPlaces.Contains(place)) return;
        
        SelectWeapon(inventoryPlaces.ToList().IndexOf(place));
    }
    
    private void SelectWeapon(int index) {
        InventoryPlace myPlace = inventoryPlaces[index];
        
        if (myPlace == null) return;
        
        HideAllWeapons(myPlace.holdenItem.Settings.armedHand);
        
        myPlace.holdenItem.transform.SetParent(null);

        myPlace.holdenItem.transform.position = dropPoint.position;
        myPlace.holdenItem.transform.rotation = dropPoint.rotation;
        
        myPlace.holdenItem.Freeze = false;

        myPlace.holdenItem.SetControlLive(liveInfo);
        
        InventoryHandler.SetGrabbableInHand(myPlace.holdenItem);
    }

    private void HideAllWeapons(ArmedHand armed) {
        foreach (var x in inventoryPlaces) {
            if (!x.holdenItem) continue;

            ArmedHand currentArmed = x.holdenItem.Settings.armedHand;
            
            if (currentArmed != ArmedHand.Both && armed != ArmedHand.Both
                && currentArmed != armed) continue;

            x.holdenItem.Freeze = true;
            x.holdenItem.transform.SetParent(x.holder);
            x.holdenItem.transform.position = Vector3.zero;
            x.holdenItem.transform.rotation = Quaternion.identity;
        }
    }
}