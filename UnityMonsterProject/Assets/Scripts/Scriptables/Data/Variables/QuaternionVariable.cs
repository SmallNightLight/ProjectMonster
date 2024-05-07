using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [CreateAssetMenu(fileName = "QuaternionVariable", menuName = "Scriptables/Variables/Quaternion")]
    public class QuaternionVariable : Variable<Quaternion>
    {
    }
}