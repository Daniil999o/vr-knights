using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform target;
    public Transform mapTransform;
    
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;
    public void Map() {
        mapTransform.position = target.TransformPoint(trackingPositionOffset);
        mapTransform.rotation = target.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class IKTargetFollowVRRig : MonoBehaviour
{
    public VRMap[] mapObjects;
    
    // Update is called once per frame
    void LateUpdate()
    {
        for (int i = 0; i < mapObjects.Length; i++)
            mapObjects[i].Map();
    }
}
