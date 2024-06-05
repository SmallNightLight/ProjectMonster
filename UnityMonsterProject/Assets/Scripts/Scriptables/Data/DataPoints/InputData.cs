using ScriptableArchitecture.Core;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class InputData : IDataPoint
    {
        public bool IsAccelerating;
        public bool IsBraking;
        public float SteerInput;
        public bool IsTricking;
        public bool AbilityBoost;
        public bool Ability1;
    }
}