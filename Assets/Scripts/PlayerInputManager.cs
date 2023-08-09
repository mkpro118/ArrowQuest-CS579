using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerInputManager
{
    private bool useDefaults = true;
    private float xMove = 0f;
    private float zMove = 0f;
    private float xRot = 0f;
    private float yRot = 0f;
    private bool jump = false;

    // Public Properties
    public float Horizontal 
    { 
        get 
        {
            if (!useDefaults || defaultInputs[defaultInputIndexMap["Horizontal"]].IsDisabled()) {
                throw new InvalidOperationException(PropertyAccessErrMsg("Horizontal"));
            }
            return xMove;
        } 
    }

    public float Vertical 
    { 
        get 
        {
            if (!useDefaults || defaultInputs[defaultInputIndexMap["Vertical"]].IsDisabled()){
                throw new InvalidOperationException(PropertyAccessErrMsg("Vertical"));
            }
            return zMove;
        } 
    }

    public float MouseX 
    { 
        get 
        {
            if (!useDefaults || defaultInputs[defaultInputIndexMap["Mouse X"]].IsDisabled()){
                throw new InvalidOperationException(PropertyAccessErrMsg("Mouse X"));
            }
            return xRot;
        } 
    }

    public float MouseY 
    { 
        get 
        {
            if (!useDefaults || defaultInputs[defaultInputIndexMap["Mouse Y"]].IsDisabled()){
                throw new InvalidOperationException(PropertyAccessErrMsg("Mouse Y"));
            }
            return yRot;
        } 
    }

    public bool Jump 
    { 
        get 
        {
            if (!useDefaults || defaultInputs[defaultInputIndexMap["Jump"]].IsDisabled()){
                throw new InvalidOperationException(PropertyAccessErrMsg("Jump"));
            }
            return jump;
        } 
    }


    private readonly IPlayerInput[] defaultInputs = new IPlayerInput[] {
        new PlayerButtonInput("Horizontal", isAxis: true, useRaw: true, disabled: false),
        new PlayerButtonInput("Vertical", isAxis: true, useRaw: true, disabled: false),
        new PlayerButtonInput("Mouse X", isAxis: true, useRaw: true, disabled: false),
        new PlayerButtonInput("Mouse Y", isAxis: true, useRaw: true, disabled: false),
        new PlayerButtonInput("Jump", disabled: false),
    };

    private readonly Dictionary<string, int> defaultInputIndexMap = new Dictionary<string, int>
    {
        { "Horizontal", 0},
        { "Vertical", 1},
        { "Mouse X", 2},
        { "Mouse Y", 3},
        { "Jump", 4},
    };

    private IPlayerInput[] playerInputs;
    private readonly Dictionary<string, int> inputIndexMap;

    public PlayerInputManager(IPlayerInput[] playerInputs = null, Dictionary<string, int> inputIndexMap = null)
    {
        this.playerInputs = playerInputs;

        if (playerInputs == null) return;

        if (inputIndexMap != null && inputIndexMap.Count == playerInputs.Length)
        {
            this.inputIndexMap = inputIndexMap;
            return;
        }

        inputIndexMap = new Dictionary<string, int>();

        int i = 0;
        foreach (IPlayerInput input in playerInputs)
        {
            inputIndexMap[input.Name] = i++;
        }
    }

    public IPlayerInput this[string name]
    {
        get {
            if (useDefaults && defaultInputIndexMap.TryGetValue(name, out int idx))
            {
                return defaultInputs[idx];
            }

            if (playerInputs != null && inputIndexMap != null) {
                if (inputIndexMap.TryGetValue(name, out idx))
                {
                    return playerInputs[idx];
                }
            }

            throw new KeyNotFoundException(
                "Input " + name + " is not set to be tracked."
                + " Available inputs are: " + GetTrackedInputs());
        }
    }

    public void Update()
    {
        if (useDefaults) {
            UpdateDefaultInputs();
            xMove = defaultInputs[defaultInputIndexMap["Horizontal"]].Value;
            zMove = defaultInputs[defaultInputIndexMap["Vertical"]].Value;
            xRot = defaultInputs[defaultInputIndexMap["Mouse X"]].Value;
            yRot = defaultInputs[defaultInputIndexMap["Mouse Y"]].Value;
            jump = defaultInputs[defaultInputIndexMap["Jump"]].Down;
        }

        if (playerInputs != null)
        {
            UpdateCustomInputs();
        }
    }

    private void UpdateDefaultInputs()
    {
        foreach (IPlayerInput input in defaultInputs)
        {
            input.Update();
        }
    }

    private void UpdateCustomInputs() {
        foreach (IPlayerInput input in playerInputs)
        {
            input.Update();
        }
    }

    public void Disable(string name)
    {
        playerInputs[inputIndexMap[name]].Disable();
    }

    public void Enable(string name)
    {
        playerInputs[inputIndexMap[name]].Disable();
    }

    public void DisableDefault(string name)
    {
        defaultInputs[defaultInputIndexMap[name]].Disable();
    }

    public void EnableDefault(string name)
    {
        defaultInputs[defaultInputIndexMap[name]].Enable();
    }

    public void DisableDefaults() { 
        useDefaults = false;
        foreach(IPlayerInput input in defaultInputs) { input.Disable(); }
    }

    public void EnableDefaults() { 
        useDefaults = true; 
        foreach(IPlayerInput input in defaultInputs) { input.Enable(); }
    }

    private static string PropertyAccessErrMsg(string property)
    {
        return (
            "Cannot read default property " + property + " because it has been disabled."
            + " Use PlayerInputManager.EnableDefaults() to have access to default properties."
        );
    }

    private string GetTrackedInputs()
    {
        string[] availableInputs;
        if (playerInputs != null)
        {
            availableInputs = new string[defaultInputs.Length + playerInputs.Length];
        }
        else
        {
            availableInputs = new string[defaultInputs.Length];
        }

        int i = 0;
        foreach (IPlayerInput input in defaultInputs)
        {
            availableInputs[i++] = input.Name;
        }
        if (playerInputs != null)
        {
            foreach (IPlayerInput input in playerInputs)
            {
                availableInputs[i++] = input.Name;
            }
        }
        return "(" + string.Join(", ", availableInputs + ")");
    }
}
