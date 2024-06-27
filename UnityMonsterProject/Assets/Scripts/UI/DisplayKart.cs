using ScriptableArchitecture.Data;
using UnityEngine;

public class DisplayKart : MonoBehaviour
{
    [SerializeField] private GameDataReference _gameData;
    [SerializeField] private int _playerIndex;

    [SerializeField] private Transform _kartVisual;
    [SerializeField] private Transform _characterVisual;

    private void Start()
    {
        UpdateCharacter();
    }

    public void UpdateCharacter()
    {
        foreach (Transform t in _kartVisual)
        {
            Destroy(t.gameObject);
        }

        foreach (Transform t in _characterVisual)
        {
            Destroy(t.gameObject);
        }

        Instantiate(_gameData.Value.CharacterDatas[_playerIndex].KartPrefab, _kartVisual);
        Instantiate(_gameData.Value.CharacterDatas[_playerIndex].CharacterPrefab, _characterVisual);
    }
}