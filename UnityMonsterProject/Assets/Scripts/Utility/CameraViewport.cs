using ScriptableArchitecture.Data;
using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CameraViewport : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private IntReference _playerCount;

    [SerializeField] private KartBase _kartBase;

    private void Start()
    {
        _camera = GetComponent<Camera>();

        if (_kartBase == null || _playerCount.Value == 0)
        {
            Debug.LogWarning("Coukd not set camera viewport");
        }

        int playerNumber = _kartBase.Player - 1;

        switch (_playerCount.Value)
        {
            case 1:
                //Full screen
                _camera.rect = new Rect(0f, 0f, 1f, 1f);
                break;
            case 2:
                //Split screen horizontally
                _camera.rect = new Rect(playerNumber * 0.5f, 0f, 0.5f, 1f);
                break;
            default:
                Debug.LogWarning("Player count greater than 4 is not supported in this script.");
                break;
        }

    }
}