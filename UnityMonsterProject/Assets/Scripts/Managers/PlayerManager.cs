using Cinemachine;
using ScriptableArchitecture.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerManager : MonoBehaviour, ISetupManager
{
    [SerializeField] private GameDataReference _gameData;
    [SerializeField] private List<GameObject> _playerPrefabs;

    public void Setup()
    {
        //Setup Map
        GameObject map = Instantiate(_gameData.Value.Map.MapPrefab);
        GameObject splines = Instantiate(_gameData.Value.Map.SplinePrefab);

        if (splines.TryGetComponent(out RoadGenerator roadGenerator))
            roadGenerator.GenerateMesh();

        RoadSplines roadSplines = splines.GetComponent<RoadSplines>();

        //Setup players
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

            CinemachineVirtualCamera vm = player.GetComponentInChildren<CinemachineVirtualCamera>();
            Transform cameraT = player.GetComponentInChildren<CameraViewport>().gameObject.transform;
            
            vm.ForceCameraPosition(_gameData.Value.Map.PlayerSpawnPositions[i], Quaternion.identity);
            //vm.PreviousStateIsValid = true;
            vm.transform.position = _gameData.Value.Map.PlayerSpawnPositions[i];
            Debug.Log(vm.gameObject.transform.position);
        }

        //Setup bots
        //For now just assign splines
        foreach ( var v in FindObjectsOfType<KartBaseBot>())
        {
            v.Splines = roadSplines;
        }
    }
}