using ScriptableArchitecture.Data;
using UnityEngine;

public class KeyboardInput : IInputType
{
    public void Setup()
    {
        
    }

    public InputData UpdateInput()
    {
        return new InputData
        {
            SteerInput = Input.GetAxis("Horizontal"),
            AccelerateInput = Input.GetAxis("Vertical")
        };
    }

    public bool IsConnected()
    {
        return true;
    }
}