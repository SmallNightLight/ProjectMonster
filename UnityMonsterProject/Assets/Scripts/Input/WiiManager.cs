using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using WiimoteApi;

public class WiiManager : MonoBehaviour, IInputManager
{
    private Dictionary<InputAsset, Wiimote> _players = new Dictionary<InputAsset, Wiimote>();

    public bool TryGetInput(Wiimote mote, out InputData input)
    {
        if (mote == null)
        {
            input = null;
            return false;
        }

        //Acceleration input
        float acceleration = 0f;

        if (mote.Button.two) acceleration++;
        if (mote.Button.one) acceleration--;

        //Steering input
        float[] motion = mote.Accel.GetCalibratedAccelData();
        //
        //float mouseX = motion[0] - 0.3f;
        //float mouseY = -motion[1] + 0.3f;
        //
        //if (mouseX > -0.3f && mouseX < 0.3f) mouseX = 0;
        //
        //if (mouseY > -0.3f && mouseY < 0.3f) mouseY = 0;

        float steering = -Mathf.Clamp((motion[1] - 0.75f) * 3f, -1, 1);
        //Debug.Log(steering);
        //Debug.DrawLine(transform.position, transform.position + new Vector3(motion[0], 0, motion[1]));

        input = new InputData
        {
            AccelerateInput = acceleration,
            SteerInput = steering
        };
        return true;
    }

    private void CalibrateWiimote(Wiimote mote, int player)
    {
        mote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
        mote.Accel.CalibrateAccel(AccelCalibrationStep.A_BUTTON_UP);
        mote.SendPlayerLED(player == 1, player == 2, player == 3, player == 4);
    }

    public bool TryAddPlayer(InputAsset playerInputAsset)
    {
        if (!WiimoteManager.HasWiimote()) return false;

        if (WiimoteManager.Wiimotes.Count <= PlayerCount()) return false;

        Wiimote playerMote = WiimoteManager.Wiimotes[PlayerCount()];
        _players.Add(playerInputAsset, playerMote);
        CalibrateWiimote(playerMote, playerInputAsset.Player);
        return true;
    }

    public void RemovePlayer(InputAsset playerInputAsset)
    {
        _players.Remove(playerInputAsset);
    }

    public void UpdateInput()
    {
        foreach (var player in _players)
        {
            if (TryGetInput(player.Value, out var input))
                player.Key.InputData = input;
        }
    }

    public int PlayerCount() => _players.Count;

    public int GetAvailablePlayerSlots()
    {
        WiimoteManager.FindWiimotes();

        if (!WiimoteManager.HasWiimote()) return 0;

        return  WiimoteManager.Wiimotes.Count - PlayerCount();
    }
}