// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "VTP/SpeedTree/Branch (Tessellated)" {
	Properties {
		
		
		[Header(General)]
		[Toggle] _showGeneral("Show General", Float) = 0
		[Toggle] _showRed("Show Red", Float) = 0
		[Toggle] _showGreen("Show Green", Float) = 0
		[Toggle] _showBlue("Show Blue", Float) = 0
		[Toggle] _showTesselation("Show Tesselation", Float) = 0


		[Space(3)]
		[KeywordEnum(Distance, Edge, Fixed)] _TesselationMode ("Tesselation mode", Float) = 0
		_TesselationAmount ("Tesselation Amount", Range(1,64)) = 16
		_TesselationMin ("Min Distance", Float) = 15
		_TesselationMax ("Max Distance", Float) = 30
		_EdgeLength ("Max Distance", Range(2,50)) = 15
		_TessFix ("Fixed Amount", Range(2,32)) = 15
		[Toggle(_HEIGHTBASED_BLENDING)] _Heightbased_Blending("Heightbased Blending", Float) = 0

		[Toggle(_RECALC_MESH)] _RecalcNormals("Heightbased Blending", Float) = 0

		[Space(3)]
		[Toggle] _showVertexColor("Visualize Vertex Color", Float) = 0
		[Toggle] _showAlpha("Visualize Alpha Vertex Color", Float) = 0
		[Toggle] _showVertexFlow("Visualize Flow Data", Float) = 0

		[Toggle(_HEIGHTBASED_BLENDING)] _useHeightBasedBlending ("Use Heightbased Blending", Float) = 0
		[Toggle(_WETNESS)] _useWetness("Use Wetness", Float) = 0
		_wetnessEdgeBlend ("Wetness Edge Blend", Range(0,1)) = 0.3
		
		
		[Toggle(_FLOW)] _useFlow("Use Flow", Float) = 0
		[Toggle(_FLOW_DRIFT)] _useDrift ("Use Height drift", Float) = 1
		[Toggle(_FLOW_NORMALS)] _flowNormals ("Only flow Normals", Float) = 0
		
		[Space(3)]
		[Toggle(_PARALLAX)] _useParallax ("Use Parallax", Float) = 1
		_ParallaxInterpolation ("Parallax Interpolation", Int) = 15


		[Space(10)]

		[Header(Depth Bias)]
		[Space(3)]
		[Toggle(_DEPTHBIAS)] _useDepthBias ("Use Depth-Bias", Float) = 0
		_DepthBias ("Depth Bias Distance", Range(0.001,100)) = 50
		_DepthBiasPower ("Depth Bias Power", Range(1,10)) = 2
		_DepthBiasThreshold ("Depth Bias Threshold", Range(0,0.01)) = 0

		[Space(10)]


		[Space(15)]
		
		[Header(Settings for first texture (red channel))]
		[Space(3)]
		_TesselationStrength ("Tesselation Strength", Range(-2,2)) = 0
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_AlbedoColor ("Albedo Color", COLOR) = (1,1,1)
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_NormalScale ("Normal Strength", Range(0,2)) = 1
		_CombinedMap ("Combined Map", 2D) = "white" {}
		_Parallax ("Parallax Strength", Range(-0.05,0.05)) = -0.01
		_Occlusion ("Occlusion Strength", Range(0,1)) = 1
		_Metallic ("Metallic Strength", Range(0,1)) = 0
		_Smoothness ("Smoothness Strength", Range(0,1)) = 1
		//_Emission ("Emission", Range(0,1)) = 0
		_EmissionColor ("Emission Color", COLOR) = (0,0,0)
		
		_wetnessAlbedoModifier ("Wetness Color", COLOR) = (0,0,0)
		_wetnessSmoothnessModifier ("Wetness Smoothness", Range(0,1)) = 0.9
		_wetnessNormalModifier ("Wetness Normal", Range(0,1)) = 0.75
		
		_DetailMask ("Detail Mask (BW)", 2D) = "white" {}
		_DetailTex ("Detail Albedo (RGB)", 2D) = "white" {}
		[Normal] _DetailBumpMap ("Normal Map", 2D) = "bump" {}
		_DetailNormalScale ("Detail Normal Scale", Range(0,2)) = 1
		_DetailScaleU ("Detail Scale U", Float) = 1
		_DetailScaleV ("Detail Scale V", Float) = 1


		_TilingU ("Scale U", Float) = 1
		_TilingV ("Scale V", Float) = 1

		[Space(10)]
		
		[Header(Settings for second texture (green channel))]
		[Space(3)]
		_TesselationStrength2 ("Tesselation Strength", Range(-2,2)) = 0
		_MainTex2 ("Albedo (RGB)", 2D) = "white" {}
		_AlbedoColor2 ("Albedo Color", COLOR) = (1,1,1)
		[Normal] _BumpMap2 ("Normal Map", 2D) = "bump" {}
		_NormalScale2 ("Normal Strength", Range(0,2)) = 1
		_CombinedMap2 ("Combined Map", 2D) = "white" {}
		_Parallax2 ("Parallax Strength", Range(-0.05,0.05)) = -0.01
		[Toggle] _useP1_2("Use Heightmap of base layer", Float) = 1
		_Occlusion2 ("Occlusion Strength", Range(0,1)) = 1
		_Metallic2 ("Metallic Strength", Range(0,1)) = 0
		_Smoothness2 ("Smoothness Strength", Range(0,1)) = 1
		//_Emission2 ("Emission", Range(0,1)) = 0
		_EmissionColor2 ("Emission Color", COLOR) = (0,0,0)
		_BaseHeight2 ("Base Height", Range(-2,2)) = 0
		_HeightmapFactor2 ("Height Map Factor", Range(0,1)) = 0.5
		_BlendSmooth2 ("Blend Smooth", Range(0.001,1)) = 0.5
		_HeightBasedTransparency2 ("Heightbased transparency", Range(0,1)) = 0.5 
		
		_FlowSpeed2 ("Flow Speed", Float) = 0.25
		
		_wetnessAlbedoModifier2 ("Wetness Color", COLOR) = (0,0,0)
		_wetnessSmoothnessModifier2 ("Wetness Smoothness", Range(0,1)) = 0.9
		_wetnessNormalModifier2 ("Wetness Normal", Range(0,1)) = 0.75
	
		_TilingU2 ("Scale U", Float) = 1
		_TilingV2 ("Scale V", Float) = 1
		
		

		[Space(10)]
		
		[Header(Settings for third texture (blue channel))]
		[Space(3)]
		_TesselationStrength3 ("Tesselation Strength", Range(-2,2)) = 0
		_MainTex3 ("Albedo (RGB)", 2D) = "white" {}
		_AlbedoColor3 ("Albedo Color", COLOR) = (1,1,1)
		[Normal] _BumpMap3 ("Normal Map", 2D) = "bump" {}
		_NormalScale3 ("Normal Strength", Range(0,2)) = 1
		_CombinedMap3 ("Combined Map", 2D) = "white" {}
		_Parallax3 ("Parallax Strength", Range(-0.05,0.05)) = -0.01
		[Toggle] _useP1_3("Use Heightmap of base layer", Float) = 1
		_Occlusion3 ("Occlusion Strength", Range(0,1)) = 1
		_Metallic3 ("Metallic Strength", Range(0,1)) = 0
		_Smoothness3 ("Smoothness Strength", Range(0,1)) = 1
		//_Emission3 ("Emission", Range(0,1)) = 0
		_EmissionColor3 ("Emission Color", COLOR) = (0,0,0)
		_BaseHeight3 ("Base Height", Range(-2,2)) = 0
		_HeightmapFactor3 ("Height Map Factor", Range(0,1)) = 0.5
		_BlendSmooth3 ("Blend Smooth", Range(0.001,1)) = 0.5
		_HeightBasedTransparency3 ("Heightbased transparency", Range(0,1)) = 0.5 
		_FlowSpeed3 ("Flow Speed", Float) = 0.25
		
		_wetnessAlbedoModifier3 ("Wetness Color", COLOR) = (0,0,0)
		_wetnessSmoothnessModifier3 ("Wetness Smoothness", Range(0,1)) = 0.9
		_wetnessNormalModifier3 ("Wetness Normal", Range(0,1)) = 0.75
		
		_TilingU3 ("Scale U", Float) = 1
		_TilingV3 ("Scale V", Float) = 1


		[MaterialEnum(None,0,Fastest,1,Fast,2,Better,3,Best,4,Palm,5)] _WindQuality ("Wind Quality", Range(0,5)) = 0
	}
		
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard keepalpha fullforwardshadows addshadow vertex:disp tessellate:tess
		// Use shader model 3.0 target, to get nicer looking lighting

		#pragma multi_compile __ LOD_FADE_PERCENTAGE LOD_FADE_CROSSFADE
		#pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_FROND
		#pragma shader_feature _TESSELATIONMODE_DISTANCE _TESSELATIONMODE_EDGE _TESSELATIONMODE_FIXED
		#pragma shader_feature __ _HEIGHTBASED_BLENDING

		#define ENABLE_WIND
		#define TESSELATION 1
		#define METALLIC 1
		#define TARGET30 1
		#pragma target 3.0
		#include "SpeedTreeWind.cginc"
		#include "Tessellation.cginc"
		


    	float _TilingU;
    	float _TilingV;
    	float _TilingU2;
    	float _TilingV2;
    	float _TilingU3;
    	float _TilingV3;


		float _TesselationAmount;
		float _TesselationMin ;
		float _TesselationMax ;
		float _EdgeLength;
		float _TessFix;
		

		float _showVertexColor;
		float _showAlpha;
		float _showVertexFlow;

		float _useHeightBasedBlending;

		float _useParallax;
		int _ParallaxInterpolation;

		float _useDepthBias;
		float _DepthBias;
		float _DepthBiasPower;
		float _DepthBiasThreshold;

		float _useFlowMapGlobal;
		float _flowOnlyNormal;

		fixed _wetnessEdgeBlend;

		float _TesselationStrength;
		sampler2D _MainTex;
		fixed3 _AlbedoColor;
		sampler2D _BumpMap;
		float _NormalScale;
		sampler2D _CombinedMap;
		float _Parallax;
		float _Occlusion;	
		float _Metallic;
		float _Smoothness;
		//float _Emission;
		fixed3 _EmissionColor;
		
		fixed3 _wetnessAlbedoModifier;
		fixed _wetnessSmoothnessModifier;
		fixed _wetnessNormalModifier;

		sampler2D _DetailMask;
		sampler2D _DetailTex;
		sampler2D _DetailBumpMap;
		float _DetailScaleU;
		float _DetailScaleV;
		float _DetailNormalScale;


		float _TesselationStrength2;
		sampler2D _MainTex2;
		fixed3 _AlbedoColor2;
		sampler2D _BumpMap2;
		float _NormalScale2;
		sampler2D _CombinedMap2;
		float _Parallax2;
		float _useP1_2;
		float _Occlusion2;	
		float _Metallic2;
		float _Smoothness2;
		//float _Emission2;
		fixed3 _EmissionColor2;
		float _BaseHeight2;
		float _HeightmapFactor2;
		float _BlendSmooth2;
		float _HeightBasedTransparency2;
		float _useFlowMap2;
		float _FlowSpeed2;
		float _useDrift2;
		
		fixed3 _wetnessAlbedoModifier2;
		fixed _wetnessSmoothnessModifier2;
		fixed _wetnessNormalModifier2;
		
		sampler2D _DetailMask2;
		//sampler2D _DetailTex2;
		sampler2D _DetailBumpMap2;
		float _DetailScaleU2;
		float _DetailScaleV2;
		float _DetailNormalScale2;


		float _TesselationStrength3;
		sampler2D _MainTex3;
		fixed3 _AlbedoColor3;
		sampler2D _BumpMap3;
		float _NormalScale3;
		sampler2D _CombinedMap3;
		float _Parallax3;
		float _useP1_3;
		float _Occlusion3;	
		float _Metallic3;
		float _Smoothness3;		
		//float _Emission3;
		fixed3 _EmissionColor3;
		float _BaseHeight3;
		float _HeightmapFactor3;
		float _BlendSmooth3;
		float _HeightBasedTransparency3;
		float _useFlowMap3;
		float _FlowSpeed3;
		float _useDrift3;
		
		fixed3 _wetnessAlbedoModifier3;
		fixed _wetnessSmoothnessModifier3;
		fixed _wetnessNormalModifier3;
		
		
		sampler2D _DetailMask3;
		//sampler2D _DetailTex3;
		sampler2D _DetailBumpMap3;
		float _DetailScaleU3;
		float _DetailScaleV3;
		float _DetailNormalScale3;
		


		float4 _MainTex_TexelSize;
		float4 _MainTex2_TexelSize;
		float4 _MainTex3_TexelSize;


		struct Input {
			float2 uv_MainTex	: TEXCOORD0;
			float2 uv4_MainTex2 : TEXCOORD1;

            float2 secondUV		: TEXCOORD3;
            fixed4 color 		: COLOR;

            UNITY_DITHER_CROSSFADE_COORDS
       
       	};

       	struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float4 color : COLOR;
            float2 texcoord : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1;
            float2 texcoord2 : TEXCOORD2;
            float2 texcoord3 : TEXCOORD3;
        };


       

        #ifdef ENABLE_WIND

		#define WIND_QUALITY_NONE		0
		#define WIND_QUALITY_FASTEST	1
		#define WIND_QUALITY_FAST		2
		#define WIND_QUALITY_BETTER		3
		#define WIND_QUALITY_BEST		4
		#define WIND_QUALITY_PALM		5

		uniform half _WindQuality;
		uniform half _WindEnabled;

		#endif

		#include "SpeedTree_VTPUtil.cginc"





