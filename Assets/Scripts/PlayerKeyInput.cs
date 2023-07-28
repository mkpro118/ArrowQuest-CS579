using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerKeyInput: IPlayerInput
{
    private string inputName;
    private KeyCode inputKeyCode;

    private bool down;
    private bool up;
    private bool pressed;

    private bool disabled;

    public string Name { get { return inputName; } }

    public bool Down { get { return down; } }

    public bool Pressed { get { return pressed; } }

    public bool Up { get { return up; } }

    public float Value
    {
        get { 
            throw new InvalidOperationException(
                "Cannot access `Value` property for key inputs."
            );
        }
    }

    public bool IsDisabled() => disabled;

    public PlayerKeyInput(KeyCode inputKeyCode, bool disabled) => Initialize(inputKeyCode, disabled);

    public PlayerKeyInput(KeyCode inputKeyCode) => Initialize(inputKeyCode, false);

    public PlayerKeyInput(string buttonName, bool disabled = false) => Initialize((KeyCode) Enum.Parse(typeof(KeyCode), buttonName), disabled);

    private void Initialize(KeyCode inputKeyCode, bool disabled)
    {
        this.inputKeyCode = inputKeyCode;
        this.disabled = disabled;

        inputName = inputKeyCode.ToString();
    }

    public void Disable() { 
        disabled = true;
        down = pressed = up = false;
    }

    public void Enable() => disabled = false;

    public void Update()
    {
        if (disabled) return;

        GetInput();
    }

    private void GetInput()
    {
        down = Input.GetKeyDown(inputKeyCode);
        pressed = Input.GetKey(inputKeyCode);
        up = Input.GetKeyUp(inputKeyCode);
    }
}