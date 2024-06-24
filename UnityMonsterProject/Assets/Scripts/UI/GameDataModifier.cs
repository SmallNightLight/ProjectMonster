using ScriptableArchitecture.Data;
using UnityEngine;

public class GameDataModifier : MonoBehaviour
{
    [SerializeField] private GameDataReference _gameData;

    public void SetMode(int mode)
    {
        switch (mode)
        {
            case 1:
                _gameData.Value.PlayerCount = 1;
                break;
            case 2:
                _gameData.Value.PlayerCount = 2;
                break;
            case 3:
                Debug.Log("No practive mode made");
                break;
        }
    }

    public void SetMap(MapDataVariable map)
    {
        _gameData.Value.Map = map.Value;
    }
}