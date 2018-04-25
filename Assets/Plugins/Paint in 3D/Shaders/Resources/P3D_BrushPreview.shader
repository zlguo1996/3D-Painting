Shader "Hidden/P3D_BrushPreview"
{
	Properties
	{
		_Shape("Shape", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Tiling("Tiling", Vector) = (1, 1, 0, 0)
		_Offset("Offset", Vector) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags
		{
			"Queue"           = "Overlay+11"
			"RenderType"      = "Transparent"
			"IgnoreProjector" = "True"
		}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			Lighting Off
			ZWrite Off
			Offset -1, -1
			
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				
				sampler2D _Shape;
				float4x4  _WorldMatrix;
				float4x4  _PaintMatrix;
				float2    _CanvasResolution;
				float2    _Tiling;
				float2    _Offset;
				float4    _Color;
				
				struct a2v
				{
					float4 vertex    : POSITION;
					float2 texcoord0 : TEXCOORD0;
				};
				
				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float2 texcoord0 : TEXCOORD0;
					float3 texcoord1 : TEXCOORD1;
				};
				
				struct f2g
				{
					float4 color : COLOR;
				};
				
				float Sample(float2 uv)
				{
					float2 clipUV = abs(uv - 0.5f);
					
					if (max(clipUV.x, clipUV.y) > 0.5f)
					{
						return 0.0f;
					}
					
					return tex2D(_Shape, uv).a;
				}
				
				void Vert(a2v i, out v2f o)
				{
					float4x4 wvp = mul(UNITY_MATRIX_VP, _WorldMatrix);
					
					o.vertex    = mul(wvp, i.vertex);
					o.texcoord0 = i.texcoord0 * _Tiling + _Offset;
				}
				
				void Frag(v2f i, out f2g o)
				{
					// Repeat texture and put in pixel space
					float2 tiledPixel = frac(i.texcoord0) * _CanvasResolution;

					// Find the 0..1 position across the current texel
					float2 tiledFrac = abs(frac(tiledPixel) - 0.5f);

					// Snap to center of pixel
					float2 centerPixel = floor(tiledPixel) + 0.5f;

					// Transform pixel space position to shape space 0..1
					float2 shapeCoord = mul((float3x3)_PaintMatrix, float3(centerPixel, 1.0f));

					// Base color
					o.color = _Color;

					// Are we within 90% of the texel?
					if (max(tiledFrac.x, tiledFrac.y) < 0.45f)
					{
						o.color.a *= Sample(shapeCoord);
					}
					else
					{
						o.color.a = 0.0f;
					}
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader