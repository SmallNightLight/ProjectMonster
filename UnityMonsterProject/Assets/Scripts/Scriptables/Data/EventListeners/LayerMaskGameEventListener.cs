using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/LayerMask Event Listener")]
    public class LayerMaskGameEventListener : GameEventListenerBase<LayerMask>
    {
        [SerializeField] private LayerMaskVariable _event;

        public override IGameEvent<LayerMask> GetGameEventT() => _event;
    }
}