using ScriptableArchitecture.Data;
using UnityEngine;
using WiimoteApi;

public class WiimoteInput : IInputType
{
    private Wiimote _mote;

    public void Setup()
    {
        WiimoteManager.FindWiimotes();

        if (!WiimoteManager.HasWiimote()) return;

        _mote = WiimoteManager.Wiimotes[0];
        _mote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
        _mote.Accel.CalibrateAccel(AccelCalibrationStep.A_BUTTON_UP);
        _mote.SendPlayerLED(true, false, true, false);
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