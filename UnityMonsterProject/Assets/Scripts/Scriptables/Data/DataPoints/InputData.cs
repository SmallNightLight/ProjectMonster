using ScriptableArchitecture.Core;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class InputData : IDataPoint
    {
        public float AccelerateInput;
        public float SteerInput;
    }
}