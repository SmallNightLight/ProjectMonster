using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    public void LoadScene(string scneneName)
    {
        SceneManager.LoadScene(scneneName);
    }

    public void LoadSceneAsync(string scneneName)
    {
        SceneManager.LoadSceneAsync(scneneName);
    }
}