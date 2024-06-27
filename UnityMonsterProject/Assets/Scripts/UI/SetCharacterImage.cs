using ScriptableArchitecture.Data;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetCharacterImage : MonoBehaviour
{
    [SerializeField] private CharacterDataReference _characterData;

    private void Start()
    {
        GetComponent<Image>().sprite = _characterData.Value.CharacterIconSquare;
    }
}