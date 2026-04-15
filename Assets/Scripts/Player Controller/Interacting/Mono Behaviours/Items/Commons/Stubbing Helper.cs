using UnityEngine;

public class StubbingHelper : MonoBehaviour
{
    [SerializeField] private CollideEvent collideEvent;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<IDamageable>() as MonoBehaviour)
            collideEvent.Stubbing = true;
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.GetComponent<IDamageable>() as MonoBehaviour)
            collideEvent.Stubbing = false;
    }

    private void OnDisable() {
        collideEvent.Stubbing = false;
    }
}