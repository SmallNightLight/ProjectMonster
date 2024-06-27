using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/SuspensionStats Event Listener")]
    public class SuspensionStatsGameEventListener : GameEventListenerBase<SuspensionStats>
    {
        [SerializeField] private SuspensionStatsVariable _event;

        public override IGameEvent<SuspensionStats> GetGameEventT() => _event;
    }
}