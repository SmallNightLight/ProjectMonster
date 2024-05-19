#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION

//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/AmbientOcclusion.hlsl"

#ifndef AMBIENTOCCLUSION_INCLUDED
#define AMBIENTOCCLUSION_INCLUDED

void AmbientOcclusion_float(float2 screenUV, out float indirectAmbientOcclusion, out float directAmbientOcclusion)
{
	#if defined(SHADERGRAPH_PREVIEW)
	indirectAmbientOcclusion = 1.0;
	directAmbientOcclusion = 1.0;
	#else
	AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(screenUV);
	indirectAmbientOcclusion = aoFactor.indirectAmbientOcclusion;
	directAmbientOcclusion = aoFactor.directAmbientOcclusion;
	#endif
}

#endif