using ScriptableArchitecture.Data;
using UnityEngine;

public class KartPlace : MonoBehaviour
{
    [SerializeField] private PlacementReference _placements;
    [SerializeField] private int _player;

    [SerializeField] private Transform _kartVisual;
    [SerializeField] private Transform _characterVisual;

    private void Start()
    {
        if (_placements.Value.GetCharacter(_player, out CharacterData characterData))
        {
            Instantiate(characterData.KartPrefab, _kartVisual);
            Instantiate(characterData.CharacterPrefab, _characterVisual);
        }
        else Debug.Log("Could not find player in placements");
    }
}