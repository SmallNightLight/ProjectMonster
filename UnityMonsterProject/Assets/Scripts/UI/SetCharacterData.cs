using ScriptableArchitecture.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetCharacterData : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private GameDataReference _gameData;
    [SerializeField] private int _playerIndex;

    [SerializeField] private UnityEvent _onSelect;
    [SerializeField] private UnityEvent _onPress;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Select();
    }

    public void OnSelect(BaseEventData eventData)
    {
        Select();
    }

    public void Select()
    {
        _onSelect.Invoke();
    }

    public void Press()
    {
        _onPress.Invoke();
    }

    public void Set(CharacterDataVariable characterData)
    {
        _gameData.Value.CharacterDatas[_playerIndex] = characterData.Value;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GetComponent<Toggle>().isOn)
            Press();
    }
}