// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "VTP/SpeedTree/Leaf"
{
	Properties
	{


		[Header(VTP Internal)]
		[Toggle] _showGeneral("Show General", Float) = 0
		[Toggle] _showRed("Show Red", Float) = 0
		[Toggle] _showGreen("Show Green", Float) = 0
		[Toggle] _showBlue("Show Blue", Float) = 0
		[Toggle] _showTranslucency("Show Translucency", Float) = 0


		[Header(General Settings)]
		[Header(Translucency)]
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.2
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 0.9
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.5
		_TransShadow("Shadow", Range( 0 , 1)) = 0.8


		[Header(Frist Channel)]
		[Header(General)]
		_MaskClipValue( "Mask Clip Value", Range( 0 , 1) ) = 0.33
		[Header(PBR Settings)]
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_NormalScale ("Normal Strength", Range(0,2)) = 1
		_Combined("Combined Map", 2D) = "white" {}
		_AlbedoColor("Color", Color) = (1,1,1,1)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Occlusion("Occlusion", Range( 0 , 1)) = 0
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 4



		[Header(Second Channel)]
		[Header(General)]
		[Header(PBR Settings)]
		_Albedo2("Albedo", 2D) = "white" {}
		_Normal2("Normal", 2D) = "bump" {}
		_NormalScale2 ("Normal Strength", Range(0,2)) = 1
		_Combined2("Combined Map", 2D) = "white" {}
		_AlbedoColor2("Color", Color) = (1,1,1,1)
		_Smoothness2("Smoothness", Range( 0 , 1)) = 0
		_Occlusion2("Occlusion", Range( 0 , 1)) = 0
		[Header(Translucency)]
		_Translucency2("Strength", Range( 0 , 50)) = 4




		[Header(Third Channel)]
		[Header(General)]
		[Header(PBR Settings)]
		_Albedo3("Albedo", 2D) = "white" {}
		_Normal3("Normal", 2D) = "bump" {}
		_NormalScale3 ("Normal Strength", Range(0,2)) = 1
		_Combined3("Combined Map", 2D) = "white" {}
		_AlbedoColor3("Color", Color) = (1,1,1,1)
		_Smoothness3("Smoothness", Range( 0 , 1)) = 0
		_Occlusion3("Occlusion", Range( 0 , 1)) = 0
		[Header(Translucency)]
		_Translucency3("Strength", Range( 0 , 50)) = 4
	


		_BlendColor_2("Blend Color (Layer 2)", Color) = (1,1,1,1)
		_BlendColor_3("Blend Color (Layer 3)", Color) = (1,1,1,1)

		[Header(General)]
		[MaterialEnum(None,0,Fastest,1,Fast,2,Better,3,Best,4,Palm,5)] _WindQuality ("Wind Quality", Range(0,5)) = 0
		[KeywordEnum(None, Fade, Hard, Cap)] _Height ("Height Layer", Float) = 0
		[Toggle(LOD_FADE_PERCENTAGE)] _Percentage ("Percentage?", Float) = 0
		[Toggle(LOD_FADE_CROSSFADE)] _Cross ("Cross?", Float) = 0

	}

	SubShader
	{
		Tags{ "Queue"="Geometry" "RenderType" = "Opaque" "DisableBatching"="LODFading"  }
		Cull Off
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "SpeedTreeWind.cginc"


		#pragma target 3.0
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows dithercrossfade vertex:vert 
		#pragma multi_compile __ LOD_FADE_PERCENTAGE LOD_FADE_CROSSFADE
		#pragma multi_compile _HEIGHT_NONE _HEIGHT_FADE _HEIGHT_HARD _HEIGHT_CAP
		#pragma multi_compile_instancing
		#pragma instancing_options assumeuniformscaling lodfade maxcount:50

		#define ENABLE_WIND
		#define GEOM_TYPE_LEAF


		struct Input
		{
			float2 uv_Albedo   : TEXCOORD0;
			float4 vertexColor : COLOR;

		};

		struct SurfaceOutputStandardCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			fixed3 Translucency;
		};


		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;



		uniform float _MaskClipValue;

		uniform sampler2D _Normal;
		uniform float _NormalScale;
		uniform sampler2D _Albedo;
		uniform sampler2D _Combined;
		uniform float4 _AlbedoColor;
		uniform float _Smoothness;
		uniform float _Occlusion;
		uniform half _Translucency;





		uniform sampler2D _Normal2;
		uniform float _NormalScale2;
		uniform sampler2D _Albedo2;
		uniform sampler2D _Combined2;
		uniform float4 _AlbedoColor2;
		uniform float _Smoothness2;
		uniform float _Occlusion2;
		uniform half _Translucency2;
	





		uniform sampler2D _Normal3;
		uniform float _NormalScale3;
		uniform sampler2D _Albedo3;
		uniform sampler2D _Combined3;
		uniform float4 _AlbedoColor3;
		uniform float _Smoothness3;
		uniform float _Occlusion3;
		uniform half _Translucency3;





		uniform float4 _BlendColor_2;
		uniform float4 _BlendColor_3;

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


		struct SpeedTreeVB 
		{
			float4 vertex		: POSITION;
			float4 tangent		: TANGENT;
			float3 normal		: NORMAL;
			float4 texcoord		: TEXCOORD0;
			float4 texcoord1	: TEXCOORD1;
			float4 texcoord2	: TEXCOORD2;
			float2 texcoord3	: TEXCOORD3;
			half4 color			: COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};



		/* Unity Built-In Speedtree */

		float3 calculateWindAffections(SpeedTreeVB v, float lodValue) {

			float3 finalPosition = v.vertex.xyz;

			#ifdef ENABLE_WIND
				half windQuality = _WindQuality * _WindEnabled;

				float3 rotatedWindVector, rotatedBranchAnchor;
				if (windQuality <= WIND_QUALITY_NONE)
				{
					rotatedWindVector = float3(0.0f, 0.0f, 0.0f);
					rotatedBranchAnchor = float3(0.0f, 0.0f, 0.0f);
				}
				else
				{
					// compute rotated wind parameters
					rotatedWindVector = normalize(mul(_ST_WindVector.xyz, (float3x3)unity_ObjectToWorld));
					rotatedBranchAnchor = normalize(mul(_ST_WindBranchAnchor.xyz, (float3x3)unity_ObjectToWorld)) * _ST_WindBranchAnchor.w;
				}
			#endif

			#if defined(GEOM_TYPE_BRANCH) || defined(GEOM_TYPE_FROND)

				// smooth LOD
				#ifdef LOD_FADE_PERCENTAGE
					finalPosition = lerp(finalPosition, v.texcoord1.xyz, lodValue);
				#endif

				// frond wind, if needed
				#if defined(ENABLE_WIND) && defined(GEOM_TYPE_FROND)
					if (windQuality == WIND_QUALITY_PALM)
						finalPosition = RippleFrond(finalPosition, v.normal, v.texcoord.x, v.texcoord.y, v.texcoord2.x, v.texcoord2.y, v.texcoord2.z);
				#endif

			#elif defined(GEOM_TYPE_LEAF)

				// remove anchor position
				finalPosition -= v.texcoord1.xyz;

				bool isFacingLeaf = v.color.a == 0;
				if (isFacingLeaf)
				{
					#ifdef LOD_FADE_PERCENTAGE
						finalPosition *= lerp(1.0, v.texcoord1.w, lodValue);
					#endif
					// face camera-facing leaf to camera
					float offsetLen = length(finalPosition);
					finalPosition = mul(finalPosition.xyz, (float3x3)UNITY_MATRIX_IT_MV); // inv(MV) * finalPosition
					finalPosition = normalize(finalPosition) * offsetLen; // make sure the offset vector is still scaled
				}
				else
				{
					#ifdef LOD_FADE_PERCENTAGE
						float3 lodPosition = float3(v.texcoord1.w, v.texcoord3.x, v.texcoord3.y);
						finalPosition = lerp(finalPosition, lodPosition, lodValue);
					#endif
				}

				#ifdef ENABLE_WIND
					// leaf wind
					if (windQuality > WIND_QUALITY_FASTEST && windQuality < WIND_QUALITY_PALM)
					{
						float leafWindTrigOffset = v.texcoord1.x + v.texcoord1.y;
						finalPosition = LeafWind(windQuality == WIND_QUALITY_BEST, v.texcoord2.w > 0.0, finalPosition, v.normal, v.texcoord2.x, float3(0,0,0), v.texcoord2.y, v.texcoord2.z, leafWindTrigOffset, rotatedWindVector);
					}
				#endif

				// move back out to anchor
				finalPosition += v.texcoord1.xyz;

			#endif

			#ifdef ENABLE_WIND
				float3 treePos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);

				#ifndef GEOM_TYPE_MESH
					if (windQuality >= WIND_QUALITY_BETTER)
					{
						// branch wind (applies to all 3D geometry)
						finalPosition = BranchWind(windQuality == WIND_QUALITY_PALM, finalPosition, treePos, float4(v.texcoord.zw, 0, 0), rotatedWindVector, rotatedBranchAnchor);
					}
				#endif

				if (windQuality > WIND_QUALITY_NONE)
				{
					// global wind
					finalPosition = GlobalWind(finalPosition, treePos, true, rotatedWindVector, _ST_WindGlobal.x);
				}
			#endif

			return finalPosition;



		}


	

		void vert( inout SpeedTreeVB v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );



			float minY = 100;
			float maxY = 125;

		#if _HEIGHT_FADE
			float lerpValue = saturate(1 - (abs(mul(unity_ObjectToWorld, v.vertex).y - ( 0.5 * (maxY + minY)))) / ((maxY-minY)*0.5));
			v.color.b = lerp(0, v.color.b, lerpValue);
		#endif
		#if _HEIGHT_HARD
			if(mul(unity_ObjectToWorld, v.vertex).y < minY || mul(unity_ObjectToWorld, v.vertex).y > maxY) {
				v.color.b = 0;
			}
		#endif
		#if _HEIGHT_CAP
			v.color.b *= smoothstep(minY,maxY,mul(unity_ObjectToWorld, v.vertex).y);
		#endif



			v.vertex.xyz = calculateWindAffections(v, unity_LODFade.x);


		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency, 0 );

			gi.light.color *= (1-transVdotL);
			_LightColor0 *= (1-transVdotL);

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			UNITY_GI(gi, s, data);
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{


			float4 albedo = lerp( 
								lerp(
										tex2D( _Albedo,i.uv_Albedo) * _AlbedoColor, 
										tex2D( _Albedo2,i.uv_Albedo) * _AlbedoColor2, 
										i.vertexColor.g), 
								tex2D( _Albedo3, i.uv_Albedo) * _AlbedoColor3,
								i.vertexColor.b
							); 

			clip( tex2D( _Albedo,i.uv_Albedo).a - _MaskClipValue );


			//Occlusion
			o.Albedo = albedo.rgb * i.vertexColor.r;

			o.Normal = lerp( 
								lerp(
										UnpackScaleNormal( tex2D( _Normal,i.uv_Albedo), _NormalScale ), 
										UnpackScaleNormal( tex2D( _Normal2,i.uv_Albedo), _NormalScale2 ), 
										i.vertexColor.g), 
								UnpackScaleNormal( tex2D( _Normal3,i.uv_Albedo), _NormalScale3 ),
								i.vertexColor.b
							); 


			
			float4 combinedData = tex2D( _Combined, i.uv_Albedo);
			float4 combinedData2 = tex2D( _Combined2, i.uv_Albedo);
			float4 combinedData3 = tex2D( _Combined3, i.uv_Albedo);


			o.Smoothness = lerp( 
								lerp(
										combinedData.b * _Smoothness, 
										combinedData2.b * _Smoothness2, 
										i.vertexColor.g), 
								combinedData3.b * _Smoothness3,
								i.vertexColor.b
							); 

			o.Occlusion = lerp( 
								lerp(
										lerp(1,combinedData.g, _Occlusion), 
										lerp(1,combinedData2.g, _Occlusion2), 
										i.vertexColor.g), 
								lerp(1,combinedData3.g, _Occlusion3),
								i.vertexColor.b
							); 



			//o.Translucency = combinedData.g * albedo.rgb;
			o.Translucency = albedo.rgb * lerp( 
								lerp(
										combinedData.r * _Translucency, 
										combinedData2.r * _Translucency2, 
										i.vertexColor.g), 
								combinedData3.r * _Translucency3,
								i.vertexColor.b
							); 
			o.Alpha = 1;

		}

		ENDCG






	}
	Fallback "Diffuse"
	CustomEditor "ShaderEditor_SpeedTree_Leaf"
}