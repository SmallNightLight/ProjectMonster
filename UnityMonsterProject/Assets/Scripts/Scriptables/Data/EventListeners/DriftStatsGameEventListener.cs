using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/DriftStats Event Listener")]
    public class DriftStatsGameEventListener : GameEventListenerBase<DriftStats>
    {
        [SerializeField] private DriftStatsVariable _event;

        public override IGameEvent<DriftStats> GetGameEventT() => _event;
    }
}