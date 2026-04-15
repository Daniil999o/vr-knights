using UnityEngine;
using Player_System.Extensions;

namespace Player_System.Movement.FootstepsSystem
{
    public class FoleyObject : MonoBehaviour
    {
        [SerializeField] internal FoleyData foleyData;

        private void OnTriggerEnter(Collider other) {
            if (other.TryGetComponentInParent(out Footsteps footsteps))
                footsteps.PlayFoley(foleyData, other.ClosestPoint(transform.position));
        }
    }
}