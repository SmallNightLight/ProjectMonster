using AlmenaraGames;
using UnityEngine;

[RequireComponent(typeof(MultiAudioSource), typeof(TrailRenderer))]
public class TrailSFX : MonoBehaviour
{
    private MultiAudioSource _multiAudioSource;
    private TrailRenderer _trailRenderer;

    private void Start()
    {
        TryGetComponent(out _multiAudioSource);
        TryGetComponent(out _trailRenderer);
    }

    private void Update()
    {
        _multiAudioSource.Mute = !_trailRenderer.emitting;
    }
}