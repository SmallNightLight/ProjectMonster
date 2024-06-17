using UnityEditor.Overlays;
using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [CreateAssetMenu(fileName = "PlacementVariable", menuName = "Scriptables/Variables/Placement")]
    public class PlacementVariable : Variable<Placement>
    {
    }
}