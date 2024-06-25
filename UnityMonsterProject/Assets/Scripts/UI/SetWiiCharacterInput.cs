using ScriptableArchitecture.Data;
using UnityEngine;

public class SetWiiCharacterInput : MonoBehaviour
{
    [SerializeField] private GameDataReference _gameData;
    [SerializeField] private InputAssetReference _inputAsset;

    [SerializeField] private CharacterDataReference _characterDataUp;
    [SerializeField] private CharacterDataReference _characterDataDown;
    [SerializeField] private CharacterDataReference _characterDataRight;
    [SerializeField] private CharacterDataReference _characterDataLeft;

    private void Update()
    {
        if (_inputAsset.Value.InputData.MoveDown)
        {
            _gameData.Value.CharacterDatas[_inputAsset.Value.Player - 1] = _characterDataDown.Value;
        }
        else if (_inputAsset.Value.InputData.MoveUp)
        {
            _gameData.Value.CharacterDatas[_inputAsset.Value.Player - 1] = _characterDataUp.Value;
        }
        else if (_inputAsset.Value.InputData.MoveRight)
        {
            _gameData.Value.CharacterDatas[_inputAsset.Value.Player - 1] = _characterDataRight.Value;
        }
        else if (_inputAsset.Value.InputData.MoveLeft)
        {
            _gameData.Value.CharacterDatas[_inputAsset.Value.Player - 1] = _characterDataLeft.Value;
        }
    }
}