using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AOLinesPass : ScriptableRenderPass
{
    AOLinesPassSettings _settings;

    private RTHandle _source;
    private RTHandle _destination;
    private RTHandle _temp;

    private string _profilerTag;
    private Material _material;

    public AOLinesPass(AOLinesPassSettings settings, string tag)
    {
        _settings = settings;
        _profilerTag = tag;
        renderPassEvent = settings.RenderPassEvent;

        if (_material == null && settings.Shader != null)
            _material = CoreUtils.CreateEngineMaterial(settings.Shader);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (_material == null)
            return;

        ConfigureInput(ScriptableRenderPassInput.Normal);

        var descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;

        RenderingUtils.ReAllocateIfNeeded(ref _temp, descriptor, name: "_TemporaryColorTexture");

        var renderer = renderingData.cameraData.renderer;
        _source = _destination = renderer.cameraColorTargetHandle;

        //_material.SetColor("_Color", _settings.OutlineColor);
        //_material.SetFloat("_Threshhold", _settings.Threshhold);
        //_material.SetFloat("_Thickness", _settings.Thickness);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Preview || _material == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get(_profilerTag);

        using (new ProfilingScope(cmd, new ProfilingSampler("Ambient Occlusion Line Effect")))
        {
            Blitter.BlitCameraTexture(cmd, _source, _temp, _material, 0);
            Blitter.BlitCameraTexture(cmd, _temp, _destination, Vector2.one);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        _source = null;
        _destination = null;
    }

    public void Dispose()
    {
        _temp?.Release();
    }
}