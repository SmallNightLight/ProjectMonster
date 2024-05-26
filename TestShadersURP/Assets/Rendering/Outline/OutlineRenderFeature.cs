using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private OutlinePassSettings _settings;
    private OutlineRenderPass _outlinePass;

    public override void Create()
    {
        _outlinePass = new OutlineRenderPass(_settings, name);
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
        _outlinePass.Dispose();
    }
}

[System.Serializable]
public class OutlinePassSettings
{
    public Shader OutlineShader;

    public Color OutlineColor;
    public float Threshhold;
    public float Thickness;

    public RenderPassEvent RenderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
}