using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class ControllerManager : MonoBehaviour, IInputManager
{
    private List<InputAsset> _inputs = new List<InputAsset>();

    private Dictionary<InputAsset, (Gamepad gamepad, bool connected)> _players = new Dictionary<InputAsset, (Gamepad gamepad, bool connected)>();

    public bool TryAddPlayer(InputAsset playerInputAsset)
    {
        if (1 <= PlayerCount()) return false;

        Gamepad pad = DS4.getConroller();

        if (pad == null) return false;

        _inputs.Add(playerInputAsset);
        _players.Add(playerInputAsset, (pad, true));
        return true;
    }

    public void RemovePlayer(InputAsset playerInputAsset)
    {
        _inputs.Remove(playerInputAsset);
    }

    public void UpdateInput()
    {
        if (PlayerCount() == 0) return;

        //1. player - WASD

        foreach (var v in _players)
        {
            Gamepad pad = v.Value.gamepad;

            if (pad == null)
            {
                try 
                {
                    _players[v.Key] = (DS4.getConroller(), true);
                }
                catch
                {
                    _players[v.Key] = (null, false);
                    continue;
                } 
            }

            Quaternion gyro = DS4.getRotation(4000 * Time.deltaTime);
            Vector3 eulerAngles = gyro.eulerAngles;
            float yaw = eulerAngles.y;
            float steer = Mathf.InverseLerp(-90, 90, Mathf.DeltaAngle(0, yaw));

            InputData input = new InputData
            {
                IsAccelerating = pad.buttonEast.isPressed,
                IsBraking = pad.buttonSouth.isPressed,
                //SteerInput = pad.leftStick.ReadValue().x,
                SteerInput  = steer,
                IsTricking = false,
                AbilityBoost = pad.buttonWest.isPressed,
                Ability1 = pad.buttonNorth.isPressed,
               
                Press = pad.buttonEast.wasPressedThisFrame,
                Back = pad.buttonSouth.isPressed,

                MoveUp = pad.dpad.up.wasPressedThisFrame,
                MoveDown = pad.dpad.down.wasPressedThisFrame,
                MoveRight = pad.dpad.right.wasPressedThisFrame,
                MoveLeft = pad.dpad.left.wasPressedThisFrame
            };

            SetPlayerInput(v.Key, input);
        }
    }

    private void SetPlayerInput(InputAsset playerInputAsset, InputData inputData)
    {
        playerInputAsset.InputData = inputData;
    }

    public int PlayerCount() => _inputs.Count;

    public int GetAvailablePlayerSlots()
    {
        return 1 - PlayerCount();
    }

    public bool IsConnected(InputAsset inputAsset)
    {
        return true;
    }
}