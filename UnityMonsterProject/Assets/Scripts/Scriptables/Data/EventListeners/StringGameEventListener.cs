using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/String Event Listener")]
    public class StringGameEventListener : GameEventListenerBase<string>
    {
        [SerializeField] private StringVariable _event;

        public override IGameEvent<string> GetGameEventT() => _event;
    }
}