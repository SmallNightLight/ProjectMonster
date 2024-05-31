using ScriptableArchitecture.Data;
using UnityEngine;

public class SetCameraData : MonoBehaviour
{
    [SerializeField] private Vector3Reference _cameraPosition;


    private void Update()
    {
        _cameraPosition.Value = transform.position;
    }
}