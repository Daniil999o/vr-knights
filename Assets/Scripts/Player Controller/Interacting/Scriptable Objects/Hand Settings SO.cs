using UnityEngine;

[CreateAssetMenu(fileName = "Hand Settings", menuName = "Player Controller/Hand Settings")]
public class HandSettingsSO : ScriptableObject
{
    public float pinchSpeed = 4f;
    public float grabSpeed = 3f;
}