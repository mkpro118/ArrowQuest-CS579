using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    [Header("Arrow Prefab")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform currentArrow;
    [SerializeField] private Transform arrowExitPoint;
    [SerializeField] private Transform bowStringEnd;


    private Vector3 arrowOriginalPosition;
    private Vector3 bowStringEndOriginalPosition;
    private const float targetZOfPulledArrow = -0.1f;
    private const float pullEpsilon = 0.1f;

    private readonly float arrowMass = 0.15f;
    private float arrowPullRange;

    private Rigidbody arrowRB;

    private Renderer[] arrowRenderers;

    private void Start()
    {
        arrowOriginalPosition = currentArrow.localPosition;
        bowStringEndOriginalPosition = bowStringEnd.localPosition;
        arrowRenderers = currentArrow.GetComponentsInChildren<Renderer>();
        arrowPullRange = Mathf.Abs(arrowOriginalPosition.z - targetZOfPulledArrow);
    }

    private bool DonePulling() => Mathf.Abs(currentArrow.localPosition.z - targetZOfPulledArrow) <= pullEpsilon;

    public float PullArrow(float rate = 1.2f)
    {
        if (DonePulling()) return 1f;
        currentArrow.transform.localPosition = new Vector3(
            currentArrow.transform.localPosition.x,
            currentArrow.transform.localPosition.y,
            Mathf.Lerp(
                currentArrow.transform.localPosition.z,
                targetZOfPulledArrow,
                Time.deltaTime * rate
            )
        );
        bowStringEnd.transform.localPosition = new Vector3(
            bowStringEnd.transform.localPosition.x,
            Mathf.Lerp(
                bowStringEnd.transform.localPosition.y,
                targetZOfPulledArrow,
                Time.deltaTime * rate
            ),
            bowStringEnd.transform.localPosition.z
        );
        float pullDistance = Mathf.Abs(targetZOfPulledArrow - currentArrow.transform.localPosition.z);
        return 1 - (pullDistance / arrowPullRange);
    }

    public void ReleaseArrow(Vector3 targetPoint, float releaseForce)
    {
        currentArrow.transform.localPosition = new Vector3(
            currentArrow.transform.localPosition.x,
            currentArrow.transform.localPosition.y,
            3f
        );

        Vector3 direction = targetPoint - currentArrow.transform.position;
        direction = direction.normalized;

        GameObject arrow = Instantiate(
            arrowPrefab,
            arrowExitPoint.transform.position,
            currentArrow.transform.rotation
        );

        foreach( Renderer renderer in arrowRenderers)
        {
            renderer.enabled = false;
        }

        arrow.name = "Arrow";
        arrowRB = arrow.AddComponent<Rigidbody>();
        arrowRB.constraints = RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationY;
        arrowRB.velocity = direction * GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().velocity.magnitude;
        
        DisableCollision();

        arrowRB.mass = arrowMass;
        arrowRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
        arrowRB.interpolation = RigidbodyInterpolation.Interpolate;
        arrowRB.useGravity = true;
        arrowRB.excludeLayers = LayerMask.GetMask("Player");

        arrowRB.AddForce(direction * releaseForce, ForceMode.Impulse);

        bowStringEnd.transform.localPosition = bowStringEndOriginalPosition;

        Invoke(nameof(EnableCollision), 0.05f);
        Invoke(nameof(EnableArrow), 0.5f);
    }

    private void EnableArrow()
    {
        foreach (Renderer renderer in arrowRenderers)
        {
            renderer.enabled = true;
        }
    }


    private void DisableCollision()
    {
        arrowRB.detectCollisions = false;
    }

    private void EnableCollision()
    {
        arrowRB.detectCollisions = true;
    }

    public void UpdateCurrentArrowPosition() => arrowOriginalPosition = currentArrow.localPosition;

    public bool ResetArrowPosition(float rate = 5) {
        currentArrow.transform.localPosition = new Vector3(
            currentArrow.transform.localPosition.x,
            currentArrow.transform.localPosition.y,
            Mathf.Lerp(
                currentArrow.transform.localPosition.z,
                arrowOriginalPosition.z,
                Time.deltaTime * rate
            )
        );

        return Mathf.Abs(currentArrow.localPosition.z - arrowOriginalPosition.z) <= pullEpsilon;
    }
}
