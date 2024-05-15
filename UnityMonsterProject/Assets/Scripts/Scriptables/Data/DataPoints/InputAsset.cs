using ScriptableArchitecture.Core;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class InputAsset : IDataPoint
    {
        public int Player;
        public InputData InputData;
    }
}