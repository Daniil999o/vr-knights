using UnityEngine;

[CreateAssetMenu(fileName = "Shield Profile", menuName = "Player Controller/Weapons/Shield Profile")]
public class ShieldProfileSO : ScriptableObject
{
    public float shieldUpSpeed = 6f;
    public float shieldIdleSpeed = 3f;

    [Space]

    public PoseDef shieldIdle;
    public PoseDef shieldUp;
    
    [Space]
    
    public PoseDef tempPose;
}