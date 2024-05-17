#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION

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

void MyFunction_float(float2 A, float B, out float out1, out float out2)
{
	AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(A);
    out1 = aoFactor.indirectAmbientOcclusion;
	out2 = aoFactor.directAmbientOcclusion;
}
#endif