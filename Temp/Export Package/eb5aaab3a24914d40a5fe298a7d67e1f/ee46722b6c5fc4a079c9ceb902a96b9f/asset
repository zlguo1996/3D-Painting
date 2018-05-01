#ifndef VTP_UTIL_INCLUDED
#define VTP_UTIL_INCLUDED


half3 Albedo(float2 tex_Red,float2 tex_Green,float2 tex_Blue, fixed4 vColor)
{

	half3 albedo_Red = _AlbedoColor * tex2D (_MainTex, tex_Red).rgb;
#if METALLIC
	albedo_Red *= LerpWhiteTo (tex2D (_DetailTex, tex_Red * float2(_DetailScaleU,_DetailScaleV)).rgb , tex2D (_DetailMask, tex_Red).a);
#endif
	half3 albedo_Green = _AlbedoColor2 * tex2D (_MainTex2, tex_Green).rgb;
	//albedo_Green *= LerpWhiteTo (tex2D (_DetailTex2, tex_Green * float2(_DetailScaleU2,_DetailScaleV2)).rgb , tex2D (_DetailMask, tex_Red).a);
	
	half3 albedo_Blue = _AlbedoColor3 * tex2D (_MainTex3, tex_Blue).rgb;
	//albedo_Blue *= LerpWhiteTo (tex2D (_DetailTex3, tex_Blue * float2(_DetailScaleU3,_DetailScaleV3)).rgb , tex2D (_DetailMask, tex_Red).a);

	half3 albedo = vColor.r * albedo_Red + vColor.g * albedo_Green + vColor.b * albedo_Blue;

	return albedo;
}



half3 AlbedoWithWetness(float2 tex_Red,float2 tex_Green,float2 tex_Blue, fixed4 vColor, fixed3 wetnessAlbedoModifier, fixed3 wetnessAlbedoModifier2, fixed3 wetnessAlbedoModifier3)
{


	half3 albedo_Red = _AlbedoColor * tex2D (_MainTex, tex_Red).rgb;
#if METALLIC
	albedo_Red *= LerpWhiteTo (tex2D (_DetailTex, tex_Red * float2(_DetailScaleU,_DetailScaleV)).rgb , tex2D (_DetailMask, tex_Red).a);
#endif
	half3 albedo_Green = _AlbedoColor2 * tex2D (_MainTex2, tex_Green).rgb;
	//albedo_Green *= LerpWhiteTo (tex2D (_DetailTex2, tex_Green * float2(_DetailScaleU2,_DetailScaleV2)).rgb , tex2D (_DetailMask, tex_Red).a);
	
	half3 albedo_Blue = _AlbedoColor3 * tex2D (_MainTex3, tex_Blue).rgb;
	//albedo_Blue *= LerpWhiteTo (tex2D (_DetailTex3, tex_Blue * float2(_DetailScaleU3,_DetailScaleV3)).rgb , tex2D (_DetailMask, tex_Red).a);

	half3 albedo = vColor.r * albedo_Red * wetnessAlbedoModifier + vColor.g * albedo_Green * wetnessAlbedoModifier2 + vColor.b * albedo_Blue * wetnessAlbedoModifier3;

	return albedo;
}