#if _TESSELATIONMODE_EDGE
        float4 tess (appdata_full v0, appdata_full v1, appdata_full v2) {
            return UnityEdgeLengthBasedTessCull (v0.vertex, v1.vertex, v2.vertex, _EdgeLength,1 );
        }
#endif

#if _TESSELATIONMODE_DISTANCE
        float4 tess (appdata_full v0, appdata_full v1, appdata_full v2) {
		    return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, _TesselationMin, _TesselationMax, _TesselationAmount);
		}
#endif

#if _TESSELATIONMODE_FIXED
		float4 tess() {
		    return _TessFix;
		}
#endif



        void disp (inout appdata_full v)
        {

        	

        	fixed4 colorFactor = v.color;
        	float2 tex_Red = v.texcoord.xy * float2(_TilingU,_TilingV);
			float2 tex_Green = v.texcoord.xy * float2(_TilingU2,_TilingV2);
			float2 tex_Blue = v.texcoord.xy * float2(_TilingU3,_TilingV3);

			#ifdef _HEIGHTBASED_BLENDING
				half3 heightValues = calculateColorFactorWithHeightReturn(colorFactor, float4(tex_Red,0,0), float4(tex_Green,0,0), float4(tex_Blue,0,0));
			#else
        		half3 heightValues = half3( tex2Dlod (_CombinedMap, float4(tex_Red,0,0)).r, tex2Dlod (_CombinedMap2, float4(tex_Green,0,0)).r, tex2Dlod (_CombinedMap3, float4(tex_Blue,0,0)).r);
			#endif
        	//half3 heightValues = calculateColorFactorWithHeightReturn(colorFactor, float4(tex_Red,0,0), float4(tex_Green,0,0), float4(tex_Blue,0,0), v.texcoord3.xy * 2.0 - 1.0 );
        	//half3 heightValues = half3( tex2Dlod (_CombinedMap, float4(tex_Red,0,0)).r, tex2Dlod (_CombinedMap2, float4(tex_Green,0,0)).r, tex2Dlod (_CombinedMap3, float4(tex_Blue,0,0)).r);
	


			//float3 wpos = mul(unity_ObjectToWorld,v.vertex).xyz;
			//float dist = distance (mul(unity_ObjectToWorld,v.vertex).xyz, _WorldSpaceCameraPos);
			float f = clamp(1.0 - (distance (mul(unity_ObjectToWorld,v.vertex).xyz, _WorldSpaceCameraPos) - _TesselationMin) / (_TesselationMax - _TesselationMin), 0, 1.0);

           // float d = (_TesselationStrength * colorFactor.r + _TesselationStrength2 * colorFactor.g + _TesselationStrength3 * colorFactor.b) * (heightValues.r * colorFactor.r + heightValues.g * colorFactor.g + heightValues.b * colorFactor.b);
            float d = lerp( lerp( _TesselationStrength, _TesselationStrength2, colorFactor.g), _TesselationStrength3, colorFactor.b) * lerp( lerp( heightValues.r, heightValues.g, colorFactor.g), heightValues.b, colorFactor.b);

            v.vertex.xyz = calculateWindAffectionsBranch(v, unity_LODFade.x);

            v.vertex.xyz += v.normal * d * f;

           // UNITY_TRANSFER_DITHER_CROSSFADE(o, v.vertex)



        }






		void surf (Input IN, inout SurfaceOutputStandard o) {

			fixed4 colorFactor = IN.color;
			float2 tex_Red = IN.uv_MainTex * float2(_TilingU,_TilingV);
			float2 tex_Green = IN.uv_MainTex * float2(_TilingU2,_TilingV2);
			float2 tex_Blue = IN.uv_MainTex * float2(_TilingU3,_TilingV3);

			#ifdef _HEIGHTBASED_BLENDING
				calculateColorFactor(colorFactor, tex_Red, tex_Green, tex_Blue);
			#endif

			o.Albedo = Albedo(tex_Red, tex_Green, tex_Blue, colorFactor);
			half2 metallicGloss = MetallicGloss(tex_Red, tex_Green, tex_Blue, colorFactor);	

			
			
			o.Metallic = metallicGloss.x;
			o.Smoothness = metallicGloss.y;
			       		
			o.Normal = NormalInTangentSpace(tex_Red, tex_Green, tex_Blue, colorFactor);
			
			o.Occlusion = Occlusion(tex_Red, tex_Green, tex_Blue, colorFactor);
			
			o.Emission = Emission(tex_Red, tex_Green, tex_Blue, colorFactor);


			//UNITY_APPLY_DITHER_CROSSFADE(IN)

		}
		ENDCG
	} 



	FallBack "Diffuse"
	CustomEditor "ShaderEditor_SpeedTree_Branch_Tesselation"
}

