using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/PlayerData Event Listener")]
    public class CharacterDataGameEventListener : GameEventListenerBase<CharacterData>
    {
        [SerializeField] private CharacterDataVariable _event;

        public override IGameEvent<CharacterData> GetGameEventT() => _event;
    }
}