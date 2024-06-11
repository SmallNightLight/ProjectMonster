using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;
using WiimoteApi;

public class WiiManager : MonoBehaviour, IInputManager
{
    private Dictionary<InputAsset, Wiimote> _players = new Dictionary<InputAsset, Wiimote>();
    private Dictionary<InputAsset, float> _lastPlayerInput = new Dictionary<InputAsset, float>();
    private Dictionary<InputAsset, int> _lastAccelInput = new Dictionary<InputAsset, int>();

    [SerializeField] private float _maxNoInputTime = 5.0f;

    public float ChangeSteering = 0f;

    public bool TryGetInput(Wiimote mote, InputAsset key, out InputData input)
    {
        if (mote == null)
        {
            input = null;
            return false;
        }

        int status = mote.ReadWiimoteData();

        if (status > 0)
        {
            //Reset input timer
            _lastPlayerInput[key] = 0f;
        }
        else
        {
            _lastPlayerInput[key] += Time.deltaTime;
        }

        //Calculate steering input
        float[] motion = mote.Accel.GetCalibratedAccelData();
        float steering = -Mathf.Clamp((motion[1] - 0.75f) * 3f, -1, 1);
        //Debug.Log(steering);
        Debug.DrawLine(transform.position, transform.position + new Vector3(motion[0], 0, motion[1]));

        int accel = mote.Accel.accel[0];
        bool isTricking = Mathf.Abs(accel - _lastAccelInput[key]) > 10;

        input = new InputData
        {
            IsAccelerating = mote.Button.two,
            IsBraking = mote.Button.one,
            SteerInput = steering,
            IsTricking = isTricking,
            AbilityBoost = mote.Button.b,
            Ability1 = mote.Button.d_down | mote.Button.d_up | mote.Button.d_left | mote.Button.d_right
        };

        _lastAccelInput[key] = accel;

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
        _lastPlayerInput[playerInputAsset] = 0;
        _lastAccelInput[playerInputAsset] = 0;
        CalibrateWiimote(playerMote, playerInputAsset.Player);

        return true;
    }

    public void RemovePlayer(InputAsset playerInputAsset)
    {
        WiimoteManager.Cleanup(_players[playerInputAsset]);

        _players.Remove(playerInputAsset);
        _lastPlayerInput.Remove(playerInputAsset);
        _lastAccelInput.Remove(playerInputAsset);
    }

    public void UpdateInput()
    {
        foreach (var player in _players)
        {
            if (TryGetInput(player.Value, player.Key, out var input))
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

    public bool IsConnected(InputAsset inputAsset)
    {
        if (!_lastPlayerInput.ContainsKey(inputAsset)) return false;

        return _lastPlayerInput[inputAsset] < _maxNoInputTime;
    }
}