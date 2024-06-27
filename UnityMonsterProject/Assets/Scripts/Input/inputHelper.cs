using ScriptableArchitecture.Data;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class inputHelper : MonoBehaviour
{
    [SerializeField] private InputAssetReference _input;
    [SerializeField] private Sprite _nextSprite;
    [SerializeField] private GameObject _g;
    [SerializeField] private GameObject _disable;

    private void Start()
    {

        if (_input.Value.InputData.IsPS)
        {
            if (_g == null)
            {
                GetComponent<Image>().sprite = _nextSprite;
            }
            else
            {
                _g.SetActive(true);
                GetComponent<Image>().enabled = false;
            }
            
            if (TryGetComponent(out Animation anim))
                anim.enabled = false;

            if (_disable != null)
                _disable.SetActive(false);
        }        
    }
}