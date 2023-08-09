using UnityEngine;

public class PlayerWeaponControls : MonoBehaviour
{
    [Header("Weapon")]
    [Tooltip("Weapon Container. This will be centered when aiming")]
    [SerializeField] private Transform weapon;
    
    [Header("Camera")]
    [Tooltip("Camera where the weapon will be centered. This should be the main fps camera")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float normalFieldOfView = 60f;
    [SerializeField] private float zoomedFieldOfView = 30f;
    [SerializeField] private float zoomRate = 5;

    [Header("Arrow Script")]
    [Tooltip("Control the behavior of the arrow")]
    [SerializeField] private ArrowBehavior arrowBehavior;
    [SerializeField] private float arrowReleaseForce = 5f;
    [SerializeField] private float arrowRange = 20f;

    private bool isAiming = false;
    private bool wasAiming = false;
    float pullPercent = 0;
    private static readonly Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);

    private Vector3 weaponOriginalPosition;

    private const float zoomEpsilon = 0.1f;

    private bool isReloading = false;

    public void SetAim(bool aim) => isAiming = aim;

    private bool DoneZooming() => Mathf.Abs(playerCamera.fieldOfView - zoomedFieldOfView) <= zoomEpsilon;
    
    private bool DoneUnzooming() => Mathf.Abs(playerCamera.fieldOfView - normalFieldOfView) <= zoomEpsilon;

    private void Start()
    {
        weaponOriginalPosition = weapon.localPosition;
        arrowBehavior.UpdateCurrentArrowPosition();
    }

    private void Update()
    {
        if (isAiming)
        {
            if (!ZoomOnAim()) return;

            pullPercent = arrowBehavior.PullArrow();
            wasAiming = true;
        }
        else
        {
            if (wasAiming)
            {
                Ray ray = playerCamera.ViewportPointToRay(screenCenter);

                Vector3 targetPoint;

                // Check if RayCast hits something
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    targetPoint = hit.point;    
                }
                else
                {
                    targetPoint = ray.GetPoint(arrowRange);
                }
                arrowBehavior.ReleaseArrow(targetPoint, arrowReleaseForce * pullPercent);
                isReloading = true;
                wasAiming = false;
            }
            else if (isReloading)
            {
                if(arrowBehavior.ResetArrowPosition())
                {
                    isReloading = false;
                }
            }
            ResetZoom();
        }
    }

    private bool ZoomOnAim()
    {
        if (DoneZooming()) return true;
        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            zoomedFieldOfView,
            Time.deltaTime * zoomRate
        );

        weapon.transform.localPosition = new Vector3(
            Mathf.Lerp(
                weapon.transform.localPosition.x,
                playerCamera.transform.localPosition.x,
                Time.deltaTime * zoomRate
            ),
            weapon.transform.localPosition.y,
            weapon.transform.localPosition.z
        );
        return false;
    }

    private void ResetZoom()
    {
        if (DoneUnzooming()) return;

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            normalFieldOfView,
            Time.deltaTime * zoomRate
        );

        weapon.transform.localPosition = new Vector3(
            Mathf.Lerp(
                weapon.transform.localPosition.x,
                weaponOriginalPosition.x,
                Time.deltaTime * zoomRate
            ),
            weaponOriginalPosition.y,
            weaponOriginalPosition.z
        );
    }
}
