using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/Vector3Int Event Listener")]
    public class Vector3IntGameEventListener : GameEventListenerBase<Vector3Int>
    {
        [SerializeField] private Vector3IntVariable _event;

        public override IGameEvent<Vector3Int> GetGameEventT() => _event;
    }
}