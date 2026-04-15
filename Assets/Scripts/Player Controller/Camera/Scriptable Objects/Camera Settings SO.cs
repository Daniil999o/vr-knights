using UnityEngine;

[CreateAssetMenu(fileName = "Camera Settings", menuName = "Player Controller/Camera Settings")]
public class CameraSettingsSO : ScriptableObject
{
    public float horizontalSensitivity = 2f;
    public float verticalSensitivity = 2f;
    public float maxVerticalAngle = 80f;
    
    [Space]
    
    public bool invertHorizontal = false;
    public bool invertVertical = false;
    public bool followHeadRotation = true;
}