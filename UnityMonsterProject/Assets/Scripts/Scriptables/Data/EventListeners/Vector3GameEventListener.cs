using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/Vector3 Event Listener")]
    public class Vector3GameEventListener : GameEventListenerBase<Vector3>
    {
        [SerializeField] private Vector3Variable _event;

        public override IGameEvent<Vector3> GetGameEventT() => _event;
    }
}