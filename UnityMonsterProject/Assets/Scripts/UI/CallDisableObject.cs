using UnityEngine;

public class CallDisableObject : MonoBehaviour
{
    public void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public void EnableObject()
    {
        gameObject.SetActive(true);
    }
}