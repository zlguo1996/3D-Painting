Shader "VTP/Metallic/3 Texture Blend (Advanced, Wetness, NoFlow)" {
	Properties {
		
		
		[Header(General)]
		[Toggle] _showGeneral("Show General", Float) = 0
		[Toggle] _showParallax("Show Parallax", Float) = 0
		[Toggle] _showRed("Show Red", Float) = 0
		[Toggle] _showGreen("Show Green", Float) = 0
		[Toggle] _showBlue("Show Blue", Float) = 0



		[Space(3)]
		[Toggle] _showVertexColor("Visualize Vertex Color", Float) = 0
		[Toggle] _showAlpha("Visualize Alpha Vertex Color", Float) = 0
		[Toggle] _showVertexFlow("Visualize Flow Data", Float) = 0

		_useHeightBasedBlending ("Use Heightbased Blending", Float) = 0
		_wetnessEdgeBlend ("Wetness Edge Blend", Range(0,1)) = 0.25

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
		_Emission ("Emission", Range(0,1)) = 0
		_EmissionColor ("Emission Color", COLOR) = (0,0,0)

		_wetnessAlbedoModifier ("Albedo Modifier", COLOR) = (0.3, 0.3, 0.35)
		_wetnessSmoothnessModifier ("Smoothness Modifier", Range(0,1)) = 0.8
		_wetnessNormalModifier ("Normal Modifier", Range(0,1)) = 0.5

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
		_Emission2 ("Emission", Range(0,1)) = 0
		_EmissionColor2 ("Emission Color", COLOR) = (0,0,0)
		_BaseHeight2 ("Base Height", Range(-2,2)) = 0
		_HeightmapFactor2 ("Height Map Factor", Range(0,1)) = 0.5
		_BlendSmooth2 ("Blend Smooth", Range(0,1)) = 0.5
		_HeightBasedTransparency2 ("Heightbased transparency", Range(0,1)) = 0.5 

		_wetnessAlbedoModifier2 ("Albedo Modifier", COLOR) = (0.3, 0.3, 0.35)
		_wetnessSmoothnessModifier2 ("Smoothness Modifier", Range(0,1)) = 0.8
		_wetnessNormalModifier2 ("Normal Modifier", Range(0,1)) = 0.5

		[Space(10)]
		
		[Header(Settings for third texture (blue channel))]
		[Space(3)]
		_MainTex3 ("Albedo (RGB)", 2D) = "white" {}
		[Normal] _BumpMap3 ("Normal Map", 2D) = "bump" {}
		_CombinedMap3 ("Combined Map", 2D) = "white" {}
		_Parallax3 ("Parallax Strength", Range(-0.05,0.05)) = -0.01
		[Toggle] _useP1_3("Use Heightmap of base layer", Float) = 1
		_Occlusion3 ("Occlusion Strength", Range(0,1)) = 1
		_Metallic3 ("Metallic Strength", Range(0,1)) = 0
		_Smoothness3 ("Smoothness Strength", Range(0,1)) = 1
		_Emission3 ("Emission", Range(0,1)) = 0
		_EmissionColor3 ("Emission Color", COLOR) = (0,0,0)
		_BaseHeight3 ("Base Height", Range(-2,2)) = 0
		_HeightmapFactor3 ("Height Map Factor", Range(0,1)) = 0.5
		_BlendSmooth3 ("Blend Smooth", Range(0,1)) = 0.5
		_HeightBasedTransparency3 ("Heightbased transparency", Range(0,1)) = 0.5 

		_wetnessAlbedoModifier3 ("Albedo Modifier", COLOR) = (0.3, 0.3, 0.35)
		_wetnessSmoothnessModifier3 ("Smoothness Modifier", Range(0,1)) = 0.8
		_wetnessNormalModifier3 ("Normal Modifier", Range(0,1)) = 0.5

	}
		
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		float _showVertexColor;
		float _showAlpha;
		float _showVertexFlow;

		float _useHeightBasedBlending;
		float _wetnessEdgeBlend;

		float _useParallax;
		int _ParallaxInterpolation;

		float _useDepthBias;
		float _DepthBias;
		float _DepthBiasPower;
		float _DepthBiasThreshold;

		float _useFlowMapGlobal;
		float _flowOnlyNormal;


		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _CombinedMap;
		float _Parallax;
		float _Occlusion;	
		float _Metallic;
		float _Smoothness;
		float _Emission;
		fixed3 _EmissionColor;

		float3 _wetnessAlbedoModifier;
		float _wetnessSmoothnessModifier;
		float _wetnessNormalModifier;



		sampler2D _MainTex2;
		sampler2D _BumpMap2;
		sampler2D _CombinedMap2;
		float _Parallax2;
		float _useP1_2;
		float _Occlusion2;	
		float _Metallic2;
		float _Smoothness2;
		float _Emission2;
		fixed3 _EmissionColor2;
		float _BaseHeight2;
		float _HeightmapFactor2;
		float _BlendSmooth2;
		float _HeightBasedTransparency2;

		float3 _wetnessAlbedoModifier2;
		float _wetnessSmoothnessModifier2;
		float _wetnessNormalModifier2;


		sampler2D _MainTex3;
		sampler2D _BumpMap3;
		sampler2D _CombinedMap3;
		float _Parallax3;
		float _useP1_3;
		float _Occlusion3;	
		float _Metallic3;
		float _Smoothness3;		
		float _Emission3;
		fixed3 _EmissionColor3;
		float _BaseHeight3;
		float _HeightmapFactor3;
		float _BlendSmooth3;
		float _HeightBasedTransparency3;

		float3 _wetnessAlbedoModifier3;
		float _wetnessSmoothnessModifier3;
		float _wetnessNormalModifier3;


		float4 _MainTex_TexelSize;
		float4 _MainTex2_TexelSize;
		float4 _MainTex3_TexelSize;


		/*
         * Steep Parallax Mapping
         * graphics.cs.brown.edu/games/SteepParallax
         */

       	float2 calculateParallaxOffset(float3 viewDir, fixed2 uv, sampler2D heightMap, fixed parallax) {
		

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

			if(inverse==1) v *= -1;

			v.z = abs(v.z);				
			v.xy *= (1 - pow( v.z, 16 )) * parallax;

			const int linearSearchSteps = _ParallaxInterpolation;
			const int binarySearchSteps = _ParallaxInterpolation * 0.5;
			
			v /= v.z * linearSearchSteps;

			int i;
			fixed tex;
			for( i=0;i<linearSearchSteps;i++ )
			{
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

		float mip_map_level(float2 texture_coordinate)
		{
		    float2  dx_vtc        = ddx(texture_coordinate);
		    float2  dy_vtc        = ddx(texture_coordinate);
		    float delta_max_sqr = max(dot(dx_vtc, dx_vtc), dot(dy_vtc, dy_vtc));
		    float mml = 0.5 * log2(delta_max_sqr);
		    if( mml < 0 )
		    	return 0;
		    else
		   	 return mml;
		}



		struct Input {
			float2 uv_MainTex;
			
			float2 uv_MainTex2;

			float2 uv_MainTex3;

            float2 secondUV;
            float2 wetness;

            fixed4 color : COLOR;

            float3 viewDir;

            INTERNAL_DATA
       	};





       	
       	void vert (inout appdata_full v, out Input o) {

       		UNITY_INITIALIZE_OUTPUT(Input,o);

       		o.wetness = v.texcoord3;
        	o.secondUV.y = length(ObjSpaceViewDir(v.vertex));

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




			if( _useDepthBias ) {

				half viewDistance = IN.secondUV.y;
				fixed depthFactor = 1;

				if( _DepthBiasPower > 0 )
					depthFactor = pow(1-saturate(length(viewDistance)/_DepthBias),_DepthBiasPower);

				_Parallax *= depthFactor;
				_Parallax2 *= depthFactor;
				_Parallax3 *= depthFactor;

			}

			if( abs(_Parallax) < _DepthBiasThreshold  )
				_Parallax = 0;

			if( abs(_Parallax2) < _DepthBiasThreshold  )
				_Parallax2 = 0;

			if( abs(_Parallax3) < _DepthBiasThreshold  )
				_Parallax3 = 0;


			fixed factorAlpha = IN.color.a;
			fixed factorRed = IN.color.r;
			fixed factorGreen = IN.color.g;
			fixed factorBlue = IN.color.b;

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

				if( factorBlue > 0  ) {
					if( _useP1_3) {
						uv_offset += factorBlue * calculateParallaxOffset(IN.viewDir, IN.uv_MainTex, _CombinedMap, _Parallax3) ;
					} else {
						uv_offset += factorBlue * calculateParallaxOffset(IN.viewDir, IN.uv_MainTex3, _CombinedMap3, _Parallax3);
					}
				}
									
				IN.uv_MainTex += uv_offset;
				IN.uv_MainTex2 += uv_offset;
				IN.uv_MainTex3 += uv_offset;

			}

			fixed normalFactorGreen = factorGreen;
			fixed normalFactorBlue  = factorBlue;
			fixed normalFactorRed = 1.0 - factorGreen - factorBlue;
			fixed normalCorrectionFactor = 1 / (normalFactorRed + normalFactorGreen + normalFactorBlue);

			fixed heightData = 0;
			fixed heightMapGreen;
			fixed heightMapBlue;
			half heightGreen = 0;
			half heightBlue = 0;

			if( _useHeightBasedBlending ) {


				heightData = tex2D (_CombinedMap, IN.uv_MainTex).r * 2 - 1;

				heightMapGreen = tex2D( _CombinedMap2, IN.uv_MainTex2).r * 2 - 1;
				heightMapBlue = tex2D( _CombinedMap3, IN.uv_MainTex3).r * 2 - 1;




				heightGreen = _BaseHeight2 + IN.color.a * 2 - 1.0 + (heightMapGreen * 2 - 1.0) * _HeightmapFactor2 ;
				heightBlue = _BaseHeight3 + IN.color.a * 1 - 1.0 + (heightMapBlue * 2 - 1.0) * _HeightmapFactor3;
			

				_BlendSmooth2 += 0.1 * ( _BlendSmooth2 + 0.001 );

				if( heightGreen <= heightData - _BlendSmooth2 || factorGreen == 0) {
					factorGreen = 0;
					
				} else if ( heightGreen > heightData - _BlendSmooth2 && heightGreen < heightData ) {
					factorGreen = IN.color.g *  pow( (heightGreen - heightData + _BlendSmooth2) / _BlendSmooth2, 0.5);

				} else {

				}


				_BlendSmooth3 += 0.1 * ( _BlendSmooth3 + 0.001 );

				if( heightBlue <= heightData - _BlendSmooth3 || factorBlue == 0) {
					factorBlue = 0;
				
				} else if ( heightBlue > heightData - _BlendSmooth3 && heightBlue < heightData ) {
					factorBlue = IN.color.b * pow( (heightBlue - heightData + _BlendSmooth3) / _BlendSmooth3, 0.5);
				} else {

				}


				normalFactorGreen = factorGreen;
				normalFactorBlue  = factorBlue;
				normalFactorRed = 1.0 - factorGreen - factorBlue;
				normalCorrectionFactor = 1 / (normalFactorRed + normalFactorGreen + normalFactorBlue);

				if( _HeightBasedTransparency2 > 0 )
					factorGreen *= pow( saturate( heightGreen - heightData + _BlendSmooth2 ) , 0.5 * _HeightBasedTransparency2);

				if( _HeightBasedTransparency3 > 0 )
					factorBlue *= pow( saturate( heightBlue - heightData + _BlendSmooth3) , 0.5 * _HeightBasedTransparency3);

				factorRed = 1.0 - factorGreen - factorBlue ;

			}

								
			//Correction factor for blending between the colors			
			fixed correctionFactor = 1 / (factorRed + factorGreen + factorBlue);


			/* Flow && Output*/

		    fixed3 _albedo;
       		fixed3 _normal;
       		fixed _smoothness;
       		fixed _occlusion;
       		fixed _metallic;
       		fixed _emission;


			float mipL1 = mip_map_level(IN.uv_MainTex * _MainTex_TexelSize.zw);
			float mipL2 = mip_map_level(IN.uv_MainTex2 * _MainTex2_TexelSize.zw);
			float mipL3 = mip_map_level(IN.uv_MainTex3 * _MainTex3_TexelSize.zw);



			fixed4 combinedMapData = tex2Dlod (_CombinedMap, float4(IN.uv_MainTex,0,mipL1));
			fixed4 combinedMap2Data = tex2Dlod (_CombinedMap2, float4(IN.uv_MainTex2,0,mipL2));
			fixed4 combinedMap3Data = tex2Dlod (_CombinedMap3, float4(IN.uv_MainTex3,0,mipL3));
			           

		
			float wetness = IN.wetness.x;
			float wetnessHeight = IN.wetness.y * 2 - 1;


			float3 wetnessAlbedo;
			float3 wetnessAlbedo2;
			float3 wetnessAlbedo3;


			float wetnessSmoothness;
			float wetnessSmoothness2; 
			float wetnessSmoothness3; 

			float wetnessNormalScale; 
			float wetnessNormalScale2;
			float wetnessNormalScale3;

			//float3 _wetnessAlbedoModifier;
			//float _wetnessSmoothnessModifier;
			//float _wetnessNormalModifier;

			float wetnessHeightData = correctionFactor * (heightData*factorRed + heightGreen * factorGreen / 4 + heightBlue * factorBlue / 4);

			if ( wetnessHeight > wetnessHeightData - _wetnessEdgeBlend && wetnessHeight < wetnessHeightData ) {
				wetness = wetness * pow( (wetnessHeight - wetnessHeightData + _wetnessEdgeBlend) / _wetnessEdgeBlend, 0.5);

				wetnessAlbedo = lerp( float3(1,1,1), _wetnessAlbedoModifier, wetness);
				wetnessAlbedo2 = lerp( float3(1,1,1), _wetnessAlbedoModifier2, wetness);
				wetnessAlbedo3 = lerp( float3(1,1,1), _wetnessAlbedoModifier3, wetness);

				wetnessSmoothness = lerp(combinedMapData.b * _Smoothness,_wetnessSmoothnessModifier,wetness);
				wetnessSmoothness2 = lerp(combinedMap2Data.b * _Smoothness2,_wetnessSmoothnessModifier2,wetness);
				wetnessSmoothness3 = lerp(combinedMap3Data.b * _Smoothness3,_wetnessSmoothnessModifier3,wetness);

				wetnessNormalScale = lerp(1,1-_wetnessNormalModifier,wetness);
				wetnessNormalScale2 = lerp(1,1-_wetnessNormalModifier2,wetness);
				wetnessNormalScale3 = lerp(1,1-_wetnessNormalModifier3,wetness);


			} else if( wetnessHeightData >= wetnessHeight  ) {

				wetnessAlbedo = float3(1,1,1);
				wetnessAlbedo2 = float3(1,1,1);
				wetnessAlbedo3 = float3(1,1,1);
				wetnessSmoothness = combinedMapData.b * _Smoothness;
				wetnessSmoothness2 = combinedMap2Data.b * _Smoothness2;
				wetnessSmoothness3 = combinedMap3Data.b * _Smoothness3;
				wetnessNormalScale = 1;
				wetnessNormalScale2 = 1;
				wetnessNormalScale3 = 1;

			} else {

				wetnessAlbedo = lerp( float3(1,1,1), _wetnessAlbedoModifier, wetness);
				wetnessAlbedo2 = lerp( float3(1,1,1), _wetnessAlbedoModifier2, wetness);
				wetnessAlbedo3 = lerp( float3(1,1,1), _wetnessAlbedoModifier3, wetness);

				wetnessSmoothness = lerp(combinedMapData.b * _Smoothness,_wetnessSmoothnessModifier,wetness);
				wetnessSmoothness2 = lerp(combinedMap2Data.b * _Smoothness2,_wetnessSmoothnessModifier2,wetness);
				wetnessSmoothness3 = lerp(combinedMap3Data.b * _Smoothness3,_wetnessSmoothnessModifier3,wetness);

				wetnessNormalScale = lerp(1,1-_wetnessNormalModifier,wetness);
				wetnessNormalScale2 = lerp(1,1-_wetnessNormalModifier2,wetness);
				wetnessNormalScale3 = lerp(1,1-_wetnessNormalModifier3,wetness);

			}







			//Normal w nCF
			_normal = normalCorrectionFactor * (UnpackScaleNormal (tex2Dlod (_BumpMap, float4(IN.uv_MainTex,0,mipL1)), wetnessNormalScale) * normalFactorRed + UnpackScaleNormal (tex2Dlod (_BumpMap2, float4(IN.uv_MainTex2,0,mipL2)), wetnessNormalScale2)* normalFactorGreen + UnpackScaleNormal (tex2Dlod (_BumpMap3, float4(IN.uv_MainTex3,0,mipL3)), wetnessNormalScale3) * normalFactorBlue ); 	                 

			//Albedo w CF
			_albedo = correctionFactor * ( tex2Dlod (_MainTex, float4(IN.uv_MainTex,0,mipL1)).rgb * factorRed * wetnessAlbedo + tex2Dlod (_MainTex2, float4(IN.uv_MainTex2, 0, mipL2) ).rgb * factorGreen * wetnessAlbedo2 + tex2Dlod (_MainTex3, float4(IN.uv_MainTex3,0,mipL3)).rgb * factorBlue * wetnessAlbedo3 );

	        //Smoothness w CF     
	        _smoothness = normalCorrectionFactor * (  wetnessSmoothness * normalFactorRed + wetnessSmoothness2 * normalFactorGreen + wetnessSmoothness3 * normalFactorBlue ) ;
				
			//Occlusion w CF
			_occlusion =  correctionFactor * ( lerp(1, combinedMapData.g, _Occlusion) * factorRed  + lerp(1, combinedMap2Data.g, _Occlusion2) * factorGreen + lerp(1, combinedMap3Data.g, _Occlusion3) * factorBlue );
				
			//Metallic
			_metallic =  correctionFactor * ( _Metallic * tex2Dlod (_MainTex, float4(IN.uv_MainTex,0,mipL1)).a * factorRed + _Metallic2 * tex2Dlod (_MainTex2, float4(IN.uv_MainTex2,0,mipL2)).a * factorGreen + _Metallic3 * tex2Dlod (_MainTex3, float4(IN.uv_MainTex3,0,mipL3)).a * factorBlue);

			_emission =  correctionFactor * ( _EmissionColor * _Emission * combinedMapData.a * factorRed + _EmissionColor2 * _Emission2 * combinedMap2Data.a * factorGreen + _EmissionColor3 * _Emission3 * combinedMap3Data.a * factorBlue);

	

       		o.Albedo = _albedo;
       		o.Normal = _normal;
       		o.Smoothness = _smoothness;
       		o.Metallic = _metallic;
       		o.Occlusion = _occlusion;
       		o.Emission = _emission;

		
		}
		ENDCG
	} 
	FallBack "Diffuse"
	CustomEditor "ShaderEditor3TexWet"
}