half3 NormalInTangentSpace(float2 tex_Red,float2 tex_Green,float2 tex_Blue, fixed4 vColor)
{


	half3 normalTangent_Red = UnpackScaleNormal(tex2D (_BumpMap, tex_Red), _NormalScale);
#if METALLIC
	normalTangent_Red = lerp(normalTangent_Red, 
							BlendNormals(normalTangent_Red, UnpackScaleNormal(tex2D (_DetailBumpMap, tex_Red * float2(_DetailScaleU,_DetailScaleV)), _DetailNormalScale)),
							tex2D (_DetailMask, tex_Red).a);
#endif


	half3 normalTangent_Green = UnpackScaleNormal(tex2D (_BumpMap2, tex_Green), _NormalScale2);
	//normalTangent_Green = lerp(normalTangent_Green, 
	//						BlendNormals(normalTangent_Green, UnpackScaleNormal(tex2D (_DetailBumpMap2, tex_Green * float2(_DetailScaleU2,_DetailScaleV2)), _DetailNormalScale2)),
	//						tex2D (_DetailMask2, tex_Green).a);
							
							
	half3 normalTangent_Blue = UnpackScaleNormal(tex2D (_BumpMap3, tex_Blue), _NormalScale3);
	//normalTangent_Blue = lerp(normalTangent_Blue, 
	//						BlendNormals(normalTangent_Blue, UnpackScaleNormal(tex2D (_DetailBumpMap3, tex_Blue * float2(_DetailScaleU3,_DetailScaleV3)), _DetailNormalScale3)),
	//						tex2D (_DetailMask3, tex_Blue).a);
							
							

	half3 normalTangent = vColor.r * normalTangent_Red
						+ vColor.g * normalTangent_Green
						+ vColor.b * normalTangent_Blue;

	return normalTangent;
}

#ifdef METALLIC
half2 MetallicGloss(float2 tex_Red,float2 tex_Green,float2 tex_Blue, fixed4 vColor)
{
	half2 mg;
	
	mg = vColor.r * half2(tex2D(_MainTex, tex_Red).a * _Metallic, tex2D(_CombinedMap, tex_Red).b * _Smoothness)
	   + vColor.g * half2(tex2D(_MainTex2, tex_Green).a * _Metallic2, tex2D(_CombinedMap2, tex_Green).b * _Smoothness2)
	   + vColor.b * half2(tex2D(_MainTex3, tex_Blue).a * _Metallic3, tex2D(_CombinedMap3, tex_Blue).b * _Smoothness3);

	return mg;
}



half2 MetallicGlossWithWetness(float2 tex_Red,float2 tex_Green,float2 tex_Blue, fixed4 vColor, fixed3 wetnessSmoothnessModifiers)
{
	half2 mg;
	
	mg = vColor.r * half2(tex2D(_MainTex, tex_Red).a * _Metallic,  wetnessSmoothnessModifiers.x)
	   + vColor.g * half2(tex2D(_MainTex2, tex_Green).a * _Metallic2,  wetnessSmoothnessModifiers.y)
	   + vColor.b * half2(tex2D(_MainTex3, tex_Blue).a * _Metallic3,  wetnessSmoothnessModifiers.z);

	return mg;
}
#endif

#ifdef SPECULAR
half4 SpecularGloss(float2 tex_Red,float2 tex_Green,float2 tex_Blue, fixed4 vColor)
{
	half4 sg;
	
	sg = vColor.r * half4(tex2D(_SpecularTex, tex_Red).rgb * _Specular, tex2D(_CombinedMap, tex_Red).b * _Smoothness)
	   + vColor.g * half4(tex2D(_SpecularTex2, tex_Green).rgb * _Specular2, tex2D(_CombinedMap2, tex_Green).b * _Smoothness2)
	   + vColor.b * half4(tex2D(_SpecularTex3, tex_Blue).rgb * _Specular3, tex2D(_CombinedMap3, tex_Blue).b * _Smoothness3);

	return sg;
}

half4 SpecularGlossWithWetness(float2 tex_Red,float2 tex_Green,float2 tex_Blue, fixed4 vColor, fixed3 wetnessSmoothnessModifiers)
{
	half4 sg;
	
	sg = vColor.r * half4(tex2D(_SpecularTex, tex_Red).rgb * _Specular, wetnessSmoothnessModifiers.x)
	   + vColor.g * half4(tex2D(_SpecularTex2, tex_Green).rgb * _Specular2, wetnessSmoothnessModifiers.y)
	   + vColor.b * half4(tex2D(_SpecularTex3, tex_Blue).rgb * _Specular3, wetnessSmoothnessModifiers.z);

	return sg;
}
#endif

