using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/Float Event Listener")]
    public class FloatGameEventListener : GameEventListenerBase<float>
    {
        [SerializeField] private FloatVariable _event;

        public override IGameEvent<float> GetGameEventT() => _event;
    }
}