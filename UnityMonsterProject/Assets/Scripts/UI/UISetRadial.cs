using ScriptableArchitecture.Data;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UISetRadial : MonoBehaviour
{
    private Image _image;

    [SerializeField] private FloatReference _amount;

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    public void Update()
    {
        _image.fillAmount = _amount.Value;
    }
}