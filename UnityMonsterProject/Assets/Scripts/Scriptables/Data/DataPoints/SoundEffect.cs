using ScriptableArchitecture.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace ScriptableArchitecture.Data
{
    /// <summary>
    /// DataPoint for sound data. Included the mixer, clip, volum, pitch and loop
    /// </summary>
    [System.Serializable]
    public class SoundEffect : IDataPoint
    {
        public AudioMixerGroup audioMixerSnapshot;

        public AudioClip AudioClip;
        [Range(0, 1)]
        public float Volume = 0.5f;
        [Range(0, 3)]
        public float Pitch = 1f;

        public bool Loop = false;
    }
}