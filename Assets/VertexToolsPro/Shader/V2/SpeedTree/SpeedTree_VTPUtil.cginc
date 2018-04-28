#ifndef SpeedTree_VTPUtil_INCLUDED
#define SpeedTree_VTPUtil_INCLUDED


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

	//half3 albedo = vColor.r * albedo_Red + vColor.g * albedo_Green + vColor.b * albedo_Blue;
	half3 albedo = lerp(lerp(albedo_Red, albedo_Green, vColor.g), albedo_Blue, vColor.b);
	albedo *= vColor.r;
	return albedo;
}


//TODO: Implemented vColor.r as Occlusion everywhere



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
							
							



	half3 normalTangent = lerp( lerp( normalTangent_Red, normalTangent_Green, vColor.g), normalTangent_Blue, vColor.b);

	return normalTangent;
}

#ifdef METALLIC
half2 MetallicGloss(float2 tex_Red,float2 tex_Green,float2 tex_Blue, fixed4 vColor)
{
	half2 mg;

	mg = half2(
		lerp( lerp( tex2D(_MainTex, tex_Red).a * _Metallic, tex2D(_MainTex2, tex_Green).a * _Metallic2, vColor.g), tex2D(_MainTex3, tex_Blue).a * _Metallic3, vColor.b),
		lerp( lerp( tex2D(_CombinedMap, tex_Red).b * _Smoothness, tex2D(_CombinedMap2, tex_Green).b * _Smoothness2, vColor.g), tex2D(_CombinedMap3, tex_Blue).b * _Smoothness3, vColor.b)
	);

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

#endif

half Occlusion(float2 tex_Red,float2 tex_Green,float2 tex_Blue, fixed4 vColor)
{

	return lerp( lerp( LerpOneTo(tex2D(_CombinedMap, tex_Red).g, _Occlusion), LerpOneTo(tex2D(_CombinedMap2, tex_Green).g, _Occlusion2), vColor.g), LerpOneTo(tex2D(_CombinedMap3, tex_Blue).g, _Occlusion3), vColor.b);


}

half3 Emission(float2 tex_Red,float2 tex_Green,float2 tex_Blue, fixed4 vColor)
{

	return lerp( lerp( tex2D(_CombinedMap, tex_Red).a * _EmissionColor.rgb, tex2D(_CombinedMap2, tex_Green).a * _EmissionColor2.rgb, vColor.g), tex2D(_CombinedMap3, tex_Blue).a * _EmissionColor3.rgb, vColor.b);


}




#ifdef ENABLE_WIND
float3 calculateWindAffectionsBranch(appdata_full v, float lodValue) {

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


#endif


#ifdef _HEIGHTBASED_BLENDING
void calculateColorFactor(inout fixed4 colorFactor, float2 tex_Red, float2 tex_Green, float2 tex_Blue) {

	fixed heightData = tex2D (_CombinedMap, tex_Red).r;
	fixed heightMapGreen = tex2D( _CombinedMap2, tex_Green).r;
	fixed heightMapBlue = tex2D( _CombinedMap3, tex_Blue).r;


		


		half heightGreen = _BaseHeight2 + colorFactor.a * 2 - 1.0 + (heightMapGreen * 2 - 1.0) * _HeightmapFactor2 ;
		half heightBlue = _BaseHeight3 + colorFactor.a * 2 - 1.0 + (heightMapBlue * 2 - 1.0) * _HeightmapFactor3;


		if( heightGreen <= heightData - _BlendSmooth2 || colorFactor.g == 0) {
			colorFactor.g = 0;
			
		} else if ( heightGreen > heightData - _BlendSmooth2 && heightGreen < heightData ) {
			colorFactor.g = colorFactor.g *  pow( (heightGreen - heightData + _BlendSmooth2) / _BlendSmooth2, 0.5);
		} 


		if( heightBlue <= heightData - _BlendSmooth3 || colorFactor.b == 0) {
			colorFactor.b = 0;
		
		} else if ( heightBlue > heightData - _BlendSmooth3 && heightBlue < heightData ) {
			colorFactor.b = colorFactor.b * pow( (heightBlue - heightData + _BlendSmooth3) / _BlendSmooth3, 0.5);
		} 


	/*
	#if _HEIGHTBASED_TRANSPARENCY
			colorFactor.g *= pow( saturate( heightGreen - heightData + _BlendSmooth2 ) , 0.5 * (_HeightBasedTransparency2+0.001));
			colorFactor.b *= pow( saturate( heightBlue - heightData + _BlendSmooth3) , 0.5 * (_HeightBasedTransparency3+0.001));
	#endif
	*/




}
#endif



#ifdef _HEIGHTBASED_BLENDING

half3 calculateColorFactorWithHeightReturn(inout fixed4 colorFactor, float4 tex_Red, float4 tex_Green, float4 tex_Blue) {

	fixed heightData = tex2Dlod (_CombinedMap, tex_Red).r;
	fixed heightMapGreen = tex2Dlod( _CombinedMap2, tex_Green).r;
	fixed heightMapBlue = tex2Dlod( _CombinedMap3, tex_Blue).r;


	half heightGreen = _BaseHeight2 + colorFactor.a * 2 - 1.0 + (heightMapGreen * 2 - 1.0) * _HeightmapFactor2 ;
	half heightBlue = _BaseHeight3 + colorFactor.a * 2 - 1.0 + (heightMapBlue * 2 - 1.0) * _HeightmapFactor3;



	if( heightGreen <= heightData - _BlendSmooth2 || colorFactor.g == 0) {
		colorFactor.g = 0;
		
	} else if ( heightGreen > heightData - _BlendSmooth2 && heightGreen < heightData ) {
		colorFactor.g = colorFactor.g *  pow( (heightGreen - heightData + _BlendSmooth2) / _BlendSmooth2, 0.5);
	} 


	if( heightBlue <= heightData - _BlendSmooth3 || colorFactor.b == 0) {
		colorFactor.b = 0;
	
	} else if ( heightBlue > heightData - _BlendSmooth3 && heightBlue < heightData ) {
		colorFactor.b = colorFactor.b * pow( (heightBlue - heightData + _BlendSmooth3) / _BlendSmooth3, 0.5);
	} 


	/*
	#if _HEIGHTBASED_TRANSPARENCY
			colorFactor.g *= pow( saturate( heightGreen - heightData + _BlendSmooth2 ) , 0.5 * (_HeightBasedTransparency2+0.001));
			colorFactor.b *= pow( saturate( heightBlue - heightData + _BlendSmooth3) , 0.5 * (_HeightBasedTransparency3+0.001));
	#endif
	*/


	return half3(heightData, heightGreen, heightBlue);

}
#endif


#endif

