using ScriptableArchitecture.Data;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour, ISetupManager, IFirstFrameManager
{
    [SerializeField] private GameDataReference _gameData;
    [SerializeField] private List<GameObject> _playerPrefabs;

    [SerializeField] private bool _spawnPlayers;
    [SerializeField] private bool _spawnMap;
    [SerializeField] private bool _spawnSplines;
    [SerializeField] private GameState _startGameState = GameState.StartCinematic;

    [SerializeField] private FloatReference _countDownTime;

    private List<int> _endedPlayers = new List<int>();

    [SerializeField] private float _endTime = 3f;
    [SerializeField] private float _maxEndTime = 30f;

    [SerializeField] private string _podiumSceneName = "Podium";

    [SerializeField] private PlacementReference _placements;

    public void Setup()
    {
        _placements.Value.Clear();

        //Setup Map
        if (_spawnMap)
            Instantiate(_gameData.Value.Map.MapPrefab);


        //Setup splines
        RoadSplines roadSplines;
        if (_spawnSplines && _gameData.Value.Map.SplinePrefab != null)
        {
            GameObject splines = Instantiate(_gameData.Value.Map.SplinePrefab);
            roadSplines = splines.GetComponent<RoadSplines>();

            if (splines.TryGetComponent(out RoadGenerator roadGenerator))
                roadGenerator.GenerateMesh();
        }
        else
        {
            roadSplines = FindObjectOfType<RoadSplines>();
        }


        //Setup players
        if (_spawnPlayers)
        {
            for (int i = 0; i < _gameData.Value.PlayerCount; i++)
            {
                GameObject player = Instantiate(_playerPrefabs[i]);
                KartBase kartBase = player.GetComponentInChildren<KartBase>();
                Rigidbody kartRigidBody = player.GetComponentInChildren<Rigidbody>();

                if (!kartBase)
                {
                    Debug.Log($"No kartBase found in player Object: {player.name}");
                    continue;
                }

                if (kartRigidBody != null && _gameData.Value.Map.PlayerSpawnPositions != null && _gameData.Value.Map.PlayerSpawnPositions.Count > i)
                    kartRigidBody.MovePosition(_gameData.Value.Map.PlayerSpawnPositions[i]);
                else
                {
                    Debug.Log($"Could not assign spawn position to: {player.name}");
                }

                if (kartRigidBody != null && _gameData.Value.Map.PlayerSpawnRotations != null && _gameData.Value.Map.PlayerSpawnRotations.Count > i)
                    kartRigidBody.MoveRotation(Quaternion.Euler(_gameData.Value.Map.PlayerSpawnRotations[i]));
                else
                {
                    Debug.Log($"Could not assign rotation to: {player.name}");
                }

                if (roadSplines)
                    kartBase.Splines = roadSplines;
                else
                    Debug.Log("No valid roadSplines");

                if (_gameData.Value.TryGetCharacterData(i, out CharacterData characterData))
                {
                    kartBase.CharacterData = characterData;
                }
                else
                {
                    Debug.Log("No valid character data");
                }
            }
        }
        

        //Setup splines to all karts (players and bots)
        if (roadSplines != null)
        {
            foreach (KartBase kartBase in FindObjectsOfType<KartBase>())
            {
                kartBase.Splines = roadSplines;
            }
        }
    }

    public void FirstFrame()
    {
        //Set gamestate
        _gameData.Value.State = _startGameState;
        _gameData.Raise();

        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(_countDownTime.Value);
        _gameData.Value.State = GameState.Gameplay;
        _gameData.Raise();
    }

    public void PlayerReachedEnd(int player)
    {
        _endedPlayers.Add(player);

        int endedPlayers = 0;
        for(int i = 1; i < _gameData.Value.PlayerCount + 1; i++)
        {
            if (_endedPlayers.Contains(i))
                endedPlayers++;
        }

        if (endedPlayers != _gameData.Value.PlayerCount)
        {
            StartCoroutine(End(_maxEndTime));
        }
        else
        {
            if (endRoutine != null)
                StopCoroutine(endRoutine);

            StartCoroutine(End(_endTime));
        }
    }

    Coroutine endRoutine;
    private IEnumerator End(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadSceneAsync(_podiumSceneName);
    }
}