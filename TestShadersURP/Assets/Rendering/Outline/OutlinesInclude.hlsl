#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION

#ifndef OUTLINES_INCLUDED
#define OUTLINES_INCLUDED

void GetCrossSampleUVs_float(float4 UV, float2 texelSize, float4 multiplier,
	out float2 UVTopRight, out float2 UVBottomLeft, out float2 UVTopLeft, out float2 UVBottomRight)
{
	UVTopRight = UV.xy + float2(texelSize.x, texelSize.y) * multiplier[0];
	UVBottomLeft = UV.xy - float2(texelSize.x, texelSize.y) * multiplier[1];
	UVTopLeft = UV.xy + float2(-texelSize.x * multiplier[2], texelSize.y * multiplier[2]);
	UVBottomRight =  UV.xy + float2(texelSize.x * multiplier[3], -texelSize.y * multiplier[3]);
}

#endif