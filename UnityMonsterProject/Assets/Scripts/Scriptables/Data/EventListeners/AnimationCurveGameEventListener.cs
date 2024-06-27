using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/AnimationCurve Event Listener")]
    public class AnimationCurveGameEventListener : GameEventListenerBase<AnimationCurve>
    {
        [SerializeField] private AnimationCurveVariable _event;

        public override IGameEvent<AnimationCurve> GetGameEventT() => _event;
    }
}