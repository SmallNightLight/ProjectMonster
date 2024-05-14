using ScriptableArchitecture.Core;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class InputAsset : IDataPoint
    {
        public int Player;
        public IInputType InputType;
        public InputData InputData;
    }

    public interface IInputType
    {
        void Setup();
        InputData UpdateInput();
        bool IsConnected();
    }
}