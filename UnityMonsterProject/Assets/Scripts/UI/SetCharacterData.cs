using ScriptableArchitecture.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SetCharacterData : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    [SerializeField] private GameDataReference _gameData;
    [SerializeField] private int _playerIndex;

    [SerializeField] private UnityEvent _onSelect;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _onSelect.Invoke();
    }

    public void OnSelect(BaseEventData eventData)
    {
        _onSelect.Invoke();
    }

    public void Set(CharacterDataVariable characterData)
    {
        _gameData.Value.CharacterDatas[_playerIndex] = characterData.Value;
    }
}