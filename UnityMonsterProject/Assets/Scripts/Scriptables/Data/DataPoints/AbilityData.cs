using ScriptableArchitecture.Core;
using System.Collections.Generic;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class AbilityData : IDataPoint
    {
        public List<KartStatsVariable> AdditionalStats;
        public float Duration;
        public float CoolDown;
        public List<WorldEffect> WorldEffects;
    }
}