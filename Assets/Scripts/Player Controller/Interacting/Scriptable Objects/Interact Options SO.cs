using UnityEngine;

[CreateAssetMenu(fileName = "Interact Options", menuName = "Player Controller/Interact Options")]
public class InteractOptionsSO : ScriptableObject
{
    public LayerMask interactLayerMask;
    public float interactRadius = 3f;
}