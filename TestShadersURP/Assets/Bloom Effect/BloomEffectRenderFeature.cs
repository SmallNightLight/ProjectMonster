using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class BloomEffectRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader _bloomShader;
    [SerializeField] private Shader _compositeShader;

    [SerializeField] private BloomEffectSettings _settings;

    private Material _bloomMaterial;
    private Material _compositeMaterial;

    private BloomEffectPass _customPass;

    public override void Create()
    {
        _bloomMaterial = CoreUtils.CreateEngineMaterial(_bloomShader);
        _compositeMaterial = CoreUtils.CreateEngineMaterial(_compositeShader);

        _customPass = new BloomEffectPass(_settings, _bloomMaterial, _compositeMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera)
            return;
#endif

        renderer.EnqueuePass(_customPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            _customPass.ConfigureInput(ScriptableRenderPassInput.Depth);
            _customPass.ConfigureInput(ScriptableRenderPassInput.Color);
            _customPass.SetTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        }
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(_bloomMaterial);
        CoreUtils.Destroy(_compositeMaterial);
    }
}

[System.Serializable]
public class BloomEffectSettings
{
    [Header("Bloom")]
    public float Threshold = 0.9f;
    [Range(0, 1)] public float Scatter = 0.7f;
    public int Clamp = 65472;
    [Range(0, 10)] public int MaxIterations = 6;

    [Header("Benday Dots")]
    public int Density = 30;
    public float Cutoff = 0.1f;
    public float BackgroundBloomIntensity = 10.0f;
    public float ColorIntensityHigh = 50.0f;
    public float ColorIntensityLow = 7.0f;
}