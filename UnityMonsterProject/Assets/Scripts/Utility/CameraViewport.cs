using ScriptableArchitecture.Data;
using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CameraViewport : MonoBehaviour
{
    private Camera[] _cameras;
    [SerializeField] private GameDataReference _gameData;

    [SerializeField] private KartBase _kartBase;

    private void Start()
    {
        _cameras = GetComponentsInChildren<Camera>();

        if (_kartBase == null || _gameData.Value.PlayerCount == 0)
        {
            Debug.LogWarning("Could not set camera viewport");
        }

        int playerNumber = _kartBase.Player - 1;

        switch (_gameData.Value.PlayerCount)
        {
            case 1:
                //Full screen
                for(int i = 0; i < _cameras.Length; i++)
                    _cameras[i].rect = new Rect(0f, 0f, 1f, 1f);
                break;
            case 2:
                //Split screen horizontally
                for (int i = 0; i < _cameras.Length; i++)
                    _cameras[i].rect = new Rect(playerNumber * 0.5f, 0f, 0.5f, 1f);
                break;
            default:
                Debug.LogWarning("Player count greater than 4 is not supported in this script.");
                break;
        }

    }
}