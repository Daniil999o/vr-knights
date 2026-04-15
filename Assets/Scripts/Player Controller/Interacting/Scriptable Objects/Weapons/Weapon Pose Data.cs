using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Poses", menuName = "Player Controller/Weapons/Weapon Poses")]
public class WeaponPoseData : ScriptableObject
{
    [Header("Temp")]
    public PoseDef tempPose;
    
    [Space]
    
    [Header("Idle")]
    public PoseDef idlePose;

    [Space]
    
    [Header("Windups")]
    public PoseDef windupRight;
    public PoseDef windupLeft;
    public PoseDef windupTop;
    
    [Space]

    [Header("Strikes")]
    public PoseDef strikeRight;
    public PoseDef strikeLeft;
    public PoseDef strikeTop;
    
    [Space]

    [Header("Thrust")]
    public PoseDef thrustWindup;
    public PoseDef thrustStrike;

    [Space]

    [Header("Block")]
    public PoseDef blockPose;
}

[System.Serializable]
public class PoseDef
{
    public Vector3 localPosition;
    public Vector3 localRotation;
    
    public RuntimePose ToRuntime() => new RuntimePose(localPosition, Quaternion.Euler(localRotation));
}

public struct RuntimePose
{
    public Vector3 pos;
    public Quaternion rot;

    public RuntimePose(Vector3 pos, Quaternion rot)
    {
        this.pos = pos;
        this.rot = rot;
    }

    public static RuntimePose Lerp(RuntimePose a, RuntimePose b, float t)
    {
        Vector3 eulerA = a.rot.eulerAngles;
        Vector3 eulerB = b.rot.eulerAngles;

        Vector3 eulerLerped = new Vector3(
            Mathf.LerpAngle(eulerA.x, eulerB.x, t),
            Mathf.LerpAngle(eulerA.y, eulerB.y, t),
            Mathf.LerpAngle(eulerA.z, eulerB.z, t)
        );

        
        return new RuntimePose(
            Vector3.Lerp(a.pos, b.pos, t),
            Quaternion.Euler(eulerLerped)
            // Quaternion.Lerp(a.rot, b.rot, t * 5f)
        );
    }
    
    public static bool Approximately(RuntimePose a, RuntimePose b, 
        float positionTolerance = 0.1f, 
        float rotationTolerance = 1f)
    {
        bool positionClose = (a.pos - b.pos).sqrMagnitude <= positionTolerance * positionTolerance;
        bool rotationClose = Quaternion.Angle(a.rot, b.rot) <= rotationTolerance;
    
        return positionClose && rotationClose;
    }
    
    public static bool Approximately(Vector3 pos1, Quaternion rot1, Vector3 pos2, Quaternion rot2, 
        float positionTolerance = 0.1f, 
        float rotationTolerance = 1f)
    {
        bool positionClose = (pos1 - pos2).sqrMagnitude <= positionTolerance * positionTolerance;
        bool rotationClose = Quaternion.Angle(rot1, rot2) <= rotationTolerance;
    
        return positionClose && rotationClose;
    }
}