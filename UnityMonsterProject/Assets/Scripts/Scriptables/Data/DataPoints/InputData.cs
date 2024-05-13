using ScriptableArchitecture.Core;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class InputData : IDataPoint
    {
        public float Horizontal;
        public float Vertical;
    }
}