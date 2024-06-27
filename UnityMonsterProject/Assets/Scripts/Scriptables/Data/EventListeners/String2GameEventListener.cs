using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/String2 Event Listener")]
    public class String2GameEventListener : GameEventListenerBase<String2>
    {
        [SerializeField] private String2Variable _event;

        public override IGameEvent<String2> GetGameEventT() => _event;
    }
}