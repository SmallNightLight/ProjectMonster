using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [CreateAssetMenu(fileName = "AnimationCurveVariable", menuName = "Scriptables/Variables/AnimationCurve")]
    public class AnimationCurveVariable : Variable<AnimationCurve>
    {
    }
}