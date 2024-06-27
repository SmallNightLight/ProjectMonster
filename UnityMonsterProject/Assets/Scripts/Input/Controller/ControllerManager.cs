using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour, IInputManager
{
    [SerializeField] private int _maxControllers = 2;
    [SerializeField] private float _scale = 4000;

    private List<InputAsset> _inputs = new List<InputAsset>();

    private Dictionary<InputAsset, (InputTest input, bool connected)> _players = new Dictionary<InputAsset, (InputTest input, bool connected)>();

    [SerializeField] private GameObject _inputPrefab;

    public bool TryAddPlayer(InputAsset playerInputAsset)
    {
        if (_maxControllers <= PlayerCount()) return false;

        InputTest input = Instantiate(_inputPrefab, transform).GetComponent<InputTest>();
        input.Setup(PlayerCount());

        if (input.Pad == null) 
        {
            Destroy(input.gameObject);
            return false;
        }
           

        _inputs.Add(playerInputAsset);
        _players.Add(playerInputAsset, (input, true));
        return true;
    }

    public void RemovePlayer(InputAsset playerInputAsset)
    {
        _inputs.Remove(playerInputAsset);
        _players.Remove(playerInputAsset);
    }

    public void UpdateInput()
    {
        if (PlayerCount() == 0) return;

        foreach (var v in _players)
        {
            Gamepad pad = v.Value.input.Pad;

            if (pad == null)
            {
                Debug.Log("Disconnected");
                _players[v.Key] = (v.Value.input, false);
                continue;
            }

            SetPlayerInput(v.Key, v.Value.input.GetData());
        }
    }

    private void SetPlayerInput(InputAsset playerInputAsset, InputData inputData)
    {
        playerInputAsset.InputData = inputData;
    }

    public int PlayerCount() => _inputs.Count;

    public int GetAvailablePlayerSlots()
    {
        return _maxControllers - PlayerCount();
    }

    public bool IsConnected(InputAsset inputAsset)
    {
        return true;
    }
}