using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/MovementStats Event Listener")]
    public class MovementStatsGameEventListener : GameEventListenerBase<MovementStats>
    {
        [SerializeField] private MovementStatsVariable _event;

        public override IGameEvent<MovementStats> GetGameEventT() => _event;
    }
}