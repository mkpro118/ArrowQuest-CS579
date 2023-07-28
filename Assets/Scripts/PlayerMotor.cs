using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [Header("First Person Camera")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform groundCheck;

    private Vector3 playerVelocity = Vector3.zero;
    private Vector3 playerRotation = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;

    private float playerJump = 0f;

    private Rigidbody rb;
    private LayerMask environmentLayerMask;

    private bool isGrounded = false;
    private const float groundCheckEpsilon = 0.1f;

    private float minVerticalAngle = -45;
    private float maxVerticalAngle = 45;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        environmentLayerMask = LayerMask.GetMask("Environment");

        if (environmentLayerMask < 0)
        {
            Debug.LogWarning("NO ENVIRONMENT FOUND");
        }
    }

    private void Update()
    {
        isGrounded = IsGrounded();
        RotatePlayer();
        RotateCamera();
    }

    public void SetPlayerVelocity(Vector3 velocity)
    {
        if (!isGrounded) return;
        playerVelocity = velocity;
    }

    public void SetPlayerRotation(Vector3 rotation)
    {
        playerRotation = rotation;
    }

    public void SetCameraRotation(Vector3 rotation)
    {
        cameraRotation = rotation;
    }

    public void setJump(float jump)
    {
        if (Mathf.Approximately(playerJump, 0f))
        {
            playerJump = jump;
        }
    }


    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (playerVelocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + playerVelocity * Time.fixedDeltaTime);
        }

        if (isGrounded)
        {
            rb.AddForce(Vector3.up * playerJump, ForceMode.Impulse);
            playerJump = 0f;
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(
            groundCheck.position,
            groundCheckEpsilon,
            environmentLayerMask
        );
    }

    private void RotatePlayer()
    {
        if (playerRotation != Vector3.zero)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(playerRotation));
        }
    }

    private void RotateCamera()
    {
        if (playerCamera == null) return;
        if (cameraRotation != Vector3.zero)
        {
            //playerCamera.transform.Rotate(cameraRotation);

            playerCamera.transform.Rotate(cameraRotation);

            // Calculate the new vertical angle after applying the rotation
            float newVerticalAngle = playerCamera.transform.localEulerAngles.x;

            // Clamp the new vertical angle to the desired range
            newVerticalAngle = ClampAngle(newVerticalAngle, minVerticalAngle, maxVerticalAngle);

            // Apply the clamped vertical angle to the camera's localEulerAngles.x
            playerCamera.transform.localEulerAngles = new Vector3(newVerticalAngle, 0f, 0f);
        }
    }

    private float ClampAngle(float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle + 180f, 360f) - 180f;
        return Mathf.Clamp(angle, min, max);
    }
}
