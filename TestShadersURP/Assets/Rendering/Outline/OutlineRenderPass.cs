using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

public class OutlineRenderPass : ScriptableRenderPass
{
    private OutlinePassSettings _settings;

    private RTHandle _source;
    private RTHandle _destination;
    private RTHandle _temp;

    private RTHandle _depthHandle;
    private RTHandle _normalsHandle;

    private string _profilerTag;

    private Material _material;
    private Material _normalsMaterial;

    public OutlineRenderPass(OutlinePassSettings settings, string tag)
    {
        profilingSampler = new ProfilingSampler("Outlines");

        _settings = settings;

        renderPassEvent = settings.RenderPassEvent;
        _profilerTag = tag;

        if (_material == null && _settings.OutlineShader != null)
            _material = CoreUtils.CreateEngineMaterial(_settings.OutlineShader);

        if (_normalsMaterial == null && _settings.NormalsShader != null)
            _normalsMaterial = CoreUtils.CreateEngineMaterial(_settings.NormalsShader);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (_material == null)
            return;
        
        ConfigureInput(ScriptableRenderPassInput.Normal);
        ConfigureInput(ScriptableRenderPassInput.Depth);

        var descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;

        RenderingUtils.ReAllocateIfNeeded(ref _temp, descriptor, name: "_TemporaryColorTexture");

        var normalDescriptor = descriptor;
        normalDescriptor.depthBufferBits = 0;
        normalDescriptor.graphicsFormat = DepthNormalOnlyPass.GetGraphicsFormat();

        RenderingUtils.ReAllocateIfNeeded(ref _temp, normalDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_CameraNormalsTexture");


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
            Blitter.BlitCameraTexture(cmd, _source, _temp, _normalsMaterial, 0);
            Blitter.BlitCameraTexture(cmd, _temp, _destination, Vector2.one);

            //Blitter.BlitCameraTexture(cmd, _source, _temp, _material, 0);
            //Blitter.BlitCameraTexture(cmd, _temp, _destination, Vector2.one);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void SetTarget(RTHandle depthHandle)
    {
        _depthHandle = depthHandle;

        ConfigureTarget(_depthHandle);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        _source = null;
        _destination = null;
        _depthHandle = null;
        _normalsHandle = null;
    }

    public void Dispose()
    {
        _temp?.Release();
    }
}