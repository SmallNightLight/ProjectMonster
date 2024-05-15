using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [AddComponentMenu("GameEvent Listeners/InputAsset Event Listener")]
    public class InputAssetGameEventListener : GameEventListenerBase<InputAsset>
    {
        [SerializeField] private InputAssetVariable _event;

        public override IGameEvent<InputAsset> GetGameEventT() => _event;
    }
}