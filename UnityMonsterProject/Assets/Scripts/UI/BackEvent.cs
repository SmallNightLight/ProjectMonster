using UnityEngine;
using UnityEngine.Events;

public class BackEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent _localBackEvent;

    public void Back()
    {
        if (gameObject.activeSelf)
            _localBackEvent.Invoke();
    }
}