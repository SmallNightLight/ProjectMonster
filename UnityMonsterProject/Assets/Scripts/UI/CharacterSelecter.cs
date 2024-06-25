using ScriptableArchitecture.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CharacterSelecter : MonoBehaviour
{
    [SerializeField] private StringReference _sceneName;

    [SerializeField] private GameDataReference _gameData;
    [SerializeField] private UnityEvent _onCharactersSelected;

    private int _player1Selected, _player2Selected;
    private bool _loading;

    private void Start()
    {
        _player1Selected = _player2Selected = 0;
    }

    public void PlayerSelected(int player)
    {
        if (player == 1)
            _player1Selected = 1;
        else if (player == 2)
            _player2Selected = 1;

        if (_player1Selected + _player2Selected >= _gameData.Value.PlayerCount)
        {
            _onCharactersSelected.Invoke();
            LoadScene();
        }
    }

    public void PlayerDeSelected(int player)
    {
        if (player == 1)
            _player1Selected = 0;
        else if (player == 2)
            _player2Selected = 0;
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