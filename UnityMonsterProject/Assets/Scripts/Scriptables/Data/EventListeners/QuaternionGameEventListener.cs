using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/Quaternion Event Listener")]
    public class QuaternionGameEventListener : GameEventListenerBase<Quaternion>
    {
        [SerializeField] private QuaternionVariable _event;

        public override IGameEvent<Quaternion> GetGameEventT() => _event;
    }
}