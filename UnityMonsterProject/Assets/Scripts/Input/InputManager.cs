using System.Collections;
using UnityEngine;
using WiimoteApi;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Wiimote _mote;

    private void Start()
    {
        WiimoteManager.FindWiimotes();

        if (!WiimoteManager.HasWiimote()) return;

        _mote = WiimoteManager.Wiimotes[0];
        _mote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
        _mote.Accel.CalibrateAccel(AccelCalibrationStep.A_BUTTON_UP);
        _mote.SendPlayerLED(true, false, true, false);
    }

    private void Update()
    {
        if (_mote == null) return;

        if (_mote.Button.home)
        {
            _mote.Accel.CalibrateAccel(AccelCalibrationStep.A_BUTTON_UP);
        }

        if (_mote != null)
        {
            float[] accel = _mote.Accel.GetCalibratedAccelData();
            float[] zeroP = _mote.Accel.GetAccelZeroPoints();

            var mouseX = accel[0] - 0.3f;
            var mouseY = -accel[1] + 0.3f;

            if (mouseX > -0.3f && mouseX < 0.3f) mouseX = 0;

            if (mouseY > -0.3f && mouseY < 0.3f) mouseY = 0;

            Debug.Log("X:" + mouseX);
            Debug.Log("Y:" + mouseY);
        }
    }
}