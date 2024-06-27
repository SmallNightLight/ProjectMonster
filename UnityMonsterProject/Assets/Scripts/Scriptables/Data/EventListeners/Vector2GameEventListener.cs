using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/Vector2 Event Listener")]
    public class Vector2GameEventListener : GameEventListenerBase<Vector2>
    {
        [SerializeField] private Vector2Variable _event;

        public override IGameEvent<Vector2> GetGameEventT() => _event;
    }
}