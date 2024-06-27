using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SelectButton : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Button>().Select();
    }
}