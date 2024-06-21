using ScriptableArchitecture.Data;
using UnityEngine;

public class PlayerScores : MonoBehaviour
{
    [SerializeField] private PlacementReference _placements;
    [SerializeField] private GameObject _scorePrefab;

    private int _currentPlace;

    private void Start()
    {
        _currentPlace = 1;
    }

    public void AddPlayerScore(int player)
    {
        if (_placements.Value.GetCharacter(player, out CharacterData data))
        {
            Instantiate(_scorePrefab, transform);


            _currentPlace++;
        }
        else
        {
            Debug.Log("No character dtaa found on placements");
        }
    }
}