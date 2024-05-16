using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class CustomPostProcessRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader _bloomShader;
    [SerializeField] private Shader _compositeShader;

    private Material _bloomMaterial;
    private Material _compositeMaterial;

    private CustomPostProcessPass _customPass;

    public override void Create()
    {
        _bloomMaterial = CoreUtils.CreateEngineMaterial(_bloomShader);
        _compositeMaterial = CoreUtils.CreateEngineMaterial(_compositeShader);

        _customPass = new CustomPostProcessPass(_bloomMaterial, _compositeMaterial);
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