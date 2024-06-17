using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(KartBase))]
public class KartLabs : MonoBehaviour
{
    private int _currentLab = 1;
    private bool _reachedLabCheckpoint = false;

    public UnityEvent<int> ChangedLabCount;

    private KartBase _base;

    private void Start()
    {
        _base = GetComponent<KartBase>();
    }

    private void NextCheckpoint()
    {
        if (_reachedLabCheckpoint)
        {
            _currentLab++;
        }
        
        _reachedLabCheckpoint = !_reachedLabCheckpoint;

        ChangedLabCount.Invoke(_currentLab);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Checkpoint checkpoint))
        {
            if (checkpoint.IsFinish == _reachedLabCheckpoint)
                NextCheckpoint();
        }
    }

    public int GetCurrentLab() => _currentLab;
}