float3 calculateWindAffections(appdata_full v, float lodValue) {

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


#ifdef GEOM_TYPE_LEAF
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
		#endif