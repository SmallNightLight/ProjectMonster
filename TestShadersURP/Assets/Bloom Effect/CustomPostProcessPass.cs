using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class CustomPostProcessPass : ScriptableRenderPass
{
    private Material _bloomMaterial;
    public Material _compositeMaterial;

    private RTHandle _cameraColorTarget;
    private RTHandle _cameraDepthTarget;

    private RTHandle _destination;

    private RenderTextureDescriptor _descriptor;
    private BendayBloomEffect _bloomEffect;

    private const int _maxPyramidSize = 16;
    private int[] _bloomMipUp;
    private int[] _bloomMipDown;
    private RTHandle[] _mbloomMipUp;
    private RTHandle[] _mbloomMipDown;
    private GraphicsFormat _hdrFormat;

    public CustomPostProcessPass(Material bloomMaterial, Material compositeMaterial)
    {
        _bloomMaterial = bloomMaterial;
        _compositeMaterial = compositeMaterial;

        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        _bloomMipUp = new int[_maxPyramidSize];
        _bloomMipDown = new int[_maxPyramidSize];
        _mbloomMipUp = new RTHandle[_maxPyramidSize];
        _mbloomMipDown = new RTHandle[_maxPyramidSize];

        for(int i = 0; i < _maxPyramidSize; i++)
        {
            _bloomMipUp[i] = Shader.PropertyToID("_BloomMipUp" + i);
            _bloomMipDown[i] = Shader.PropertyToID("_BloomMipDown" + i);

            _mbloomMipUp[i] = RTHandles.Alloc(_bloomMipUp[i], name: "_BloomMipUp" + i);
            _mbloomMipDown[i] = RTHandles.Alloc(_bloomMipDown[i], name: "_BloomMipDown" + i);
        }

        const FormatUsage usage = FormatUsage.Linear | FormatUsage.Render;

        if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, usage))
        {
            _hdrFormat = GraphicsFormat.B10G11R11_UFloatPack32;
        }
        else
        {
            _hdrFormat = QualitySettings.activeColorSpace == ColorSpace.Linear ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm;
        }
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Preview)
            return;

        VolumeStack stack = VolumeManager.instance.stack;
        _bloomEffect = stack.GetComponent<BendayBloomEffect>();

        CommandBuffer cmd = CommandBufferPool.Get();

        using (new ProfilingScope(cmd, new ProfilingSampler("Bloom Effect")))
        {
            SetupBloom(cmd, _cameraColorTarget);

            _compositeMaterial.SetFloat("_Cutoff", _bloomEffect.DotsCutoff.value);
            _compositeMaterial.SetInt("_Density", _bloomEffect.DotsDensity.value);
            _compositeMaterial.SetVector("_Direction", _bloomEffect.ScrollDirection.value);
            _compositeMaterial.SetFloat("_Intensity", _bloomEffect.Intensity.value);
            _compositeMaterial.SetFloat("_SizeThreshold", _bloomEffect.SizeThreshold.value);
            _compositeMaterial.mainTexture = _mbloomMipUp[0];

            //Texture t2 = new Texture(1, 1);
            //Graphics.CopyTexture(_mbloomMipUp[0], t2);
            _compositeMaterial.SetTexture("_SceneTexture", _mbloomMipUp[0]);

            //Blitter.BlitTexture(cmd, _cameraColorTarget, Vector2.one, _compositeMaterial, 0);

            //Blitter.BlitTexture(cmd, _cameraColorTarget, new Vector4(1, 1, 1, 0), _compositeMaterial, 0);

            cmd.Blit(_cameraColorTarget, _destination, _compositeMaterial);
            cmd.Blit(_destination, _cameraColorTarget);
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }

    private void SetupBloom(CommandBuffer cmd, RTHandle source)
    {
        //Start at half-res
        int downres = 1;
        int tw = _descriptor.width >> downres;
        int th = _descriptor.height >> downres;

        //Determine the iteration count
        int maxSize = Mathf.Max(tw, th);
        int iterations = Mathf.FloorToInt(Mathf.Log(maxSize, 2f) - 1);
        int mipCount = Mathf.Clamp(iterations, 1, _bloomEffect.MaxIterations.value);

        //Pre-filtering parameters
        float clamp = _bloomEffect.Clamp.value;
        float threshold = Mathf.GammaToLinearSpace(_bloomEffect.Threshold.value);
        float thresholdKnee = threshold * 0.5f;

        //Material setup
        float scatter = Mathf.Lerp(0.05f, 0.95f, _bloomEffect.Scatter.value);
        var bloomMaterial = _bloomMaterial;

        bloomMaterial.SetVector("_Params", new Vector4(scatter, clamp, threshold, thresholdKnee));

        //Prefilter
        var desc = GetCompatibleDescriptor(tw, th, _hdrFormat);

        for(int i = 0; i < mipCount; i++)
        {
            RenderingUtils.ReAllocateIfNeeded(ref _mbloomMipUp[i], desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: _mbloomMipUp[i].name);
            RenderingUtils.ReAllocateIfNeeded(ref _mbloomMipDown[i], desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: _mbloomMipDown[i].name);

            desc.width = Mathf.Max(1, desc.width >> 1);
            desc.height = Mathf.Max(1, desc.height >> 1);
        }

        Blitter.BlitCameraTexture(cmd, source, _mbloomMipDown[0], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, _bloomMaterial, 0);

        //Downsample - gaussion pyramid
        var lastDown = _mbloomMipDown[0];

        for(int i = 1; i < mipCount; i++)
        {
            Blitter.BlitCameraTexture(cmd, lastDown, _mbloomMipUp[i], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, _bloomMaterial, 1);
            Blitter.BlitCameraTexture(cmd, _mbloomMipUp[i], _mbloomMipDown[i], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, _bloomMaterial, 2);

            lastDown = _mbloomMipDown[i];
        }

        //Upsample

        for (int i = mipCount - 2; i >= 0; i--)
        {
            var lowMip = (i == mipCount - 2) ? _mbloomMipDown[i + 1] : _mbloomMipUp[i + 1];
            var highMip = _mbloomMipDown[i];
            var dst = _mbloomMipUp[i];

            cmd.SetGlobalTexture("_SourceTexLowMip", lowMip);
            Blitter.BlitCameraTexture(cmd, highMip, dst, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, _bloomMaterial, 3);
        }

        cmd.SetGlobalTexture("_Bloom_Texture", _mbloomMipUp[0]);
        cmd.SetGlobalFloat("_BloomIntensity", _bloomEffect.Intensity.value);
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        _descriptor = renderingData.cameraData.cameraTargetDescriptor;
    }

    RenderTextureDescriptor GetCompatibleDescriptor() => GetCompatibleDescriptor(_descriptor.width, _descriptor.height, _descriptor.graphicsFormat);

    RenderTextureDescriptor GetCompatibleDescriptor(int width, int height, GraphicsFormat format, DepthBits depthBufferBits = DepthBits.None) => GetCompatibleDescriptor(_descriptor, width, height, format, depthBufferBits);

    public static RenderTextureDescriptor GetCompatibleDescriptor(RenderTextureDescriptor descriptor, int width, int height, GraphicsFormat format, DepthBits depthBufferBits = DepthBits.None)
    {
        descriptor.depthBufferBits = (int)depthBufferBits;
        descriptor.msaaSamples = 1;
        descriptor.width = width;
        descriptor.height = height;
        descriptor.graphicsFormat = format;
        return descriptor;
    }

    public void SetTarget(RTHandle cameraColorTargetHandle, RTHandle cameraDepthHandle)
    {
        _cameraColorTarget = _destination = cameraColorTargetHandle;
        _cameraDepthTarget = cameraDepthHandle;

        ConfigureTarget(_cameraColorTarget);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        _cameraColorTarget = null;
        _cameraDepthTarget = null;
        _destination = null;
    }
}