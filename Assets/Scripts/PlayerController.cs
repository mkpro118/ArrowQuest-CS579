using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float mouseXSensitivity = 5f;
    [SerializeField] private float mouseYSensitivity = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Controls")]
    [Tooltip("Defaults to Right Mouse Button")]
    [SerializeField] private KeyCode aimButton = KeyCode.Mouse1;

    private PlayerMotor motor;

    private float xMove;
    private float zMove;

    private float xRot;
    private float yRot;

    private bool aim;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        motor = GetComponent<PlayerMotor>();
    }

    private void Update()
    {
        GetInputs();

    }

    private void GetInputs()
    {
        xMove = Input.GetAxisRaw("Horizontal");
        zMove = Input.GetAxisRaw("Vertical");

        xRot = Input.GetAxisRaw("Mouse Y");
        yRot = Input.GetAxisRaw("Mouse X");

        aim = Input.GetButton();
    }
}
