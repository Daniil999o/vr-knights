using UnityEngine;

public class CameraOffsetPosition : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    private void LateUpdate() {
        transform.position = target.position + target.forward * offset.z + target.up * offset.y + target.right * offset.x;
    }
}