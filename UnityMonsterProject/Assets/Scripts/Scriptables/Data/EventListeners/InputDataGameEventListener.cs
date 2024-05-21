using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/InputData Event Listener")]
    public class InputDataGameEventListener : GameEventListenerBase<InputData>
    {
        [SerializeField] private InputDataVariable _event;

        public override IGameEvent<InputData> GetGameEventT() => _event;
    }
}