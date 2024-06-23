using ScriptableArchitecture.Data;
using UnityEngine;

public class MapTrack : MonoBehaviour
{
    [SerializeField] private GameDataReference _gameData;

    private void Start()
    {
        Instantiate(_gameData.Value.Map.MiniMapPrefab, transform);
    }
}