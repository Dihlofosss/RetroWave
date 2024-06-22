//UNITY_SHADER_NO_UPGRADE
#ifndef BLUR_INCLUDED
#define BLUR_INCLUDED

void Blur_float(UnityTexture2D tex, float2 UV, float steps, float blurScale, out float4 Out)
{
	float4 Texture;
	float count = 0;

	for (float i = -1 * steps; i <= steps; i++)
	{
		for (float j = - 1 * steps; j <= steps; j++)
		{
			float2 offset = { i * blurScale, j * blurScale };
			Texture += SAMPLE_TEXTURE2D(tex.tex, tex.samplerstate, tex.GetTransformedUV(UV + offset));
			count++;
		}
	}
	Out = Texture / count;
}
#endif