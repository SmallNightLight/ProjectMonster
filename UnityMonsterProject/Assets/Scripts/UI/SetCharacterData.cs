using ScriptableArchitecture.Data;
using UnityEngine;

public class SetCharacterData : MonoBehaviour
{
    [SerializeField] private GameDataReference _gameData;
    [SerializeField] private int _playerIndex;

    public void Set(CharacterDataVariable characterData)
    {
        _gameData.Value.CharacterDatas[_playerIndex] = characterData.Value;
    }
}