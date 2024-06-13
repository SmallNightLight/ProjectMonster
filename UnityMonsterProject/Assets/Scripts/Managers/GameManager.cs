using UnityEngine;

/// <summary>
/// Manages setup and update operations for Managers with the ISetupManager or IUpdateManager interface that are child of the GameManager GameObject
/// </summary>
public class GameManager : MonoBehaviour
{
    ISetupManager[] _setupManagers;
    IUpdateManager[] _updateManagers;

    /// <summary>
    /// Gets the active managers from the children and starts them
    /// </summary>
    private void Awake()
    {
        _setupManagers = GetComponentsInChildren<ISetupManager>();
        _updateManagers = GetComponentsInChildren<IUpdateManager>();

        StartManagers();
    }

    /// <summary>
    /// Updates the managers
    /// </summary>
    private void Update()
    {
        UpdateManagers();
    }

    /// <summary>
    /// Setups the managers with the ISetup interface
    /// </summary>
    private void StartManagers()
    {
        if (_setupManagers == null) return;

        foreach (ISetupManager setupManager in _setupManagers)
        {
            if (setupManager == null) continue;

            setupManager.Setup();
        }
    }

    /// <summary>
    /// Updates all managers with the IUpdate interface
    /// </summary>
    private void UpdateManagers()
    {
        if (_updateManagers == null) return;

        foreach (IUpdateManager updateManager in _updateManagers)
        {
            if (updateManager == null) continue;

            updateManager.Update();
        }
    }
}

/// <summary>
/// Interface for setup managers.
/// </summary>
public interface ISetupManager
{
    public void Setup();
}

/// <summary>
/// Interface for update managers.
/// </summary>
public interface IUpdateManager
{
    public void Update();
}