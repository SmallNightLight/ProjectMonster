#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION

#ifndef OUTLINES_INCLUDED
#define OUTLINES_INCLUDED

void GetCrossSampleUVs_float(float4 UV, float2 texelSize, float4 offsetMultiplier,
	out float2 UVOriginal, out float2 UVTopRight, out float2 UVBottomLeft, out float2 UVTopLeft, out float2 UVBottomRight)
{
	UVOriginal = UV;
	UVTopRight = UV.xy + float2(texelSize.x, texelSize.y) * offsetMultiplier[0];
	UVBottomLeft = UV.xy - float2(texelSize.x, texelSize.y) * offsetMultiplier[1];
	UVTopLeft = UV.xy + float2(-texelSize.x * offsetMultiplier[2], texelSize.y * offsetMultiplier[2]);
	UVBottomRight =  UV.xy + float2(texelSize.x * offsetMultiplier[3], -texelSize.y * offsetMultiplier[3]);
}

#endif