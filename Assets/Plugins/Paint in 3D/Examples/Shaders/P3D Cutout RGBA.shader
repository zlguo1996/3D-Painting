// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Paint in 3D/Cutout RGBA"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_NoiseTex("Noise Tex", 2D) = "white" {}
		_NoiseStrength("Noise Strength", float) = 0.1
		_NoiseScale("Noise Scale", float) = 5.0
		_CutOff("Cut Off", float) = 0.1
		_ColorA("Color A", Color) = (1, 1, 0, 1)
		_ColorB("Color B", Color) = (0, 1, 0, 1)
		_ColorC("Color C", Color) = (0, 1, 1, 1)
		_ColorD("Color D", Color) = (0, 0, 1, 1)
	}
	SubShader
	{
		Tags
		{
			"IgnoreProjector" = "True"
		}
		Pass
		{
			Lighting Off
			ZWrite On
			Offset -1, -1
			
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				
				sampler2D _MainTex;
				sampler2D _NoiseTex;
				float     _CutOff;
				float     _NoiseStrength;
				float     _NoiseScale;
				float4    _ColorA;
				float4    _ColorB;
				float4    _ColorC;
				float4    _ColorD;
				
				struct a2v
				{
					float4 vertex    : POSITION;
					float2 texcoord0 : TEXCOORD0;
				};
				
				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float2 texcoord0 : TEXCOORD0;
				};
				
				struct f2g
				{
					float4 color : COLOR;
				};
				
				void Vert(a2v i, out v2f o)
				{
					o.vertex    = UnityObjectToClipPos(i.vertex);
					o.texcoord0 = i.texcoord0;
				}
				
				void Frag(v2f i, out f2g o)
				{
					float4 mainTex  = tex2D(_MainTex, i.texcoord0);
					float4 noiseTex = tex2D(_NoiseTex, i.texcoord0 * _NoiseScale);
					
					mainTex = saturate(mainTex - noiseTex * _NoiseStrength);

					o.color = 0.0f;
					
					if (mainTex.r >= o.color.a)
					{
						o.color = _ColorA; o.color.a = mainTex.r;
					}

					if (mainTex.g >= o.color.a)
					{
						o.color = _ColorB; o.color.a = mainTex.g;
					}

					if (mainTex.b >= o.color.a)
					{
						o.color = _ColorC; o.color.a = mainTex.b;
					}

					if (mainTex.a >= o.color.a)
					{
						o.color = _ColorD; o.color.a = mainTex.a;
					}

					if (o.color.a < _CutOff) discard;
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader