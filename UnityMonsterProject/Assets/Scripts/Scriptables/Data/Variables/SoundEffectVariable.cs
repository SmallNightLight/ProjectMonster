using ScriptableArchitecture.Core;
using UnityEngine;

namespace ScriptableArchitecture.Data
{
    [CreateAssetMenu(fileName = "SoundEffectVariable", menuName = "Scriptables/Variables/SoundEffect")]
    public class SoundEffectVariable : Variable<SoundEffect>
    {
        public void RaiseEffect(SoundEffectVariable effect)
        {
            Raise(effect.Value);
        }
    }
}