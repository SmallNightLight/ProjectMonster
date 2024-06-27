using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class DS4
{
    // Gyroscope
    public ButtonControl gyroX = null;
    public ButtonControl gyroY = null;
    public ButtonControl gyroZ = null;

    // Acceleration
    // public static ButtonControl acclX = null;
    // public static ButtonControl acclY = null;
    // public static ButtonControl acclZ = null;

    public Gamepad controller = null;

    public Gamepad getConroller(string layoutFile = null, int padIndex = 0)
    {
        // Read layout from JSON file
        string layout = layoutFile;// == null ? "Assets/Scripts/Input/Controller/customLayout.json" : layoutFile);

        if (padIndex == 0)
            InputSystem.RegisterLayoutOverride(layout, $"DualShock4GamepadHIDCUST");

        var gamepads = Gamepad.all;

        if (gamepads.Count <= padIndex)
            return null;

        var ds4 = gamepads[padIndex];

        if (ds4 == null)
            return null;

        // Overwrite the default layout
       

        controller = ds4;
        bindControls(controller);
        return controller;
    }

    private  void bindControls(Gamepad ds4)
    {
        gyroX = ds4.GetChildControl<ButtonControl>("gyro X 14");
        gyroY = ds4.GetChildControl<ButtonControl>("gyro Y 16");
        gyroZ = ds4.GetChildControl<ButtonControl>("gyro Z 18");
        // acclX = ds4.GetChildControl<ButtonControl>("accl X 20");
        // acclY = ds4.GetChildControl<ButtonControl>("accl Y 22");
        // acclZ = ds4.GetChildControl<ButtonControl>("accl Z 24");
    }

    public Quaternion getRotation(float scale = 1)
    {
        float x = processRawData(gyroX.ReadValue()) * scale;
        float y = processRawData(gyroY.ReadValue()) * scale;
        float z = -processRawData(gyroZ.ReadValue()) * scale;
        return Quaternion.Euler(x, y, z);
    }

    private float processRawData(float data)
    {
        return data > 0.5 ? 1 - data : -data;
    }
}