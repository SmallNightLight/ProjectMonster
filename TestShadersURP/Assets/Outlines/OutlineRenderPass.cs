using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineRenderPass : ScriptableRenderPass
{
    private OutlinePassSettings _settings;

    private RTHandle _normalsHandle;
    private RTHandle _temporaryBuffer;

    private Material _outlineMaterial;
    private Material _normalsMaterial;

    private FilteringSettings _filteringSettings;
    private RendererList _normalsRenderersList;
    private List<ShaderTagId> _shaderTagIdList;

    public OutlineRenderPass(OutlinePassSettings settings)
    {
        _settings = settings;
        _filteringSettings = new FilteringSettings(RenderQueueRange.opaque, _settings.LayerMask);
        _shaderTagIdList = new List<ShaderTagId>
        {
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
            new ShaderTagId("LightweightForward"),
            new ShaderTagId("SRPDefaultUnlit")
        };

        renderPassEvent = settings.RenderPassEvent;

        if (_outlineMaterial == null && _settings.OutlineShader != null)
            _outlineMaterial = CoreUtils.CreateEngineMaterial(_settings.OutlineShader);

        if (_normalsMaterial == null && _settings.NormalsShader != null)
            _normalsMaterial = CoreUtils.CreateEngineMaterial(_settings.NormalsShader);

        SetMaterialProperties();
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        //Reallocate Normals
        RenderTextureDescriptor textureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        textureDescriptor.colorFormat = RenderTextureFormat.ARGBFloat;
        textureDescriptor.depthBufferBits = 0;
        RenderingUtils.ReAllocateIfNeeded(ref _normalsHandle, textureDescriptor, FilterMode.Point);

        //Reallocate Color Buffer
        textureDescriptor.depthBufferBits = 0;
        RenderingUtils.ReAllocateIfNeeded(ref _temporaryBuffer, textureDescriptor, FilterMode.Bilinear);

        ConfigureTarget(_normalsHandle, renderingData.cameraData.renderer.cameraDepthTargetHandle);
        ConfigureClear(ClearFlag.Color, Color.black);

#if UNITY_EDITOR
        SetMaterialProperties();
#endif
    }

    private void SetMaterialProperties()
    {
        if (_outlineMaterial == null) return;

        _outlineMaterial.SetColor("_OutlineColor", _settings.OutlineColor);
        _outlineMaterial.SetFloat("_Thickness", _settings.Thickness);
        _outlineMaterial.SetFloat("_ThicknessThreshold", _settings.ThicknessThreshold);
        _outlineMaterial.SetFloat("_NormalsThreshold", _settings.NormalsThreshold);
        _outlineMaterial.SetFloat("_CrossMultiplier", _settings.CrossMultiplier);
        _outlineMaterial.SetFloat("_DepthThreshold", _settings.DepthThreshold);
        _outlineMaterial.SetFloat("_StepAngleThreshold", _settings.StepAngleThreshold);
        _outlineMaterial.SetFloat("_StepAngleMultiplier", _settings.StepAngleMultiplier);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Preview)
            return;

        if (_outlineMaterial == null || _normalsMaterial == null || renderingData.cameraData.renderer.cameraColorTargetHandle.rt == null || _temporaryBuffer.rt == null) return;

        CommandBuffer cmd = CommandBufferPool.Get();
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        //Normals
        DrawingSettings drawSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
        drawSettings.perObjectData = PerObjectData.None;
        drawSettings.enableDynamicBatching = false;
        drawSettings.enableInstancing = false;
        drawSettings.overrideMaterial = _normalsMaterial;

        RendererListParams normalsRenderersParams = new RendererListParams(renderingData.cullResults, drawSettings, _filteringSettings);
        _normalsRenderersList = context.CreateRendererList(ref normalsRenderersParams);
        cmd.DrawRendererList(_normalsRenderersList);

        //Pass in RT for Outlines shader
        cmd.SetGlobalTexture(Shader.PropertyToID("_SceneViewSpaceNormals"), _normalsHandle.rt);

        using (new ProfilingScope(cmd, new ProfilingSampler("Outlines")))
        {
            Blitter.BlitCameraTexture(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle, _temporaryBuffer, _outlineMaterial, 0);
            Blitter.BlitCameraTexture(cmd, _temporaryBuffer, renderingData.cameraData.renderer.cameraColorTargetHandle);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
        CoreUtils.Destroy(_outlineMaterial);
        CoreUtils.Destroy(_normalsMaterial);
        _normalsHandle?.Release();
        _temporaryBuffer?.Release();
    }
}