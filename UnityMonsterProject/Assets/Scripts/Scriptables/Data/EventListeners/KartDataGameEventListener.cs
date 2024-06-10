using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/KartData Event Listener")]
    public class KartDataGameEventListener : GameEventListenerBase<KartData>
    {
        [SerializeField] private KartDataVariable _event;

        public override IGameEvent<KartData> GetGameEventT() => _event;
    }
}