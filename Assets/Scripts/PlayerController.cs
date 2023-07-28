using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerWeaponControls))]
public class PlayerController : MonoBehaviour
{
    private enum Controls
    {
        Forwards = 0,
        Backwards = 1,
        Left = 2,
        Right = 3,
        Jump = 4,
        Sprint = 5,
        Aim = 6,
        Shoot = 7,
    }

    [Header("Movement settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float sprintSpeed = 7f;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseXSensitivity = 5f;
    [SerializeField] private float mouseYSensitivity = 5f;
    [SerializeField] private bool invertAxis = false;

    [Header("Controls")]
    [SerializeField] private KeyCode sprint = KeyCode.LeftShift;
    [SerializeField] private KeyCode aim = KeyCode.C;

    private PlayerMotor motor;
    private PlayerWeaponControls weaponControls;
    private PlayerInputManager inputManager;

    private void OnValidate()
    {
        Debug.AssertFormat(
            walkSpeed < sprintSpeed,
            "Sprint Speed should be greater than or equal to Walk Speed."
            + " Found: Walk Speed = {0} | Sprint Speed = {1}",
            walkSpeed, sprintSpeed
        );

        Debug.AssertFormat(
            sprint != aim,
            "Sprint and Aim cannot be set to the same key"
        );
    }

    private void Awake()
    {
        IPlayerInput[] inputs = new IPlayerInput[] {
            new PlayerKeyInput(sprint),
            new PlayerKeyInput(aim),
        };
        Dictionary<string, int> indexMap = new() {
            {"sprint", 0},
            {"aim", 1},
        };
        inputManager = new PlayerInputManager(playerInputs: inputs, inputIndexMap: indexMap);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        motor = GetComponent<PlayerMotor>();
        weaponControls = GetComponent<PlayerWeaponControls>();
    }

    private void Update()
    {
        inputManager.Update();
        // Move Player
        float xMove = inputManager.Horizontal;
        float zMove = inputManager.Vertical;

        Vector3 velocity = transform.right * xMove + transform.forward * zMove;

        bool sprint = inputManager["sprint"].Pressed;
        velocity = velocity.normalized * (sprint ? sprintSpeed : walkSpeed);

        motor.SetPlayerVelocity(velocity);

        // Rotate Player
        float yRot = inputManager.MouseX;
        Vector3 rotation = new Vector3(0f, yRot, 0f) * mouseXSensitivity;
        motor.SetPlayerRotation(rotation);

        // Rotate Camera
        float xRot = inputManager.MouseY;
        rotation = new Vector3(xRot, 0f, 0f) * mouseYSensitivity;
        motor.SetCameraRotation(invertAxis ? -rotation : rotation);

        // Add Jump Force
        bool jump = inputManager.Jump;
        motor.setJump(jump ? jumpForce : 0f);

        // Aim Weapon
        bool aim = inputManager["aim"].Pressed;
        weaponControls.SetAim(aim);
    }
}
