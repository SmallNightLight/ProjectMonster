using ScriptableArchitecture.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private List<InputAssetReference> _playerInput = new List<InputAssetReference>();

    private IInputManager[] _inputManagers;

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
        UpdateInput();
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
}