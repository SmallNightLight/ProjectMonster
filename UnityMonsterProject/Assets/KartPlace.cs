using ScriptableArchitecture.Data;
using UnityEngine;

public class KartPlace : MonoBehaviour
{
    [SerializeField] private PlacementReference _placements;
    [SerializeField] private int _player;

    private void Start()
    {
        if (_placements.Value.GetCharacter(_player, out CharacterData characterData))
        {
            Instantiate(characterData.KartPrefab, transform);
            Instantiate(characterData.CharacterPrefab, transform);
        }
        else Debug.Log("Could not find player in placements");
    }
}