using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/SoundEffect Event Listener")]
    public class SoundEffectGameEventListener : GameEventListenerBase<SoundEffect>
    {
        [SerializeField] private SoundEffectVariable _event;

        public override IGameEvent<SoundEffect> GetGameEventT() => _event;
    }
}