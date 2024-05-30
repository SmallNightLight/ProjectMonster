using ScriptableArchitecture.Data;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class BillboardToCamera : MonoBehaviour
{
    [SerializeField] private Vector3Reference _cameraPosition;
    [SerializeField] private Vector3Reference _rotationOffset;

    private void Update()
    {
        if (Application.isPlaying)
        {
            transform.LookAt(_cameraPosition.Value, Vector3.up);
            transform.Rotate(_rotationOffset.Value);
        }
#if UNITY_EDITOR
        else
        {
            transform.LookAt(SceneView.GetAllSceneCameras()[0].transform.position, Vector3.up);
            transform.Rotate(_rotationOffset.Value);
        }
#endif
    }
}