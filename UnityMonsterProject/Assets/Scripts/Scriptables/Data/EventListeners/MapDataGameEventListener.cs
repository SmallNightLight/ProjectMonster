using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/MapData Event Listener")]
    public class MapDataGameEventListener : GameEventListenerBase<MapData>
    {
        [SerializeField] private MapDataVariable _event;

        public override IGameEvent<MapData> GetGameEventT() => _event;
    }
}