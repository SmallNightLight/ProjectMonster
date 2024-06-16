using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/GameData Event Listener")]
    public class GameDataGameEventListener : GameEventListenerBase<GameData>
    {
        [SerializeField] private GameDataVariable _event;

        public override IGameEvent<GameData> GetGameEventT() => _event;
    }
}