half Occlusion(float2 tex_Red,float2 tex_Green,float2 tex_Blue, fixed4 vColor)
{

	return vColor.r * LerpOneTo(tex2D(_CombinedMap, tex_Red).g, _Occlusion)
		 + vColor.g * LerpOneTo(tex2D(_CombinedMap2, tex_Green).g, _Occlusion2)
		 + vColor.b * LerpOneTo(tex2D(_CombinedMap3, tex_Blue).g, _Occlusion3);

}

half3 Emission(float2 tex_Red,float2 tex_Green,float2 tex_Blue, fixed4 vColor)
{

	return vColor.r * tex2D(_CombinedMap, tex_Red).a * _EmissionColor.rgb
		 + vColor.g * tex2D(_CombinedMap2, tex_Green).a * _EmissionColor2.rgb
		 + vColor.b * tex2D(_CombinedMap3, tex_Blue).a * _EmissionColor3.rgb;
	
}




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



void doParallax(inout float2 tex_Red, inout float2 tex_Green, inout float2 tex_Blue, fixed4 colorFactor, float3 viewDir) {

	fixed2 uv_offset = float2(0,0);

	uv_offset =  calculateParallaxOffset(viewDir, tex_Red, _CombinedMap, _Parallax) * colorFactor.r;

	if( _useP1_2) {
		uv_offset += calculateParallaxOffset(viewDir, tex_Red, _CombinedMap, _Parallax2) * colorFactor.g;
	} else {
		uv_offset += colorFactor.g * calculateParallaxOffset(viewDir, tex_Green, _CombinedMap2, _Parallax2);
	}


	if( _useP1_3) {
		uv_offset += colorFactor.b * calculateParallaxOffset(viewDir, tex_Red, _CombinedMap, _Parallax3) ;
	} else {
		uv_offset += colorFactor.b * calculateParallaxOffset(viewDir, tex_Blue, _CombinedMap3, _Parallax3);
	}
	
						
	tex_Red += uv_offset;
	tex_Green += uv_offset;
	tex_Blue += uv_offset;

}


void calculateColorFactor(inout fixed4 colorFactor, float2 tex_Red, float2 tex_Green, float2 tex_Blue, float2 flowUV) {

fixed heightData = tex2D (_CombinedMap, tex_Red).r;

#if _FLOW_DRIFT
	float timeScale = _Time[1] * 0.25f;
	half2 phase = half2(frac(timeScale),frac(timeScale + 0.5));
    float flowLerp = abs((0.5f - phase.x) * 2);

#endif


#if _FLOW_DRIFT && _FLOW	
	float4 i_tex_Green_Phase = float4(tex_Green + flowUV * colorFactor.g * _FlowSpeed2 * phase.x,
								tex_Green + flowUV * colorFactor.g * _FlowSpeed2 * phase.y);
								
	fixed heightMapGreen = lerp(
							tex2D( _CombinedMap2, i_tex_Green_Phase.xy).r,
							tex2D( _CombinedMap2, i_tex_Green_Phase.zw).r,
							flowLerp);
								
#else
	fixed heightMapGreen = tex2D( _CombinedMap2, tex_Green).r;
#endif

#if _FLOW_DRIFT && _FLOW						
	float4 i_tex_Blue_Phase = float4(tex_Blue + flowUV * colorFactor.b * _FlowSpeed3 * phase.x,
								tex_Blue + flowUV * colorFactor.b * _FlowSpeed3 * phase.y);
								
	fixed heightMapBlue = lerp(
							tex2D( _CombinedMap3, i_tex_Blue_Phase.xy).r,
							tex2D( _CombinedMap3, i_tex_Blue_Phase.zw).r,
							flowLerp);

#else
	fixed heightMapBlue = tex2D( _CombinedMap3, tex_Blue).r;
#endif	

	


	half heightGreen = _BaseHeight2 + colorFactor.a * 2 - 1.0 + (heightMapGreen * 2 - 1.0) * _HeightmapFactor2 ;
	half heightBlue = _BaseHeight3 + colorFactor.a * 2 - 1.0 + (heightMapBlue * 2 - 1.0) * _HeightmapFactor3;


	//_BlendSmooth_Green += 0.1 * ( _BlendSmooth_Green + 0.001 );

	if( heightGreen <= heightData - _BlendSmooth2 || colorFactor.g == 0) {
		colorFactor.g = 0;
		
	} else if ( heightGreen > heightData - _BlendSmooth2 && heightGreen < heightData ) {
		colorFactor.g = colorFactor.g *  pow( (heightGreen - heightData + _BlendSmooth2) / _BlendSmooth2, 0.5);
	} 


	//_BlendSmooth3 += 0.1 * ( _BlendSmooth3 + 0.001 );

	if( heightBlue <= heightData - _BlendSmooth3 || colorFactor.b == 0) {
		colorFactor.b = 0;
	
	} else if ( heightBlue > heightData - _BlendSmooth3 && heightBlue < heightData ) {
		colorFactor.b = colorFactor.b * pow( (heightBlue - heightData + _BlendSmooth3) / _BlendSmooth3, 0.5);
	} 



#if _HEIGHTBASED_TRANSPARENCY
		colorFactor.g *= pow( saturate( heightGreen - heightData + _BlendSmooth2 ) , 0.5 * (_HeightBasedTransparency2+0.001));
		colorFactor.b *= pow( saturate( heightBlue - heightData + _BlendSmooth3) , 0.5 * (_HeightBasedTransparency3+0.001));
#endif



	colorFactor.r = 1.0 - colorFactor.g - colorFactor.b;


}


