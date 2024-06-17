using UnityEditor.Overlays;
using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/Placement Event Listener")]
    public class PlacementGameEventListener : GameEventListenerBase<Placement>
    {
        [SerializeField] private PlacementVariable _event;

        public override IGameEvent<Placement> GetGameEventT() => _event;
    }
}