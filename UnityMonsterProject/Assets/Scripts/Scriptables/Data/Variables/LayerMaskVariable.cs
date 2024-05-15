using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [CreateAssetMenu(fileName = "LayerMaskVariable", menuName = "Scriptables/Variables/LayerMask")]
    public class LayerMaskVariable : Variable<LayerMask>
    {
    }
}