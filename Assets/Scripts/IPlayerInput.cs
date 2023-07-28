/// <summary>
/// Interface to standardize player inputs in the game.
/// </summary>
public interface IPlayerInput
{
    /// <summary>
    /// Gets the name of the input.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the input control was pressed in the current frame.
    /// </summary>
    public bool Down { get; }

    /// <summary>
    /// Gets a value indicating whether the input control was released in the current frame.
    /// </summary>
    public bool Up { get; }

    /// <summary>
    /// Gets a value indicating whether the input control is currently being held down.
    /// </summary>
    public bool Pressed { get; }

    /// <summary>
    /// Gets the value of the input, valid only for axis inputs.
    /// </summary>
    public float Value { get; }

    public bool IsDisabled();

    public void Disable();

    public void Enable();

    /// <summary>
    /// Updates the properties of the input every frame.
    /// </summary>
    public void Update();
}
