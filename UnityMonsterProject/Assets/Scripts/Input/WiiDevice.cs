using System.Linq;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;




public struct WiiDeviceState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('W', 'I', 'I', 'M');

    [InputControl(name = "button1", layout = "Button", bit = 0)]
    [InputControl(name = "button2", layout = "Button", bit = 1)]
    public int buttons;

    
}

[InputControlLayout(stateType = typeof(WiiDeviceState))]
[InitializeOnLoad]
public class WiiDevice : InputDevice, IInputUpdateCallbackReceiver
{
   
    public ButtonControl button1 { get; private set; }
    [InputControl]
    public ButtonControl button2 { get; private set; }

    protected override void FinishSetup()
    {
        base.FinishSetup();

        button1 = GetChildControl<ButtonControl>("button1");
        button2 = GetChildControl<ButtonControl>("button2");
    }

    static WiiDevice()
    {
        InputSystem.RegisterLayout<WiiDevice>(matches: new InputDeviceMatcher().WithInterface(pattern: "WiiDevice"));

        if (!InputSystem.devices.Any(device => device is WiiDevice))
            InputSystem.AddDevice<WiiDevice>();
    }



    [MenuItem("Tools/Add WiiDevice")]
    public static void Initialize()
    {
        var device = InputSystem.AddDevice<WiiDevice>();

        //InputSystem.QueueStateEvent(device, new WiiDeviceState());
    }

    public void OnUpdate()
    {
        var state = new WiiDeviceState();
        
    }
}