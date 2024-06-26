using ScriptableArchitecture.Data;
using UnityEngine;

public class MiniMapSplit : MonoBehaviour
{
    [SerializeField] private GameDataReference _gameData;
    [SerializeField] private int _players;

    private void Start()
    {
        if (_gameData.Value.PlayerCount != _players)
            Destroy(gameObject);
    }
}