void calculateColorAndWetnessFactor(inout fixed4 colorFactor, float2 tex_Red, float2 tex_Green, float2 tex_Blue, float2 wetnessData, inout fixed3 wetnessAlbedoModifier, inout fixed3 wetnessAlbedoModifier2, inout fixed3 wetnessAlbedoModifier3, inout fixed3 wetnessSmoothnessModifiers) {




	fixed heightData = tex2D (_CombinedMap, tex_Red).r;
	fixed heightMapGreen = tex2D( _CombinedMap2, tex_Green).r;
	fixed heightMapBlue = tex2D( _CombinedMap3, tex_Blue).r;

	


	half heightGreen = _BaseHeight2 + colorFactor.a * 2 - 1.0 + (heightMapGreen * 2 - 1.0) * _HeightmapFactor2 ;
	half heightBlue = _BaseHeight3 + colorFactor.a * 2 - 1.0 + (heightMapBlue * 2 - 1.0) * _HeightmapFactor3;


	//_BlendSmooth_Green += 0.1 * ( _BlendSmooth_Green + 0.001 );

	if( heightGreen <= heightData - _BlendSmooth2 || colorFactor.g == 0) {
		colorFactor.g = 0;
		
	} else if ( heightGreen > heightData - _BlendSmooth2 && heightGreen < heightData ) {
		colorFactor.g = colorFactor.g *  pow( (heightGreen - heightData + _BlendSmooth2) / _BlendSmooth2, 0.5);
	} 


	//_BlendSmooth3 += 0.1 * ( _BlendSmooth3 + 0.001 );

	if( heightBlue <= heightData - _BlendSmooth3 || colorFactor.b == 0) {
		colorFactor.b = 0;
	
	} else if ( heightBlue > heightData - _BlendSmooth3 && heightBlue < heightData ) {
		colorFactor.b = colorFactor.b * pow( (heightBlue - heightData + _BlendSmooth3) / _BlendSmooth3, 0.5);
	} 



#if _HEIGHTBASED_TRANSPARENCY
		colorFactor.g *= pow( saturate( heightGreen - heightData + _BlendSmooth2 ) , 0.5 * (_HeightBasedTransparency2+0.001));
		colorFactor.b *= pow( saturate( heightBlue - heightData + _BlendSmooth3) , 0.5 * (_HeightBasedTransparency3+0.001));
#endif

	colorFactor.r = 1.0 - colorFactor.g - colorFactor.b;
	
	
	fixed wetness = wetnessData.x;
	fixed wetnessHeight = wetnessData.y * 2 - 0.98;
	
	float wetnessHeightData = heightData * colorFactor.r + heightGreen * colorFactor.g + heightBlue * colorFactor.b;


	
	if ( wetnessHeight > wetnessHeightData - _wetnessEdgeBlend && wetnessHeight < wetnessHeightData ) {
		wetness = wetness * pow( (wetnessHeight - wetnessHeightData + _wetnessEdgeBlend) / _wetnessEdgeBlend, 0.5);

		wetnessAlbedoModifier = lerp( float3(1,1,1), _wetnessAlbedoModifier, wetness);
		wetnessAlbedoModifier2 = lerp( float3(1,1,1), _wetnessAlbedoModifier2, wetness);
		wetnessAlbedoModifier3 = lerp( float3(1,1,1), _wetnessAlbedoModifier3, wetness);

		wetnessSmoothnessModifiers.x = lerp(tex2D (_CombinedMap, tex_Red).b * _Smoothness,_wetnessSmoothnessModifier,wetness);
		wetnessSmoothnessModifiers.y = lerp(tex2D (_CombinedMap2, tex_Green).b * _Smoothness2,_wetnessSmoothnessModifier2,wetness);
		wetnessSmoothnessModifiers.z = lerp(tex2D (_CombinedMap3, tex_Blue).b * _Smoothness3,_wetnessSmoothnessModifier3,wetness);

		_NormalScale = lerp(_NormalScale,1-_wetnessNormalModifier,wetness);
		_NormalScale2 = lerp(_NormalScale,1-_wetnessNormalModifier2,wetness);
		_NormalScale3 = lerp(_NormalScale,1-_wetnessNormalModifier3,wetness);


	} else if( wetnessHeightData >= wetnessHeight  ) {

		wetnessAlbedoModifier = float3(1,1,1);
		wetnessAlbedoModifier2 = float3(1,1,1);
		wetnessAlbedoModifier3 = float3(1,1,1);
		wetnessSmoothnessModifiers.x = tex2D (_CombinedMap, tex_Red).b * _Smoothness;
		wetnessSmoothnessModifiers.y = tex2D (_CombinedMap2, tex_Green).b * _Smoothness2;
		wetnessSmoothnessModifiers.z = tex2D (_CombinedMap3, tex_Blue).b * _Smoothness3;
		//_NormalScale = 1;
		//_NormalScale2 = 1;
		//_NormalScale3 = 1;

	} else {

		wetnessAlbedoModifier = lerp( float3(1,1,1), _wetnessAlbedoModifier, wetness);
		wetnessAlbedoModifier2 = lerp( float3(1,1,1), _wetnessAlbedoModifier2, wetness);
		wetnessAlbedoModifier3 = lerp( float3(1,1,1), _wetnessAlbedoModifier3, wetness);

		wetnessSmoothnessModifiers.x = lerp(tex2D (_CombinedMap, tex_Red).b * _Smoothness,_wetnessSmoothnessModifier,wetness);
		wetnessSmoothnessModifiers.y = lerp(tex2D (_CombinedMap2, tex_Green).b * _Smoothness2,_wetnessSmoothnessModifier2,wetness);
		wetnessSmoothnessModifiers.z = lerp(tex2D (_CombinedMap3, tex_Blue).b * _Smoothness3,_wetnessSmoothnessModifier3,wetness);

		_NormalScale = lerp(_NormalScale,1-_wetnessNormalModifier,wetness);
		_NormalScale2 = lerp(_NormalScale,1-_wetnessNormalModifier2,wetness);
		_NormalScale3 = lerp(_NormalScale,1-_wetnessNormalModifier3,wetness);

	}


}

