using ScriptableArchitecture.Core;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class AbilityData : IDataPoint
    {
        public List<WorldEffect> WorldEffects;
        public List<Variable> AdditionalStats;
        public float Duration;

        [Header("Current")]
        public float ElapsedTime;
    }
}