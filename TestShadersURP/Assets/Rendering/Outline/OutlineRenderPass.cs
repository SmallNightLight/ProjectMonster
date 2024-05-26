using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineRenderPass : ScriptableRenderPass
{
    private OutlinePassSettings _settings;

    private RTHandle _source;
    private RTHandle _destination;
    private RTHandle _temp;

    private string _profilerTag;

    private Material _material;

    public OutlineRenderPass(OutlinePassSettings settings, string tag)
    {
        profilingSampler = new ProfilingSampler("Outlines");

        _settings = settings;

        renderPassEvent = settings.RenderPassEvent;
        _profilerTag = tag;

        if (_material == null && _settings.OutlineShader != null)
            _material = CoreUtils.CreateEngineMaterial(_settings.OutlineShader);
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

        _material.SetColor("_Color", _settings.OutlineColor);
        _material.SetFloat("_Threshhold", _settings.Threshhold);
        _material.SetFloat("_Thickness", _settings.Thickness);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Preview)
            return;

        if (_material == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get(_profilerTag);

        using (new ProfilingScope(cmd, new ProfilingSampler("Outline Pass")))
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