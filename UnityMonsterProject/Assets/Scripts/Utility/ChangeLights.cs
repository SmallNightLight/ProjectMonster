using ScriptableArchitecture.Data;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ChangeLights : MonoBehaviour
{
    [SerializeField] private Material _nextMaterial;

    public void Change(GameData data)
    {
        if (data.State == GameState.Gameplay)
        {
            GetComponent<MeshRenderer>().material = _nextMaterial;
        }
    }
}