using UnityEngine;

using ScriptableArchitecture.Core;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class QuaternionReference : Reference<Quaternion, QuaternionVariable, QuaternionInstancer>
    {
    }
}