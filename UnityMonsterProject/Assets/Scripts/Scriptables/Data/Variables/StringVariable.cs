using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [CreateAssetMenu(fileName = "StringVariable", menuName = "Scriptables/Variables/String")]
    public class StringVariable : Variable<string>
    {
    }
}