#ifdef TESSELATION
float3 computeNormals( float h_A, float h_B, float h_C, float h_D, float h_N, float heightScale ) {
    //To make it easier we offset the points such that n is "0" height
    float3 va = { 0, 1, (h_A - h_N)*heightScale };
    float3 vb = { 1, 0, (h_B - h_N)*heightScale };
    float3 vc = { 0, -1, (h_C - h_N)*heightScale };
    float3 vd = { -1, 0, (h_D - h_N)*heightScale };
    //cross products of each vector yields the normal of each tri - return the average normal of all 4 tris
    float3 average_n = ( cross(va, vb) + cross(vb, vc) + cross(vc, vd) + cross(vd, va) ) / -4;
    return normalize( average_n );
}


half3 calculateColorFactorWithHeightReturn(inout fixed4 colorFactor, float4 tex_Red, float4 tex_Green, float4 tex_Blue, float2 flowUV) {

fixed heightData = tex2Dlod (_CombinedMap, tex_Red).r;

#if _FLOW_DRIFT
	float timeScale = _Time[1] * 0.25f;
	half2 phase = half2(frac(timeScale),frac(timeScale + 0.5));
    float flowLerp = abs((0.5f - phase.x) * 2);

#endif


#if _FLOW_DRIFT && _FLOW	
	float4 i_tex_Green_Phase = float4(tex_Green + flowUV * colorFactor.g * _FlowSpeed2 * phase.x,
								tex_Green + flowUV * colorFactor.g * _FlowSpeed2 * phase.y);
								
	fixed heightMapGreen = lerp(
							tex2Dlod( _CombinedMap2, float4(i_tex_Green_Phase.xy,0,0)).r,
							tex2Dlod( _CombinedMap2, float4(i_tex_Green_Phase.zw,0,0)).r,
							flowLerp);
								
#else
	fixed heightMapGreen = tex2Dlod( _CombinedMap2, tex_Green).r;
#endif

#if _FLOW_DRIFT && _FLOW						
	float4 i_tex_Blue_Phase = float4(tex_Blue + flowUV * colorFactor.b * _FlowSpeed3 * phase.x,
								tex_Blue + flowUV * colorFactor.b * _FlowSpeed3 * phase.y);
								
	fixed heightMapBlue = lerp(
							tex2Dlod( _CombinedMap3, float4(i_tex_Blue_Phase.xy,0,0)).r,
							tex2Dlod( _CombinedMap3, float4(i_tex_Blue_Phase.zw,0,0)).r,
							flowLerp);

#else
	fixed heightMapBlue = tex2Dlod( _CombinedMap3, tex_Blue).r;
#endif	

	


	half heightGreen = _BaseHeight2 + colorFactor.a * 2 - 1.0 + (heightMapGreen * 2 - 1.0) * _HeightmapFactor2 ;
	half heightBlue = _BaseHeight3 + colorFactor.a * 2 - 1.0 + (heightMapBlue * 2 - 1.0) * _HeightmapFactor3;


	//_BlendSmooth_Green += 0.1 * ( _BlendSmooth_Green + 0.001 );

	if( heightGreen <= heightData - _BlendSmooth2 || colorFactor.g == 0) {
		colorFactor.g = 0;
		
	} else if ( heightGreen > heightData - _BlendSmooth2 && heightGreen < heightData ) {
		colorFactor.g = colorFactor.g *  pow( (heightGreen - heightData + _BlendSmooth2) / _BlendSmooth2, 0.5);
	} 


	//_BlendSmooth3 += 0.1 * ( _BlendSmooth3 + 0.001 );

	if( heightBlue <= heightData - _BlendSmooth3 || colorFactor.b == 0) {
		colorFactor.b = 0;
	
	} else if ( heightBlue > heightData - _BlendSmooth3 && heightBlue < heightData ) {
		colorFactor.b = colorFactor.b * pow( (heightBlue - heightData + _BlendSmooth3) / _BlendSmooth3, 0.5);
	} 



#if _HEIGHTBASED_TRANSPARENCY
		colorFactor.g *= pow( saturate( heightGreen - heightData + _BlendSmooth2 ) , 0.5 * (_HeightBasedTransparency2+0.001));
		colorFactor.b *= pow( saturate( heightBlue - heightData + _BlendSmooth3) , 0.5 * (_HeightBasedTransparency3+0.001));
#endif



	colorFactor.r = 1.0 - colorFactor.g - colorFactor.b;

	return half3(heightData, heightGreen, heightBlue);

}


#endif


#endif

