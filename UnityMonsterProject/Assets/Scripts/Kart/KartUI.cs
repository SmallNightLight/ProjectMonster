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
    [SerializeField] private PlacementReference _placementReference;
    private KartBase _base;

    private void Start()
    {
        _base = GetComponent<KartBase>();
        UpdateLapCounter(1);
    }

    public void Update()
    {
        UpdatePlace(_placementReference.Value.GetPlace(_base.Player));
    }

    public void UpdateLapCounter(int labCount)
    {
        if (_lapCountText)
            _lapCountText.text = labCount.ToString() + "/" + _gameData.Value.Map.TotalLaps;
    }

    public void UpdatePlace(int place)
    {
        if (_placementText)
            _placementText.text = place.ToString();
    }
}