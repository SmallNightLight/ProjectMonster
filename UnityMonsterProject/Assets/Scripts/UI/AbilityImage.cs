using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityImage : MonoBehaviour
{
    [SerializeField] private CharacterDataReference _characterData;
    [SerializeField] private List<Image> _images;
    [SerializeField] private bool _isBoost;

    private void Start()
    {
        if (_images == null) return;

        foreach (var image in _images)
        {
            if (_isBoost)
                image.sprite = _characterData.Value.BoostAbility.Value.AbilityIcon;
            else
                image.sprite = _characterData.Value.MainAbility.Value.AbilityIcon;
        }
    }
}