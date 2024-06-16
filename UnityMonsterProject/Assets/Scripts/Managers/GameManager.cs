using UnityEngine;

/// <summary>
/// Manages setup and update operations for Managers with the ISetupManager or IUpdateManager interface that are child of the GameManager GameObject
/// </summary>
public class GameManager : MonoBehaviour
{
    ISetupManager[] _setupManagers;
    IUpdateManager[] _updateManagers;
    IFirstFrameManager[] _firstFrameManagers;

    private bool _isFirstFrame;

    /// <summary>
    /// Gets the active managers from the children and starts them
    /// </summary>
    private void Awake()
    {
        _setupManagers = GetComponentsInChildren<ISetupManager>();
        _updateManagers = GetComponentsInChildren<IUpdateManager>();
        _firstFrameManagers = GetComponentsInChildren<IFirstFrameManager>();

        _isFirstFrame = true;
        StartManagers();
    }

    /// <summary>
    /// Updates the managers
    /// </summary>
    private void Update()
    {
        if (_isFirstFrame)
        {
            _isFirstFrame = false;
            FirstFrameManagers();
        }

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

            updateManager.UpdateManager();
        }
    }

    private void FirstFrameManagers()
    {
        if (_firstFrameManagers == null) return;

        foreach (IFirstFrameManager firstFrameManager in _firstFrameManagers)
        {
            if (firstFrameManager == null) continue;

            firstFrameManager.FirstFrame();
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
    public void UpdateManager();
}

public interface IFirstFrameManager
{
    public void FirstFrame();
}