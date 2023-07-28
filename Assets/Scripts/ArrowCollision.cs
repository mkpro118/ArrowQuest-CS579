using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (Equals(collision.gameObject.layer, LayerMask.NameToLayer("Player"))) return;

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        Destroy(gameObject, 60f);
    }
}
