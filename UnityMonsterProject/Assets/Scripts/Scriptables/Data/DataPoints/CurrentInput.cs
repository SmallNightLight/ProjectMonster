using ScriptableArchitecture.Core;
using System.Collections.Generic;

namespace ScriptableArchitecture.Data
{
    [System.Serializable]
    public class CurrentInput : IDataPoint
    {
        public List<InputData> PlayerInput;
    }
}