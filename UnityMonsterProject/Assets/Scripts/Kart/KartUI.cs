using ScriptableArchitecture.Data;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(KartBase))]
public class KartUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameDataReference _gameData;
    [SerializeField] private FloatReference _countDownTimer;

    [Header("Components")]
    [SerializeField] private TMP_Text _lapCountText;
    [SerializeField] private TMP_Text _placementText;
    private KartBase _base;

    int _lapCount;

    private void Start()
    {
        _base = GetComponent<KartBase>();
        _lapCount = 0;
        IncreaseLapCounter();
    }

    private void Update()
    {
        
    }

    private void IncreaseLapCounter()
    {
        _lapCount++;
        if (_lapCountText)
            _lapCountText.text = _lapCount.ToString() + "/" + _gameData.Value.Map.TotalLaps;
    }
}