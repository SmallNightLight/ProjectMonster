using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AOLinesRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private AOLinesPassSettings _settings;
    private AOLinesPass _pass;

    public override void Create()
    {
        _pass = new AOLinesPass(_settings, name);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
#if UNITY_EDITOR
        if (renderingData.cameraData.isSceneViewCamera)
            return;
#endif

        renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing)
    {
        _pass.Dispose();
    }
}

[System.Serializable]
public class AOLinesPassSettings
{
    public Shader Shader;

    public RenderPassEvent RenderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
}