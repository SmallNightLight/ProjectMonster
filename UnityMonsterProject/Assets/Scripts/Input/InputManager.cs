using ScriptableArchitecture.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private List<InputAssetReference> _playerInput = new List<InputAssetReference>();
    [SerializeField] private IntReference _playerCount;

    private IInputManager[] _inputManagers;

    private Dictionary<InputAsset, IInputManager> _assets = new Dictionary<InputAsset, IInputManager>();

    private void Start()
    {
        _inputManagers = GetComponentsInChildren<IInputManager>();
        AddPlayers();
    }

    private void AddPlayers()
    {
        foreach (InputAssetReference input in _playerInput)
        {
            AddPlayer(input.Value);
        }
    }

    private void AddPlayer(InputAsset input)
    {
        bool addedPlayer = false;

        foreach(var inputManager in _inputManagers)
        {
            if (inputManager.GetAvailablePlayerSlots() > 0 && inputManager.TryAddPlayer(input))
            {
                addedPlayer = true;

                if (!_assets.ContainsKey(input))
                    _assets.Add(input, inputManager);
                else
                    _assets[input] = inputManager;

                break;
            }
        }

        if (!addedPlayer)
        {
            Debug.Log($"Could not handle player {input.Player} input - No available input types");
        }
    }

    private void Update()
    {
        ProcessConnection();
        UpdateInput();
    }

    private void ProcessConnection()
    {
        List<InputAsset> disconnectedAssets = new List<InputAsset>();

        foreach(var input in _assets)
        {
            if (!input.Value.IsConnected(input.Key))
            {
                Debug.LogWarning("Input device disconnected");
                disconnectedAssets.Add(input.Key);
                input.Value.RemovePlayer(input.Key);
            }
        }

        foreach(var asset in disconnectedAssets)
        {
            AddPlayer(asset);
        }
    }

    private void UpdateInput()
    {
        foreach (IInputManager manager in _inputManagers)
        {
            manager.UpdateInput();
        }
    }
}

public interface IInputManager
{
    public bool TryAddPlayer(InputAsset playerInputAsset);

    public void RemovePlayer(InputAsset playerInputAsset);

    public void UpdateInput();

    public int PlayerCount();

    public int GetAvailablePlayerSlots();

    public bool IsConnected(InputAsset inputAsset);
}