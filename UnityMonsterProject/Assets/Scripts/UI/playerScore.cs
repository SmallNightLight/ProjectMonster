using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class playerScore : MonoBehaviour
{
    [SerializeField] private TMP_Text _placementText;
    [SerializeField] private Image _iconImage;

    public void Setup(int place, Sprite characterIcon)
    {
        _placementText.text = place.ToString();
        _iconImage.sprite = characterIcon;
    }
}