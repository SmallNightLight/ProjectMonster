using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardManager : MonoBehaviour, IInputManager
{
    [SerializeField] private int _maxKeyboardPlayers = 2;

    private List<InputAsset> _inputs = new List<InputAsset>();

    public bool TryAddPlayer(InputAsset playerInputAsset)
    {
        if (_maxKeyboardPlayers <= PlayerCount()) return false;

        _inputs.Add(playerInputAsset);
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
        for(int i = 0; i < PlayerCount(); i++)
        {
            InputData input;

            switch (i)
            {
                case 0:
                    input = new InputData
                    {
                        IsAccelerating = Input.GetKey(KeyCode.W),
                        IsBraking = Input.GetKey(KeyCode.S),
                        SteerInput = Input.GetAxis("HorizontalAD"),
                        IsTricking = Input.GetKey(KeyCode.Q),
                        AbilityBoost = Input.GetKey(KeyCode.Space),
                        Ability1 = Input.GetKey(KeyCode.E)
                    };
                    break;

                case 1:
                    input = new InputData
                    {
                        IsAccelerating = Input.GetKey(KeyCode.UpArrow),
                        IsBraking = Input.GetKey(KeyCode.DownArrow),
                        SteerInput = Input.GetAxis("HorizontalArrows"),
                        IsTricking = Input.GetKey(KeyCode.Backspace),
                        AbilityBoost = Input.GetKey(KeyCode.Return),
                        Ability1 = Input.GetKey(KeyCode.RightShift)
                    };
                    break;

                default:
                    input = new InputData();
                    Debug.Log($"Input manager not active - More then {_maxKeyboardPlayers} keyboard players");
                    break;
            }

            SetPlayerInput(_inputs[i], input);
        }
    }

    private void SetPlayerInput(InputAsset playerInputAsset, InputData inputData)
    {
        playerInputAsset.InputData = inputData;
    }

    public int PlayerCount() => _inputs.Count;

    public int GetAvailablePlayerSlots()
    {
        return _maxKeyboardPlayers - PlayerCount();
    }

    public bool IsConnected(InputAsset inputAsset)
    {
        return true;
    }
}