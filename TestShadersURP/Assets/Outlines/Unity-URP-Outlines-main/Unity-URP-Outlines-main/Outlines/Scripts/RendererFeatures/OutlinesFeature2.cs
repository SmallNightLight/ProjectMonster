using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class OutlinesFeature2 : ScriptableRendererFeature 
{
    private class OutlinePass2 : ScriptableRenderPass {
        
        private readonly Material screenSpaceOutlineMaterial;
        private OutlinePassSettings settings;

        private FilteringSettings filteringSettings;

        private readonly List<ShaderTagId> shaderTagIdList;
        private readonly Material normalsMaterial;

        private RTHandle normals;
        private RendererList normalsRenderersList;

        RTHandle temporaryBuffer;

        public OutlinePass2(RenderPassEvent renderPassEvent, LayerMask layerMask,
            OutlinePassSettings settings) {
            this.settings = settings;
            this.renderPassEvent = renderPassEvent;

            if (screenSpaceOutlineMaterial == null && settings.OutlineShader != null)
                screenSpaceOutlineMaterial = CoreUtils.CreateEngineMaterial(settings.OutlineShader);

            if (normalsMaterial == null && settings.NormalsShader != null)
                normalsMaterial = CoreUtils.CreateEngineMaterial(settings.NormalsShader);

            SetMaterialProperties();

            filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);

            shaderTagIdList = new List<ShaderTagId> {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefaultUnlit")
            };
        }

        private void SetMaterialProperties()
        {
            if (screenSpaceOutlineMaterial == null) return;

            screenSpaceOutlineMaterial.SetColor("_OutlineColor", settings.OutlineColor);
            screenSpaceOutlineMaterial.SetFloat("_Thickness", settings.Thickness);
            screenSpaceOutlineMaterial.SetFloat("_ThicknessThreshold", settings.ThicknessThreshold);
            screenSpaceOutlineMaterial.SetFloat("_NormalsThreshold", settings.NormalsThreshold);
            screenSpaceOutlineMaterial.SetFloat("_CrossMultiplier", settings.CrossMultiplier);
            screenSpaceOutlineMaterial.SetFloat("_DepthThreshold", settings.DepthThreshold);
            screenSpaceOutlineMaterial.SetFloat("_StepAngleThreshold", settings.StepAngleThreshold);
            screenSpaceOutlineMaterial.SetFloat("_StepAngleMultiplier", settings.StepAngleMultiplier);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
            // Normals
            RenderTextureDescriptor textureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            textureDescriptor.colorFormat = RenderTextureFormat.ARGBFloat;
            textureDescriptor.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref normals, textureDescriptor, FilterMode.Point);
            
            // Color Buffer
            textureDescriptor.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref temporaryBuffer, textureDescriptor, FilterMode.Bilinear);

            ConfigureTarget(normals, renderingData.cameraData.renderer.cameraDepthTargetHandle);
            ConfigureInput(ScriptableRenderPassInput.Depth);
            ConfigureClear(ClearFlag.Color, new Color(0, 0, 0, 0));
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            if (screenSpaceOutlineMaterial  == null|| normalsMaterial == null || 
                renderingData.cameraData.renderer.cameraColorTargetHandle.rt == null || temporaryBuffer.rt == null) return;

            CommandBuffer cmd = CommandBufferPool.Get();
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
                
            //Normals
            DrawingSettings drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            drawSettings.perObjectData = PerObjectData.None;
            drawSettings.enableDynamicBatching = false;
            drawSettings.enableInstancing = false;
            drawSettings.overrideMaterial = normalsMaterial;
            
            RendererListParams normalsRenderersParams = new RendererListParams(renderingData.cullResults, drawSettings, filteringSettings);
            normalsRenderersList = context.CreateRendererList(ref normalsRenderersParams);
            cmd.DrawRendererList(normalsRenderersList);
            
            //Pass in RT for Outlines shader
            cmd.SetGlobalTexture(Shader.PropertyToID("_SceneViewSpaceNormals"), normals.rt);
            
            using (new ProfilingScope(cmd, new ProfilingSampler("ScreenSpaceOutlines"))) {

                Blitter.BlitCameraTexture(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle, temporaryBuffer, screenSpaceOutlineMaterial, 0);
                Blitter.BlitCameraTexture(cmd, temporaryBuffer, renderingData.cameraData.renderer.cameraColorTargetHandle);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Release(){
            CoreUtils.Destroy(screenSpaceOutlineMaterial);
            CoreUtils.Destroy(normalsMaterial);
            normals?.Release();
            temporaryBuffer?.Release();
        }
    }

    [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;
    [SerializeField] private LayerMask outlinesLayerMask;
    
    [SerializeField] private OutlinePassSettings outlineSettings = new OutlinePassSettings();

    private OutlinePass2 screenSpaceOutlinePass;
    
    public override void Create() {
        if (renderPassEvent < RenderPassEvent.BeforeRenderingPrePasses)
            renderPassEvent = RenderPassEvent.BeforeRenderingPrePasses;

        screenSpaceOutlinePass = new OutlinePass2(renderPassEvent, outlinesLayerMask, outlineSettings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(screenSpaceOutlinePass);
    }

    protected override void Dispose(bool disposing){
        if (disposing)
        {
            screenSpaceOutlinePass?.Release();
        }
    }
}