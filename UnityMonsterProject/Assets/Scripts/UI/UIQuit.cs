using UnityEditor;
using UnityEngine;

public class UIQuit : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
            EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif

    }
}