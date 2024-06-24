using ScriptableArchitecture.Data;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MapIdentifier : MonoBehaviour
{
    [SerializeField] private GameDataReference _gameData;

    [SerializeField] private bool _faceUp = true;
    [SerializeField] private CharacterDataReference _characterData;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = _characterData.Value.CharacterIcon;
    }

    private void Update()
    {
        if (_faceUp)
        {
            transform.LookAt(transform.position + new Vector3(0, 1, 0));

            Vector3 euler = transform.eulerAngles;
            euler.y = _gameData.Value.Map.MiniMapRotation;
            transform.eulerAngles = euler;
        }
    }
}