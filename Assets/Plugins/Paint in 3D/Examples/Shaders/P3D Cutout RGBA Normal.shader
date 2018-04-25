Shader "Paint in 3D/Cutout RGBA Normal"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_BumpTex("Bump Tex", 2D) = "normal" {}
		_NoiseTex("Noise Tex", 2D) = "white" {}
		_NoiseStrength("Noise Strength", float) = 0.1
		_NoiseScale("Noise Scale", float) = 5.0
		_CutOff("Cut Off", float) = 0.1
		_ColorA("Color A", Color) = (1, 1, 0, 1)
		_ColorB("Color B", Color) = (0, 1, 0, 1)
		_ColorC("Color C", Color) = (0, 1, 1, 1)
		_ColorD("Color D", Color) = (0, 0, 1, 1)

		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+1" "ForceNoShadowCasting" = "True" }
		LOD 200

		CGPROGRAM
		#pragma surface Surf Standard fullforwardshadows

		//#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpTex;
		sampler2D _NoiseTex;
		float     _CutOff;
		float     _NoiseStrength;
		float     _NoiseScale;
		float3    _ColorA;
		float3    _ColorB;
		float3    _ColorC;
		float3    _ColorD;

		struct Input
		{
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;

		void Surf(Input IN, inout SurfaceOutputStandard o)
		{
			float4 mainTex  = tex2D(_MainTex, IN.uv_MainTex);
			float4 noiseTex = tex2D(_NoiseTex, IN.uv_MainTex * _NoiseScale);
			float3 color    = 0.0f;
			float  alpha    = 0.0f;

			mainTex = saturate(mainTex - noiseTex * _NoiseStrength);

			if (mainTex.r >= alpha)
			{
				color = _ColorA; alpha = mainTex.r;
			}

			if (mainTex.g >= alpha)
			{
				color = _ColorB; alpha = mainTex.g;
			}

			if (mainTex.b >= alpha)
			{
				color = _ColorC; alpha = mainTex.b;
			}

			if (mainTex.a >= alpha)
			{
				color = _ColorD; alpha = mainTex.a;
			}

			if (alpha < _CutOff) discard;

			o.Albedo     = color;
			o.Normal     = UnpackNormal(tex2D(_BumpTex, IN.uv_MainTex));
			o.Alpha      = alpha;
			o.Metallic   = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}