using ScriptableArchitecture.Core;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class AbilityData : IDataPoint
    {
        public List<WorldEffect> WorldEffects;
        [RequireInterface(typeof(IStatsVariable))] public List<Variable> AdditionalStats;
        public float Duration;

        [Header("Current")]
        public float ElapsedTime;
    }
}