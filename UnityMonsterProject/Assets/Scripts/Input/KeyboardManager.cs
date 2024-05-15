using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;

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
        if (PlayerCount() == 1)
        {
            InputData playerInput = new InputData
            {
                SteerInput = Input.GetAxis("Horizontal"),
                AccelerateInput = Input.GetAxis("Vertical")
            };

            SetPlayerInput(_inputs[0], playerInput);
        }
        else if (PlayerCount() == 2)
        {
            //Player 1 and 2 have the same input for testing
            InputData playerInput = new InputData
            {
                SteerInput = Input.GetAxis("Horizontal"),
                AccelerateInput = Input.GetAxis("Vertical")
            };

            SetPlayerInput(_inputs[0], playerInput);
            SetPlayerInput(_inputs[1], playerInput);
        }
        else
        {
            Debug.Log($"Input manager not active - More then {_maxKeyboardPlayers} keyboard players");
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
}