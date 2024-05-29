using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/AbilityData Event Listener")]
    public class AbilityDataGameEventListener : GameEventListenerBase<AbilityData>
    {
        [SerializeField] private AbilityDataVariable _event;

        public override IGameEvent<AbilityData> GetGameEventT() => _event;
    }
}