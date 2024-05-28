using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private OutlinePassSettings _settings;
    private OutlineRenderPass _outlinePass;

    public override void Create()
    {
        _outlinePass = new OutlineRenderPass(_settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera)
            return;
#endif

        renderer.EnqueuePass(_outlinePass);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _outlinePass?.Dispose();
    }
}

[System.Serializable]
public class OutlinePassSettings
{
    public Shader OutlineShader;
    public Shader NormalsShader;

    public Color OutlineColor = Color.black;
    public float Thickness;
    public float ThicknessThreshold;
    public float NormalsThreshold;
    public float CrossMultiplier = 100f;
    public float DepthThreshold;
    public float StepAngleThreshold;
    public float StepAngleMultiplier;
    public float DistanceMultiplier;

    public RenderPassEvent RenderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    public LayerMask LayerMask;
}