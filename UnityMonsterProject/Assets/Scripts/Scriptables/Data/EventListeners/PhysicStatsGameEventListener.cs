using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/PhysicStats Event Listener")]
    public class PhysicStatsGameEventListener : GameEventListenerBase<PhysicStats>
    {
        [SerializeField] private PhysicStatsVariable _event;

        public override IGameEvent<PhysicStats> GetGameEventT() => _event;
    }
}