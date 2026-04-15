public interface IInventoryHandler
{
    public void GrabItem(PlayerHand hand, IGrabbable grabbable);

    public void SetGrabbableInHand(GrabbableObject grabbable);
}