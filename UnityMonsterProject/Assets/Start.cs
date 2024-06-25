using ScriptableArchitecture.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    [SerializeField] private InputAssetReference _inputAsset;
    [SerializeField] private StringReference _sceneName;

    private bool _loading;

    private void Update()
    {
        if (_inputAsset.Value.InputData.Press)
        {
            LoadScene();
        }
    }

    public void LoadScene()
    {
        if (!_loading)
        {
            _loading = true;
            SceneManager.LoadSceneAsync(_sceneName.Value);
        }
    }
}