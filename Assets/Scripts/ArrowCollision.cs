using UnityEngine;

public class ArrowCollision : MonoBehaviour
{
    private UpdateScore updateScore;

    private void Start()
    {
        updateScore = GameObject.FindGameObjectWithTag("UIText").GetComponent<UpdateScore>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (Equals(collision.gameObject.layer, LayerMask.NameToLayer("Player")))
        {
            Destroy(gameObject);
            return;
        }

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        if (Equals(collision.gameObject.layer, LayerMask.NameToLayer("Target")))
        {
            updateScore.Increment();
        }

        Destroy(gameObject, 60f);
    }
}
