// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "VTP/SpeedTree/Branch" {
	Properties {
		
		
		[Header(General)]
		[Toggle] _showGeneral("Show General", Float) = 0
		[Toggle] _showRed("Show Red", Float) = 0
		[Toggle] _showGreen("Show Green", Float) = 0
		[Toggle] _showBlue("Show Blue", Float) = 0
		[Toggle] _showTesselation("Show Tesselation", Float) = 0

		[Toggle(_HEIGHTBASED_BLENDING)] _Heightbased_Blending("Heightbased Blending", Float) = 0



		[Space(10)]


		[Space(15)]
		
		[Header(Settings for first texture (red channel))]
		[Space(3)]
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_AlbedoColor ("Albedo Color", COLOR) = (1,1,1)
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_NormalScale ("Normal Strength", Range(0,2)) = 1
		_CombinedMap ("Combined Map", 2D) = "white" {}
		_Occlusion ("Occlusion Strength", Range(0,1)) = 1
		_Metallic ("Metallic Strength", Range(0,1)) = 0
		_Smoothness ("Smoothness Strength", Range(0,1)) = 0.2
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
		_MainTex2 ("Albedo (RGB)", 2D) = "white" {}
		_AlbedoColor2 ("Albedo Color", COLOR) = (1,1,1)
		[Normal] _BumpMap2 ("Normal Map", 2D) = "bump" {}
		_NormalScale2 ("Normal Strength", Range(0,2)) = 1
		_CombinedMap2 ("Combined Map", 2D) = "white" {}
		_Occlusion2 ("Occlusion Strength", Range(0,1)) = 1
		_Metallic2 ("Metallic Strength", Range(0,1)) = 0
		_Smoothness2 ("Smoothness Strength", Range(0,1)) = 0.2
		//_Emission2 ("Emission", Range(0,1)) = 0
		_EmissionColor2 ("Emission Color", COLOR) = (0,0,0)
		_BaseHeight2 ("Base Height", Range(-2,2)) = 0
		_HeightmapFactor2 ("Height Map Factor", Range(0,1)) = 0.5
		_BlendSmooth2 ("Blend Smooth", Range(0.001,1)) = 0.5
		_HeightBasedTransparency2 ("Heightbased transparency", Range(0,1)) = 0.5 

	
		_TilingU2 ("Scale U", Float) = 1
		_TilingV2 ("Scale V", Float) = 1
		
		

		[Space(10)]
		
		[Header(Settings for third texture (blue channel))]
		[Space(3)]
		_MainTex3 ("Albedo (RGB)", 2D) = "white" {}
		_AlbedoColor3 ("Albedo Color", COLOR) = (1,1,1)
		[Normal] _BumpMap3 ("Normal Map", 2D) = "bump" {}
		_NormalScale3 ("Normal Strength", Range(0,2)) = 1
		_CombinedMap3 ("Combined Map", 2D) = "white" {}
		_Occlusion3 ("Occlusion Strength", Range(0,1)) = 1
		_Metallic3 ("Metallic Strength", Range(0,1)) = 0
		_Smoothness3 ("Smoothness Strength", Range(0,1)) = 0.2
		//_Emission3 ("Emission", Range(0,1)) = 0
		_EmissionColor3 ("Emission Color", COLOR) = (0,0,0)
		_BaseHeight3 ("Base Height", Range(-2,2)) = 0
		_HeightmapFactor3 ("Height Map Factor", Range(0,1)) = 0.5
		_BlendSmooth3 ("Blend Smooth", Range(0.001,1)) = 0.5
		_HeightBasedTransparency3 ("Heightbased transparency", Range(0,1)) = 0.5 


		
		_TilingU3 ("Scale U", Float) = 1
		_TilingV3 ("Scale V", Float) = 1


		[MaterialEnum(None,0,Fastest,1,Fast,2,Better,3,Best,4,Palm,5)] _WindQuality ("Wind Quality", Range(0,5)) = 0
	}
		
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard keepalpha fullforwardshadows addshadow vertex:disp
		// Use shader model 3.0 target, to get nicer looking lighting

		#pragma multi_compile __ LOD_FADE_PERCENTAGE LOD_FADE_CROSSFADE
		#pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_FROND
		#pragma shader_feature _TESSELATIONMODE_DISTANCE _TESSELATIONMODE_EDGE _TESSELATIONMODE_FIXED
		#pragma shader_feature __ _HEIGHTBASED_BLENDING


		#define GEOM_TYPE_BRANCH
		#define ENABLE_WIND
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



		sampler2D _MainTex;
		fixed3 _AlbedoColor;
		sampler2D _BumpMap;
		float _NormalScale;
		sampler2D _CombinedMap;
		float _Occlusion;	
		float _Metallic;
		float _Smoothness;
		//float _Emission;
		fixed3 _EmissionColor;


		sampler2D _DetailMask;
		sampler2D _DetailTex;
		sampler2D _DetailBumpMap;
		float _DetailScaleU;
		float _DetailScaleV;
		float _DetailNormalScale;


		sampler2D _MainTex2;
		fixed3 _AlbedoColor2;
		sampler2D _BumpMap2;
		float _NormalScale2;
		sampler2D _CombinedMap2;
		float _Occlusion2;	
		float _Metallic2;
		float _Smoothness2;
		//float _Emission2;
		fixed3 _EmissionColor2;
		float _BaseHeight2;
		float _HeightmapFactor2;
		float _BlendSmooth2;
		float _HeightBasedTransparency2;





		sampler2D _MainTex3;
		fixed3 _AlbedoColor3;
		sampler2D _BumpMap3;
		float _NormalScale3;
		sampler2D _CombinedMap3;
		float _Occlusion3;	
		float _Metallic3;
		float _Smoothness3;		
		//float _Emission3;
		fixed3 _EmissionColor3;
		float _BaseHeight3;
		float _HeightmapFactor3;
		float _BlendSmooth3;
		float _HeightBasedTransparency3;

		



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





        void disp (inout appdata_full v)
        {


            v.vertex.xyz = calculateWindAffectionsBranch(v, unity_LODFade.x);

            //UNITY_TRANSFER_DITHER_CROSSFADE(o, v.vertex)


        }



		void surf (Input IN, inout SurfaceOutputStandard o) {

		

			fixed4 colorFactor = IN.color;
			float2 tex_Red = IN.uv_MainTex * float2(_TilingU,_TilingV);;
			float2 tex_Green = IN.uv_MainTex * float2(_TilingU2,_TilingV2);;
			float2 tex_Blue = IN.uv_MainTex * float2(_TilingU3,_TilingV3);;


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
	CustomEditor "ShaderEditor_SpeedTree_Branch"
}

