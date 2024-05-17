using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenuForRenderPipeline("Custom/Ben Day Bloom", typeof(UniversalRenderPipeline))]
public class BendayBloomEffect : VolumeComponent, IPostProcessComponent
{
    [Header("Bloom Settings")]
    public FloatParameter Threshold = new FloatParameter(0.9f, true);
    public FloatParameter Intensity = new FloatParameter(1, true);
    public ClampedFloatParameter Scatter = new ClampedFloatParameter(0.7f, 0, 1, true);
    public IntParameter Clamp = new IntParameter(65472, true);
    public ClampedIntParameter MaxIterations = new ClampedIntParameter(6, 0, 10);
    public NoInterpColorParameter Tint = new NoInterpColorParameter(Color.white);

    [Header("Benday")]
    public IntParameter DotsDensity = new IntParameter(10, true);
    public ClampedFloatParameter DotsCutoff = new ClampedFloatParameter(0.4f, 0, 1, true);
    public Vector2Parameter ScrollDirection = new Vector2Parameter(new Vector2());
    public FloatParameter SizeThreshold = new FloatParameter(1, false);

    public bool IsActive()
    {
        return true;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
}