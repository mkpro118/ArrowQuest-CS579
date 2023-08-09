using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[System.Serializable]
public class PlayerVRInput : IPlayerInput
{
    // The name of the input (button or axis).
    private string inputName;

    // The name of the input (button or axis).
    private InputFeatureUsage usage;

    // Last Frame's input
    private bool lastFrameState;

    // Indicates whether the input control was pressed in the current frame.
    private bool down;

    // Indicates whether the input control was released in the current framce.
    private bool up;

    // Indicates whether the input control is being held down.
    private bool pressed;

    // Indicates whether the input is an axis input (true) or a button input (false).
    private bool isAxis;

    // Indicates which axis is being determined, only for axis with Vector2 results
    private int axisIndex = -1;

    // If there are multiple axes
    private bool applyAxis = false;


    // The value of the input (valid only for axis inputs).
    private float value = 0f;

    // Is the input disabled
    private bool disabled = false;

    // Left and Right Hand Controller Characteristics
    private static readonly 
        InputDeviceCharacteristics controllerCharacteristics = 
            (InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Right);

    // List of VR Input Devices
    private List<InputDevice> devices;

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
    /// Creates a new instance of the PlayerVRInput class with the specified input settings.
    /// </summary>
    /// <param name="inputName">The name of the input (button or axis).</param>
    /// <param name="isAxis">True if the input is an axis, False if it's a button.</param>
    /// <param name="useRaw">True to use raw axis input values, False otherwise.</param>
    /// <param name="disabled">True to initialize the input as disabled, False otherwise.</param>
    public PlayerVRInput(
            InputFeatureUsage usage,
            bool isAxis = false,
            int axisIndex = -1,
            bool disabled = false
        )
    {
        this.usage = usage;
        this.inputName = usage.ToString();
        this.isAxis = isAxis;
        this.axisIndex = axisIndex;
        this.disabled = disabled;

        if (isAxis && axisIndex != -1)
        {
            this.applyAxis = true;
        }
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

        devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        if (devices.Count != 2)
        {
            Debug.LogErrorFormat(
                "Invalid number of Devices. Found {}",
                devices.Count
            );
        }

        if (isAxis && applyAxis) { GetAxisInputVector2(); }
        else if (isAxis) { GetAxisInput(); }
        else { GetInput(); }
    }

    // Private methods for getting input states based on button or axis input

    private void GetInput()
    {
        bool state = false;
        foreach (InputDevice device in devices)
        {
            state = device.TryGetFeatureValue(usage.As<bool>(), out bool primaryButtonState) // did get a value
                        && primaryButtonState // the value we got
                        || state; // defaults to false, or last found state
        }

        if (state != lastFrameState) // Button state changed since last frame
        {
            if (state)
            {
                down = true;
            } else
            {
                up = true;
            }
            lastFrameState = state;
        } else
        {
            down = false;
            up = false;
            pressed = state;
        }
    }

    private void GetAxisInput()
    {
        foreach (var device in devices)
        {
            if (device.TryGetFeatureValue(usage.As<float>(), out float _value))
            {
                value = _value;
                if (!Mathf.Approximately(value, 0f)) break;
            }
        }

        down = pressed = up = !Mathf.Approximately(value, 0f);
    }

    private void GetAxisInputVector2()
    {
        foreach (var device in devices)
        {
            if (device.TryGetFeatureValue(usage.As<Vector2>(), out Vector2 axis))
            {
                value = axisIndex == 0 ? axis.x : axis.y;
                if (!Mathf.Approximately(value, 0f)) break;
            }
        }

        down = pressed = up = !Mathf.Approximately(value, 0f);
    }
}
