using UnityEngine;

[CreateAssetMenu(fileName = "Grabbable Object", menuName = "Player Controller/Items/Grabbable Object")]
public class GrabbableObjectSO : ScriptableObject
{
    public float weight = 1f;

    [Space]

    public InHandPoint handPoint;

    [Space]

    public InventoryPlaceType placeTypeInInventory;
    public ArmedHand armedHand = ArmedHand.Both;
}

public enum ArmedHand
{
    Right,
    Left,
    Both
}

[System.Serializable]
public struct InHandPoint
{
    public Vector3 Pos;
    public Vector3 Rot;
}