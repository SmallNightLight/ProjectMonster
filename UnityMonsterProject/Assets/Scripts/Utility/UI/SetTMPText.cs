using TMPro;
using UnityEngine;

public class SetTMPText : MonoBehaviour
{
    [SerializeField] private TMP_Text _textObject;

    public void SetCountDown(int newText)
    {
        if (_textObject)
        {
            string text = newText.ToString();
            if (text == "0")
                text = "Go";

            _textObject.text = text;
        }
    }
}