using ScriptableArchitecture.Data;
using UnityEngine;

public class KartBase : MonoBehaviour
{
    [SerializeField] private InputAssetReference _input;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public InputData Input => _input.Value.InputData;
}