using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/Int Event Listener")]
    public class IntGameEventListener : GameEventListenerBase<int>
    {
        [SerializeField] private IntVariable _event;

        public override IGameEvent<int> GetGameEventT() => _event;
    }
}