using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/Bool Event Listener")]
    public class BoolGameEventListener : GameEventListenerBase<bool>
    {
        [SerializeField] private BoolVariable _event;

        public override IGameEvent<bool> GetGameEventT() => _event;
    }
}