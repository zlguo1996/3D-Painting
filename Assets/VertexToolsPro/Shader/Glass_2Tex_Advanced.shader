// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VTP/Glass/2 Texture blend (2-sided)" {
	Properties {

	[Header(General)]
		[Toggle] _showGeneral("Show General", Float) = 0
		[Toggle] _showParallax("Show Parallax", Float) = 0
		[Toggle] _showRed("Show Red", Float) = 0
		[Toggle] _showGreen("Show Green", Float) = 0
		[Toggle] _showBlue("Show Blue", Float) = 0
		[Toggle] _useFlowMapGlobal("Use Global FlowMap", Float) = 0

		[Space(3)]
		[Toggle] _showVertexColor("Visualize Vertex Color", Float) = 0
		[Toggle] _showAlpha("Visualize Alpha Vertex Color", Float) = 0
		[Toggle] _showVertexFlow("Visualize Flow Data", Float) = 0

		_useHeightBasedBlending ("Use Heightbased Blending", Float) = 0

		[Space(3)]
		_useParallax ("Use Parallax", Float) = 1
		_ParallaxInterpolation ("Parallax Interpolation", Int) = 15


		[Space(10)]

		[Header(Depth Bias)]
		[Space(3)]
		_useDepthBias ("Use Depth-Bias", Float) = 0
		_DepthBias ("Depth Bias Distance", Range(0,100)) = 50
		_DepthBiasPower ("Depth Bias Power", Range(1,10)) = 2
		_DepthBiasThreshold ("Depth Bias Threshold", Range(0,0.01)) = 0

		[Space(10)]


		[Space(15)]
		
		[Header(Settings for first texture (red channel))]
		[Space(3)]
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_CombinedMap ("Combined Map", 2D) = "white" {}
		_Parallax ("Parallax Strength", Range(-0.05,0.05)) = -0.01
		_Occlusion ("Occlusion Strength", Range(0,1)) = 1
		_Metallic ("Metallic Strength", Range(0,1)) = 0
		_Smoothness ("Smoothness Strength", Range(0,1)) = 1

		_BumpAmt  ("Distortion", range (0,64)) = 10
		_Transparency ("Transparency", range (0,1)) = 0.5
		_TransparencyBlur ("Transparency Blur", range (0,10)) = 1
		_TransEmission ("Transparency Emission", Range(0,1)) = 0
		_GlobalTrans ("Global Transparency", Range(0,1)) = 0

		[Space(10)]
		
		[Header(Settings for second texture (green channel))]
		[Space(3)]
		_MainTex2 ("Albedo (RGB)", 2D) = "white" {}
		[Normal] _BumpMap2 ("Normal Map", 2D) = "bump" {}
		_CombinedMap2 ("Combined Map", 2D) = "white" {}
		_Parallax2 ("Parallax Strength", Range(-0.05,0.05)) = -0.01
		[Toggle] _useP1_2("Use Heightmap of base layer", Float) = 1
		_Occlusion2 ("Occlusion Strength", Range(0,1)) = 1
		_Metallic2 ("Metallic Strength", Range(0,1)) = 0
		_Smoothness2 ("Smoothness Strength", Range(0,1)) = 1

		_BaseHeight2 ("Base Height", Range(-2,2)) = 0
		_HeightmapFactor2 ("Height Map Factor", Range(0,1)) = 0.5
		_BlendSmooth2 ("Blend Smooth", Range(0,1)) = 0.5
		_HeightBasedTransparency2 ("Heightbased transparency", Range(0,1)) = 0.5 
		[Toggle] _useFlowMap2("Use FlowMap", Float) = 0
		_FlowSpeed2 ("Flow Speed", Float) = 0.25
		[Toggle] _useDrift2 ("Use Height drift", Float) = 1

		_BumpAmt2  ("Distortion", range (0,64)) = 10
		_Transparency2 ("Transparency", range (0,1)) = 0.5
		_TransparencyBlur2 ("Transparency Blur", range (0,10)) = 1
		_TransEmission2 ("Transparency Emission", Range(0,1)) = 0
		_GlobalTrans2 ("Global Transparency", Range(0,1)) = 0

	}
		
	
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		GrabPass { "_GrabMapGlass" }

	    Cull Off ZWrite On

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert 
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0



		float _showVertexColor;
		float _showAlpha;
		float _showVertexFlow;

		float _useFlowMapGlobal;
		float _useHeightBasedBlending;

		float _useParallax;
		int _ParallaxInterpolation;

		float _useDepthBias;
		float _DepthBias;
		float _DepthBiasPower;
		float _DepthBiasThreshold;


		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _CombinedMap;
		float _Parallax;
		float _Occlusion;	
		float _Metallic;
		float _Smoothness;

		float _BumpAmt;
		float _Transparency;
		float _TransparencyBlur;
		float _TransEmission;
		float _GlobalTrans;

		sampler2D _MainTex2;
		sampler2D _BumpMap2;
		sampler2D _CombinedMap2;
		float _Parallax2;
		float _useP1_2;
		float _Occlusion2;	
		float _Metallic2;
		float _Smoothness2;

		float _BaseHeight2;
		float _HeightmapFactor2;
		float _BlendSmooth2;
		float _HeightBasedTransparency2;
		float _useFlowMap2;
		float _FlowSpeed2;
		float _useDrift2;

		float _BumpAmt2;
		float _Transparency2;
		float _TransparencyBlur2;
		float _TransEmission2;
		float _GlobalTrans2;


		float4 _MainTex_TexelSize;
		float4 _MainTex2_TexelSize;

		sampler2D _GrabBlurTexture;
		float4 _GrabBlurTexture_TexelSize;

		sampler2D _GrabMapGlass;
		float4 _GrabMapGlass_TexelSize;
		
		
		struct Input {
			float2 uv_MainTex;
			float2 uv_MainTex2;
            float4 grabuv;

            float2 secondUV;
            float2 flowUV;

            float4 color : COLOR;

            float3 worldNormal;
            float3 viewDir;

            INTERNAL_DATA
       	};
       	
       	void vert (inout appdata_full v, out Input o) {
       	
       		UNITY_INITIALIZE_OUTPUT(Input,o);

       		o.flowUV = v.texcoord3 * 2.0f - 1.0f;
       		o.secondUV.y = length(ObjSpaceViewDir(v.vertex));

       		float4 vert = UnityObjectToClipPos(v.vertex);

        	#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			o.grabuv.xy = (float2(vert.x, vert.y*scale) + vert.w) * 0.5;
			o.grabuv.zw = vert.zw;

        }


        /*
         * Steep Parallax Mapping
         * graphics.cs.brown.edu/games/SteepParallax
         */

		float2 calculateParallaxOffset(float3 viewDir, float2 uv, sampler2D heightMap, float parallax) {
		

			float3 uv_offset			= float3(0,0,0);

			int inverse;

			if( parallax == 0 ) {
				return uv_offset.xy;
			} else  if ( parallax > 0 ) {
				inverse = 1;
			} else {
				inverse = 0;
				parallax *= -1;
			}

			viewDir = normalize(viewDir); 
			float3 p = float3(uv,0);
			float3 v = -viewDir;

			if(inverse==1)
			v *= -1;

			v.z						= abs(v.z);
						
			v.xy						*= (1 - pow( v.z, 16 ));
			v.xy						*= parallax;



			const int linearSearchSteps = _ParallaxInterpolation;
			const int binarySearchSteps = _ParallaxInterpolation * 0.5;
			
			v /= v.z * linearSearchSteps;
			
			int i;
			for( i=0;i<linearSearchSteps;i++ )
			{
				float tex;
				if(inverse == 1) {
					tex = tex2Dlod(heightMap, float4(p.xy,0,0)).r;
				} else {
					tex = 1.0 - tex2Dlod(heightMap, float4(p.xy,0,0)).r;
				}
			    
				if (p.z<tex) {
					p+=v;
					uv_offset += v;
				}
			}
			
			for( i=0;i<binarySearchSteps;i++ )
			{
				v *= 0.5;

				float tex;
				if(inverse == 1) {
					tex = tex2Dlod(heightMap, float4(p.xy,0,0)).r;
				} else {
					tex = 1.0 - tex2Dlod(heightMap, float4(p.xy,0,0)).r;
				}

				if (p.z < tex) {
					p += v;	
					uv_offset += v;
				} else {
					p -= v;
					uv_offset -= v;
				}
			}
		
			return uv_offset.xy;
		
		}


	

		void surf (Input IN, inout SurfaceOutputStandard o) {

			if( _showVertexColor == 1) {

				o.Albedo = IN.color.rgb;

				return;
			}
			if( _showAlpha == 1 ) {

				o.Albedo = fixed3(IN.color.a, IN.color.a, IN.color.a);
				return;
			}

			if( _showVertexFlow == 1 ) {

				o.Albedo = fixed3(IN.flowUV,0);
				return;

			}

			if( _useDepthBias ) {

				half viewDistance = IN.secondUV.y;
				fixed depthFactor = 1;

				if( _DepthBiasPower > 0 )
					depthFactor = pow(1-saturate(length(viewDistance)/_DepthBias),_DepthBiasPower);

				_Parallax *= depthFactor;
				_Parallax2 *= depthFactor;

			}



			fixed factorAlpha = IN.color.a;
			fixed factorRed = IN.color.r;
			fixed factorGreen = IN.color.g;

			if( _useParallax && _ParallaxInterpolation > 0 ) {

				fixed2 uv_offset = float2(0,0);

				if( factorRed > 0 )
					uv_offset =  calculateParallaxOffset(IN.viewDir, IN.uv_MainTex, _CombinedMap, _Parallax) * factorRed;

				if( factorGreen > 0 ) {
					if( _useP1_2) {
						uv_offset += calculateParallaxOffset(IN.viewDir, IN.uv_MainTex, _CombinedMap, _Parallax2) * factorGreen;
					} else {
						uv_offset += factorGreen * calculateParallaxOffset(IN.viewDir, IN.uv_MainTex2, _CombinedMap2, _Parallax2);
					}
				}

									
				IN.uv_MainTex += uv_offset;
				IN.uv_MainTex2 += uv_offset;

			}




			fixed normalFactorGreen = factorGreen;
			fixed normalFactorRed = 1.0 - factorGreen;
			fixed normalCorrectionFactor = 1 / (normalFactorRed + normalFactorGreen);

			if( _useHeightBasedBlending ) {


				fixed heightData = tex2D (_CombinedMap, IN.uv_MainTex).r;

				fixed heightMapGreen;

				if(_useDrift2 == 0 || _useFlowMapGlobal == 0) {
					heightMapGreen = tex2D( _CombinedMap2, IN.uv_MainTex2).r;
				} else {

					half timeScale = _Time[1] * 0.25f;
					half phase0 = frac(timeScale);
	        	    half phase1 = frac(timeScale + 0.5);
	          		half flowLerp = abs((0.5f - phase0) * 2);

					fixed2 flowDirGreen = IN.flowUV * factorGreen * _FlowSpeed2 ;

					fixed2 flowUV2_phase0 =  IN.uv_MainTex2;
					fixed2 flowUV2_phase1 = IN.uv_MainTex2;

					if( _useFlowMap2 == 1 ) {
						flowUV2_phase0 += flowDirGreen.xy * phase0;
						flowUV2_phase1 += flowDirGreen.xy * phase1;
					}

					fixed heightMapGreen1 = tex2D( _CombinedMap2, flowUV2_phase0).r;
					fixed heightMapGreen2 = tex2D( _CombinedMap2, flowUV2_phase1).r;
					heightMapGreen = lerp( heightMapGreen1, heightMapGreen2, flowLerp);
				}


				half heightGreen = _BaseHeight2 + IN.color.a * 2 - 1.0 + (heightMapGreen * 2 - 1.0) * _HeightmapFactor2 ;
			

				_BlendSmooth2 += 0.1 * ( _BlendSmooth2 + 0.001 );

				if( heightGreen <= heightData - _BlendSmooth2 || factorGreen == 0) {
					factorGreen = 0;
					
				} else if ( heightGreen > heightData - _BlendSmooth2 && heightGreen < heightData ) {
					factorGreen = IN.color.g *  pow( (heightGreen - heightData + _BlendSmooth2) / _BlendSmooth2, 0.5);

				} else {

				}




				normalFactorGreen = factorGreen;
				normalFactorRed = 1.0 - factorGreen;
				normalCorrectionFactor = 1 / (normalFactorRed + normalFactorGreen);

				if( _HeightBasedTransparency2 > 0 )
					factorGreen *= pow( saturate( heightGreen - heightData + _BlendSmooth2 ) , 0.5 * _HeightBasedTransparency2);
				factorRed = 1.0 - factorGreen ;

			}

								
			//Correction factor for blending between the colors			
			fixed correctionFactor = 1 / (factorRed + factorGreen);




			float3 _normal;
			float2 grabvuv_offset;
			if( (IN.flowUV.x == 0 && IN.flowUV.y == 0) || (_useFlowMap2 == 0 ) || _useFlowMapGlobal == 0) {

				//Normal w nCF
				_normal = normalCorrectionFactor * (UnpackNormal (tex2D (_BumpMap, IN.uv_MainTex)) * normalFactorRed + UnpackNormal (tex2D (_BumpMap2, IN.uv_MainTex2))* normalFactorGreen  ); 

				grabvuv_offset = normalCorrectionFactor * (UnpackNormal (tex2D (_BumpMap, IN.uv_MainTex)) * normalFactorRed * _BumpAmt + UnpackNormal (tex2D (_BumpMap2, IN.uv_MainTex2))* normalFactorGreen * _BumpAmt2  ) * _GrabMapGlass_TexelSize.xy;		                 

			} else {

				half timeScale = _Time[1] * 0.25f;
				half phase0 = frac(timeScale);
        	    half phase1 = frac(timeScale + 0.5);
          		half flowLerp = abs((0.5f - phase0) * 2);

				fixed2 flowDirGreen = IN.flowUV * factorGreen * _FlowSpeed2 ;

				fixed2 flowUV2_phase0 =  IN.uv_MainTex2;
				fixed2 flowUV2_phase1 = IN.uv_MainTex2;



				if( _useFlowMap2 == 1 ) {
					flowUV2_phase0 += flowDirGreen.xy * phase0;
					flowUV2_phase1 += flowDirGreen.xy * phase1;
				}


				fixed3 normalDataRed = UnpackNormal (tex2D (_BumpMap, IN.uv_MainTex)) * normalFactorRed;
				fixed3 normal1 = ( normalDataRed + UnpackNormal (tex2D (_BumpMap2, flowUV2_phase0)) * normalFactorGreen );  
				fixed3 normal2 = ( normalDataRed + UnpackNormal (tex2D (_BumpMap2, flowUV2_phase1)) * normalFactorGreen );  
           		_normal = normalCorrectionFactor * lerp( normal1, normal2, flowLerp );

           		fixed3 normalForGrab1 = ( normalDataRed *_BumpAmt + UnpackNormal (tex2D (_BumpMap2, flowUV2_phase0)) * normalFactorGreen * _BumpAmt2 );  
				fixed3 normalForGrab2 = ( normalDataRed *_BumpAmt + UnpackNormal (tex2D (_BumpMap2, flowUV2_phase1)) * normalFactorGreen * _BumpAmt2 ); 
				grabvuv_offset = normalCorrectionFactor * lerp( normalForGrab1, normalForGrab2, flowLerp ) * _GrabMapGlass_TexelSize.xy;	


			}


			IN.grabuv.xy = grabvuv_offset + IN.grabuv.xy;

			#define GRABPIXELTRANS(weight,kernel,size) tex2Dproj( _GrabMapGlass, UNITY_PROJ_COORD(float4(IN.grabuv.x + _GrabMapGlass_TexelSize.x * kernel*size, IN.grabuv.y +_GrabMapGlass_TexelSize.y * kernel*size, IN.grabuv.z, IN.grabuv.w)) ) * weight

			float _transitionBlurryness = correctionFactor * ( _TransparencyBlur * factorRed + _TransparencyBlur2 * factorGreen);

			float3 bgrColor = GRABPIXELTRANS(0.05, -4.0, _transitionBlurryness);
	 		bgrColor += GRABPIXELTRANS(0.07, -3.5, _transitionBlurryness);
	 		bgrColor += GRABPIXELTRANS(0.09, -3.0, _transitionBlurryness) ;
	 		bgrColor += GRABPIXELTRANS(0.105, -2.5, _transitionBlurryness) ;
	 		bgrColor +=   GRABPIXELTRANS(0.12, -2.0, _transitionBlurryness) ;
	 		bgrColor +=  GRABPIXELTRANS(0.135, -1.5, _transitionBlurryness) ;
			bgrColor +=    GRABPIXELTRANS(0.15, -1.0, _transitionBlurryness);
	 		bgrColor +=    GRABPIXELTRANS(0.165, -0.5, _transitionBlurryness); 
	 		bgrColor +=   GRABPIXELTRANS(0.18,  0.0, _transitionBlurryness) ; 
	 		bgrColor += GRABPIXELTRANS(0.165, +0.5, _transitionBlurryness);
	 		bgrColor +=  GRABPIXELTRANS(0.15, +1.0, _transitionBlurryness) ;
	 		bgrColor +=  GRABPIXELTRANS(0.135, +1.5, _transitionBlurryness) ;
	 		bgrColor +=  GRABPIXELTRANS(0.12, +2.0, _transitionBlurryness) ;
	 		bgrColor +=   GRABPIXELTRANS(0.105, +2.5, _transitionBlurryness); 
	 		bgrColor +=  GRABPIXELTRANS(0.09, +3.0, _transitionBlurryness) ;
	 		bgrColor +=  GRABPIXELTRANS(0.07, +3.5, _transitionBlurryness);
	 		bgrColor +=  GRABPIXELTRANS(0.05, +4.0, _transitionBlurryness);

	 		bgrColor *=  0.5 ;
																																																			
			//Albedo w CF
			o.Albedo = correctionFactor * ( lerp(bgrColor, tex2D (_MainTex, IN.uv_MainTex).rgb, (1 - _GlobalTrans) * (1 - _Transparency * ( 1- tex2D (_MainTex, IN.uv_MainTex).a)) ) * factorRed + lerp(bgrColor, tex2D (_MainTex2, IN.uv_MainTex2 ).rgb,  (1 - _GlobalTrans2) * (1 - _Transparency2 * ( 1- tex2D (_MainTex2, IN.uv_MainTex2).a)) ) * factorGreen );

			//Normals w CF
            o.Normal = _normal;

            fixed4 combinedMapData = tex2D (_CombinedMap, IN.uv_MainTex);
			fixed4 combinedMap2Data = tex2D (_CombinedMap2, IN.uv_MainTex2);
              
           	//Smoothness w CF     
	        o.Smoothness = normalCorrectionFactor * ( _Smoothness * combinedMapData.b * normalFactorRed + _Smoothness2 * combinedMap2Data.b * normalFactorGreen ) ;
				
			//Occlusion w CF
			o.Occlusion =  correctionFactor * ( lerp(1, combinedMapData.g, _Occlusion) * factorRed  + lerp(1, combinedMap2Data.g, _Occlusion2) * factorGreen );
				
			//Metallic
			o.Metallic =  0;

			o.Emission =  correctionFactor * ( lerp(bgrColor, tex2D (_MainTex, IN.uv_MainTex).rgb, (1 - _GlobalTrans) * (1 - _Transparency * ( 1- tex2D (_MainTex, IN.uv_MainTex).a)) ) * factorRed * _TransEmission + lerp(bgrColor, tex2D (_MainTex2, IN.uv_MainTex2 ).rgb, (1 - _GlobalTrans2) * (1 - _Transparency2 * ( 1- tex2D (_MainTex2, IN.uv_MainTex2).a)) ) * factorGreen * _TransEmission2 );
		
	
		}
		ENDCG



		Pass {
             Name "ShadowCaster"
             Tags { "LightMode" = "ShadowCaster" }
           
             Fog { Mode Off }
             ZWrite On ZTest Less Cull Off
             Offset 1, 1
             
             CGPROGRAM
 
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile_shadowcaster
             #pragma fragmentoption ARB_precision_hint_fastest
             #include "UnityCG.cginc"

             float _useHeightBasedBlending;

             sampler2D _MainTex;
             sampler2D _CombinedMap;
             float4 _MainTex_ST;
             float _GlobalTrans;


   			 sampler2D _MainTex2;
   			 sampler2D _CombinedMap2;
             float4 _MainTex2_ST;
             float _BaseHeight2;
             float _BlendSmooth2;
             float _HeightBasedTransparency2;
             float _GlobalTrans2;


             struct v2f
             { 
                 V2F_SHADOW_CASTER;
                 float2 uv : TEXCOORD1;
                 float2 uv2 : TEXCOORD2;
                 float4 color : COLOR;
                 float2 screenPos : TEXCOORD3;
             };

           
             v2f vert(appdata_full v)
             {
                 v2f o;
                 o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
                 o.uv2 = TRANSFORM_TEX (v.texcoord, _MainTex2);
                 o.color = v.color;

   				 
                 TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)

                 return o;
             }
           
             float4 frag(v2f i) : COLOR
             {

        
             	fixed factorAlpha = i.color.a;
				fixed factorRed = i.color.r;
				fixed factorGreen = i.color.g;


				if( _useHeightBasedBlending ) {


					fixed heightData = tex2D (_CombinedMap, i.uv).r;

					half heightGreen = (_BaseHeight2 + i.color.a + tex2D( _CombinedMap2, i.uv2).r - 0.75);
				

					_BlendSmooth2 += 0.1 * ( _BlendSmooth2 + 0.001 );
					heightGreen *= factorAlpha;

					if( heightGreen <= heightData - _BlendSmooth2 || factorGreen == 0) {
						factorGreen = 0;
						
					} else if ( heightGreen > heightData - _BlendSmooth2 && heightGreen < heightData ) {
						factorGreen = i.color.g *  pow( (heightGreen - heightData + _BlendSmooth2) / _BlendSmooth2, 0.5);
					}


					if( _HeightBasedTransparency2 > 0 )
						factorGreen *= pow( saturate( heightGreen - heightData + _BlendSmooth2 ) , 0.5 * _HeightBasedTransparency2);
					factorRed = 1.0 - factorGreen ;

				}





             	fixed4 texcol = tex2D( _MainTex, i.uv ) * factorRed * (1 - _GlobalTrans) + tex2D( _MainTex2, i.uv2 ) * factorGreen * (1 - _GlobalTrans2);

             	if(  texcol.a - 0.95 < 0 )
					clip( -1 );



                SHADOW_CASTER_FRAGMENT(i)
             }
 
             ENDCG
          }




	} 
	FallBack "Diffuse"
	CustomEditor "ShaderEditorGlass"
}
