using System;
using UnityEngine;

/// <summary>
/// Represents a player input for a button or axis in the game.
/// </summary>
[System.Serializable]
public class PlayerButtonInput : IPlayerInput
{
    // The name of the input (button or axis).
    private string inputName;

    // Indicates whether the input control was pressed in the current frame.
    private bool down;

    // Indicates whether the input control was released in the current framce.
    private bool up;

    // Indicates whether the input control is being held down.
    private bool pressed;

    // Indicates whether the input is an axis input (true) or a button input (false).
    private bool isAxis;

    // Gets a value indicating whether to use raw axis input values (true) or not (false).
    private bool useRaw;

    // The value of the input (valid only for axis inputs).
    private float value = 0f;

    // Is the input disabled
    private bool disabled = false;


    /// <summary>
    /// Gets the name of the input.
    /// </summary>
    public string Name { get { return inputName; } }

    /// <summary>
    /// Gets a value indicating whether the input control is currently pressed down once.
    /// </summary>
    public bool Down { get { return down; } }

    /// <summary>
    /// Gets a value indicating whether the input control is currently released.
    /// </summary>
    public bool Up { get { return up; } }

    /// <summary>
    /// Gets a value indicating whether the input control is currently being held down.
    /// </summary>
    public bool Pressed { get { return pressed; } }

    /// <summary>
    /// Gets the value of the input (valid only for axis inputs).
    /// </summary>
    public float Value { get {
            if (isAxis) return value;

            throw new InvalidOperationException(
                "Cannot access `Value` property for non-axis input."
            );
        }
    }
    public bool IsAxis { get { return isAxis; } }

    public bool IsDisabled() => disabled;

    /// <summary>
    /// Creates a new instance of the PlayerButtonInput class with the specified input settings.
    /// </summary>
    /// <param name="buttonName">The name of the input (button or axis).</param>
    /// <param name="isAxis">True if the input is an axis, False if it's a button.</param>
    /// <param name="useRaw">True to use raw axis input values, False otherwise.</param>
    /// <param name="disabled">True to initialize the input as disabled, False otherwise.</param>
    public PlayerButtonInput(
            string buttonName,
            bool isAxis = false,
            bool useRaw = false,
            bool disabled = false
        )
    {
        this.inputName = buttonName;
        this.isAxis = isAxis;
        this.useRaw = useRaw;
        this.disabled = disabled;
    }

    /// <summary>
    /// Disables the input, setting all states to false and the axis value to 0.
    /// </summary>
    public void Disable()
    {
        disabled = true;
        down = pressed = up = false;
        value = 0f;
    }

    /// <summary>
    /// Enables the input after being disabled.
    /// </summary>
    public void Enable() => disabled = false;

    /// <summary>
    /// Updates the input state by checking if it is a button or an axis input and updating accordingly.
    /// </summary>
    public void Update()
    {
        if (disabled) return;

        if (isAxis) { GetAxisInput(); }
        else { GetInput(); }
    }

    // Private methods for getting input states based on button or axis input

    private void GetInput()
    {
        down = Input.GetButtonDown(inputName);
        pressed = Input.GetButton(inputName);
        up = Input.GetButtonUp(inputName);
    }

    private void GetAxisInput()
    {
        if (useRaw)
        {
            value = Input.GetAxisRaw(inputName);
        }
        else
        {
            value = Input.GetAxis(inputName);
        }

        down = pressed = up = !Mathf.Approximately(value, 0f);
    }
}
