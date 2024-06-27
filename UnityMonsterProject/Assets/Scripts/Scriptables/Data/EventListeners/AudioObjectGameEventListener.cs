using AlmenaraGames;
using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/AudioObject Event Listener")]
    public class AudioObjectGameEventListener : GameEventListenerBase<AudioObject>
    {
        [SerializeField] private AudioObjectVariable _event;

        public override IGameEvent<AudioObject> GetGameEventT() => _event;
    }
}