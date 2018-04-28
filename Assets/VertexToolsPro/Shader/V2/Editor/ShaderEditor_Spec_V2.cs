using UnityEngine;
using System;
using UnityEditor;

public class ShaderEditor_Spec_V2 : ShaderGUI
{
	[SerializeField]
	MaterialProperty[] matProperties;
	[SerializeField]
	private MaterialEditor matEditor;
	[SerializeField]
	private Material targetMat;

	private GUIStyle headerBoxStyle;
	private GUIStyle bodyH1Style;
	private GUIStyle bodyH2Style;
	private GUIStyle neutralGUIStyle;
	private GUIStyle redChannelGUIStyle;
	private GUIStyle greenChannelGUIStyle;
	private GUIStyle blueChannelGUIStyle;

	[SerializeField]
	private bool showGeneralSettings = false;


	[SerializeField]
	private bool useHeightbasedBlending = false;
	[SerializeField]
	private bool useWetness = false;
	[SerializeField]
	private bool useFlow = false;
	[SerializeField]
	private bool useDrift = false;
	[SerializeField]
	private bool _flowNormals = false;




	//[SerializeField]
	//private bool useFlowMapGlobal = false;
	//[SerializeField]
	//private bool flowOnlyNormals = false;

	[SerializeField]
	private bool showParallaxSetting = false;
	[SerializeField]
	private bool useParallax = true;
	[SerializeField]
	private int parallaxInterpolation = 20;

	[SerializeField]
	private bool useDepthBias = false;
	[SerializeField]
	private float depthBiasDistance = 50f;
	[SerializeField]
	private float depthBiasPower = 2f;
	[SerializeField]
	private float depthBiasThreshold = 0f;

	[SerializeField]
	private bool showRedChannel = false;
	[SerializeField]
	private bool showWetnessRed = false;
	[SerializeField]
	private bool showDetailRed = false;



	[SerializeField]
	private bool showGreenChannel = false;
	[SerializeField]
	private bool showParallaxGreen = true;
	[SerializeField]
	private bool useParallaxOfR2 = false;
	[SerializeField]
	private bool showHeightBasedGreen = true;

	[SerializeField]
	private bool showFlowMapGreen = true;
	[SerializeField]
	private bool showWetnessGreen = false;
	//[SerializeField]
	//private bool showDetailGreen = false;
	//[SerializeField]
	//private bool useFlowMap2 = false;
	//[SerializeField]
	//private bool useDrift2 = false;



	[SerializeField]
	private bool showBlueChannel = false;
	[SerializeField]
	private bool showParallaxBlue = true;
	[SerializeField]
	private bool useParallaxOfR3 = false;
	[SerializeField]
	private bool showHeightBasedBlue = true;

	[SerializeField]
	private bool showFlowMapBlue = true;
	[SerializeField]
	private bool showWetnessBlue = false;
	//[SerializeField]
	//private bool showDetailBlue = false;
	//[SerializeField]
	//private bool useFlowMap3 = false;
	//[SerializeField]
	//private bool useDrift3 = false;



	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{

		matProperties = properties;
		matEditor = materialEditor;

		//base.OnGUI(matEditor, properties);
		targetMat = matEditor.target as Material;
	

		generateStyles ();

		GUILayout.BeginVertical (neutralGUIStyle);
		guiForGeneral ();
		GUILayout.EndVertical ();

		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

		GUILayout.BeginVertical (neutralGUIStyle);
		guiForParallax ();
		GUILayout.EndVertical ();

		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

		GUILayout.BeginVertical (redChannelGUIStyle);
		guiForRedChannel ();
		GUILayout.EndVertical ();

		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

		GUILayout.BeginVertical (greenChannelGUIStyle);
		guiForGreenChannel ();
		GUILayout.EndVertical ();

		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

		GUILayout.BeginVertical (blueChannelGUIStyle);
		guiForBlueChannel ();
		GUILayout.EndVertical ();

		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

		shaderKeywords ();

		// render the default gui
		//base.OnGUI(materialEditor, properties);

		/*Material targetMat = materialEditor.target as Material;

		// see if redify is set, and show a checkbox
		bool redify = Array.IndexOf(targetMat.shaderKeywords, "REDIFY_ON") != -1;
		EditorGUI.BeginChangeCheck();
		redify = EditorGUILayout.Toggle("Redify material", redify);
		if (EditorGUI.EndChangeCheck())
		{
			// enable or disable the keyword based on checkbox
			if (redify)
				targetMat.EnableKeyword("REDIFY_ON");
			else
				targetMat.DisableKeyword("REDIFY_ON");
		}

		*/

	}

	void shaderKeywords () {

		if (getProperty ("_HeightBasedTransparency2").floatValue > 0.01
		    || getProperty ("_HeightBasedTransparency3").floatValue > 0.01) {
			targetMat.EnableKeyword ("_HEIGHTBASED_TRANSPARENCY");
		} else {
			targetMat.DisableKeyword ("_HEIGHTBASED_TRANSPARENCY");
		}

		if (getProperty ("_EmissionColor").colorValue.maxColorComponent > 0.01
			|| getProperty ("_EmissionColor2").colorValue.maxColorComponent > 0.01
			|| getProperty ("_EmissionColor3").colorValue.maxColorComponent > 0.01) {

			targetMat.EnableKeyword ("_VTP_EMISSION");

		} else {
			targetMat.DisableKeyword ("_VTP_EMISSION");
		}

	}

	MaterialProperty getProperty( string propertyName ) {

		foreach (MaterialProperty matProp in matProperties) {

			if (matProp.name == propertyName)
				return matProp;

		}

		return null;

	}


	void guiForRedChannel() {


		showRedChannel = targetMat.GetFloat("_showRed") == 1;

		showRedChannel = EditorGUILayout.Foldout(showRedChannel, "Red Channel Settings");

		targetMat.SetFloat ("_showRed", showRedChannel ? 1f : 0f );

		if (!showRedChannel)
			return;

		matEditor.TextureProperty (getProperty ("_MainTex"), "Albedo Map");
		matEditor.ColorProperty ( getProperty ("_AlbedoColor"), "Albedo Color");
		matEditor.TextureProperty (getProperty ("_SpecularTex"), "Specular Map");
		matEditor.RangeProperty (getProperty ("_Specular"), "Specular Strength");

		matEditor.TextureProperty (getProperty ("_BumpMap"), "Normal Map");
		matEditor.RangeProperty (getProperty ("_NormalScale"), "Normal Scale");

		matEditor.TextureProperty (getProperty ("_CombinedMap"), "Combined Map");

		matEditor.RangeProperty (getProperty ("_Smoothness"), "Smoothness Strength");
		matEditor.RangeProperty (getProperty ("_Occlusion"), "Occlusion Strength Strength");

		matEditor.RangeProperty ( getProperty ("_Emission"), "Emission Strength");

		matEditor.ColorProperty ( getProperty ("_EmissionColor"), "Emission Color");

		if (useParallax) {

			matEditor.RangeProperty (getProperty ("_Parallax"), "Parallax Strength");

		}

		if (useWetness) {
			showWetnessRed = EditorGUILayout.Foldout (showWetnessRed, "Wetness Settings");
			if (showWetnessRed) {
				matEditor.ColorProperty (getProperty ("_wetnessAlbedoModifier"), "Wetness Color");
				matEditor.RangeProperty (getProperty ("_wetnessSmoothnessModifier"), "Wetness Smoothness");
				matEditor.RangeProperty (getProperty ("_wetnessNormalModifier"), "Flatten Normals");
			}
		}

		showDetailRed = EditorGUILayout.Foldout (showDetailRed, "Detail Maps");
		if (showDetailRed) {
			matEditor.TexturePropertySingleLine (new GUIContent ("Detail Mask"), getProperty ("_DetailMask"));
			matEditor.TexturePropertySingleLine (new GUIContent ("Detail Albedo"), getProperty ("_DetailTex"), getProperty ("_DetailScaleU"), getProperty ("_DetailScaleV"));
			matEditor.TexturePropertySingleLine (new GUIContent ("Detail Normal"), getProperty ("_DetailBumpMap"), getProperty ("_DetailNormalScale"));
		}

	}

	void guiForGreenChannel() {

		showGreenChannel = targetMat.GetFloat("_showGreen") == 1;
		showGreenChannel = EditorGUILayout.Foldout(showGreenChannel, "Green Channel Settings");
		targetMat.SetFloat ("_showGreen", showGreenChannel ? 1f : 0f );

		if (!showGreenChannel)
			return;

		matEditor.TextureProperty (getProperty ("_MainTex2"), "Albedo Map");
		matEditor.ColorProperty ( getProperty ("_AlbedoColor2"), "Albedo Color");
		matEditor.TextureProperty (getProperty ("_SpecularTex2"), "Specular Map");
		matEditor.RangeProperty (getProperty ("_Specular2"), "Specular Strength");

		matEditor.TextureProperty (getProperty ("_BumpMap2"), "Normal Map");
		matEditor.RangeProperty (getProperty ("_NormalScale2"), "Normal Scale");
		matEditor.TextureProperty (getProperty ("_CombinedMap2"), "Combined Map");

		matEditor.RangeProperty (getProperty ("_Smoothness2"), "Smoothness Strength");
		matEditor.RangeProperty (getProperty ("_Occlusion2"), "Occlusion Strength Strength");


		matEditor.RangeProperty ( getProperty ("_Emission2"), "Emission Strength");
		matEditor.ColorProperty ( getProperty ("_EmissionColor2"), "Emission Color");

		if (useParallax) {
			
			showParallaxGreen = EditorGUILayout.Foldout (showParallaxGreen, "Parallax Settings");

			if (showParallaxGreen) {

				matEditor.RangeProperty (getProperty ("_Parallax2"), "Parallax Strength");

				useParallaxOfR2 = targetMat.GetFloat("_useP1_2") == 1;
				useParallaxOfR2 = EditorGUILayout.Toggle ("Use Base Heightmap", useParallaxOfR2);
				targetMat.SetFloat ("_useP1_2", useParallaxOfR2 ? 1f : 0f );

			}

		}



		if (useHeightbasedBlending) {

			showHeightBasedGreen = EditorGUILayout.Foldout (showHeightBasedGreen, "Heightbased Blending Settings");

			if (showHeightBasedGreen) {
				
				matEditor.RangeProperty (getProperty ("_BaseHeight2"), "Base Height of Layer");
				matEditor.RangeProperty (getProperty ("_HeightmapFactor2"), "HeightMap Multiplier");
				matEditor.RangeProperty (getProperty ("_BlendSmooth2"), "Edge Blend Strength");
				matEditor.RangeProperty (getProperty ("_HeightBasedTransparency2"), "Height-Based Transparency");

			}

		}


		if (useFlow) {
			showFlowMapGreen = EditorGUILayout.Foldout (showFlowMapGreen, "FlowMap Settings");

			if (showFlowMapGreen) {

				matEditor.FloatProperty (getProperty ("_FlowSpeed2"), "Flow Speed");

			}
		}

		if (useWetness) {
			showWetnessGreen = EditorGUILayout.Foldout (showWetnessGreen, "Wetness Settings");
			if (showWetnessGreen) {
				matEditor.ColorProperty (getProperty ("_wetnessAlbedoModifier2"), "Wetness Color");
				matEditor.RangeProperty (getProperty ("_wetnessSmoothnessModifier2"), "Wetness Smoothness");
				matEditor.RangeProperty (getProperty ("_wetnessNormalModifier2"), "Flatten Normals");
			}
		}

		/*
		showDetailGreen = EditorGUILayout.Foldout (showDetailGreen, "Detail Maps");
		if (showDetailGreen) {
			matEditor.TexturePropertySingleLine (new GUIContent ("Detail Mask"), getProperty ("_DetailMask2"));
			matEditor.TexturePropertySingleLine (new GUIContent ("Detail Albedo"), getProperty ("_DetailTex2"), getProperty ("_DetailScaleU2"), getProperty ("_DetailScaleV2"));
			matEditor.TexturePropertySingleLine (new GUIContent ("Detail Normal"), getProperty ("_DetailBumpMap2"), getProperty ("_DetailNormalScale2"));
		}
		*/


	}

	void guiForBlueChannel() {

		showBlueChannel = targetMat.GetFloat("_showBlue") == 1;
		showBlueChannel = EditorGUILayout.Foldout(showBlueChannel, "Blue Channel Settings");
		targetMat.SetFloat ("_showBlue", showBlueChannel ? 1f : 0f );

		if (!showBlueChannel)
			return;

		matEditor.TextureProperty (getProperty ("_MainTex3"), "Albedo Map");
		matEditor.ColorProperty ( getProperty ("_AlbedoColor3"), "Albedo Color");
		matEditor.TextureProperty (getProperty ("_SpecularTex3"), "Specular Map");
		matEditor.RangeProperty (getProperty ("_Specular3"), "Specular Strength");

		matEditor.TextureProperty (getProperty ("_BumpMap3"), "Normal Map");
		matEditor.RangeProperty (getProperty ("_NormalScale3"), "Normal Scale");
		matEditor.TextureProperty (getProperty ("_CombinedMap3"), "Combined Map");

		matEditor.RangeProperty (getProperty ("_Smoothness3"), "Smoothness Strength");
		matEditor.RangeProperty (getProperty ("_Occlusion3"), "Occlusion Strength Strength");


		matEditor.RangeProperty ( getProperty ("_Emission3"), "Emission Strength");
		matEditor.ColorProperty ( getProperty ("_EmissionColor3"), "Emission Color");

		if (useParallax) {

			showParallaxBlue = EditorGUILayout.Foldout (showParallaxBlue, "Parallax Settings");

			if (showParallaxBlue) {

				matEditor.RangeProperty (getProperty ("_Parallax3"), "Parallax Strength");

				useParallaxOfR3 = targetMat.GetFloat("_useP1_3") == 1;
				useParallaxOfR3 = EditorGUILayout.Toggle ("Use Base Heightmap", useParallaxOfR3);
				targetMat.SetFloat ("_useP1_3", useParallaxOfR3 ? 1f : 0f );

			}

		}



		if (useHeightbasedBlending) {

			showHeightBasedBlue = EditorGUILayout.Foldout (showHeightBasedBlue, "Heightbased Blending Settings");

			if (showHeightBasedBlue) {

				matEditor.RangeProperty (getProperty ("_BaseHeight3"), "Base Height of Layer");
				matEditor.RangeProperty (getProperty ("_HeightmapFactor3"), "HeightMap Multiplier");
				matEditor.RangeProperty (getProperty ("_BlendSmooth3"), "Edge Blend Strength");
				matEditor.RangeProperty (getProperty ("_HeightBasedTransparency3"), "Height-Based Transparency");

			}

		}


		if (useFlow) {
			showFlowMapBlue = EditorGUILayout.Foldout (showFlowMapBlue, "FlowMap Settings");

			if (showFlowMapBlue) {

				matEditor.FloatProperty (getProperty ("_FlowSpeed3"), "Flow Speed");

			}
		}

		if (useWetness) {
			showWetnessBlue = EditorGUILayout.Foldout (showWetnessBlue, "Wetness Settings");
			if (showWetnessBlue) {
				matEditor.ColorProperty (getProperty ("_wetnessAlbedoModifier3"), "Wetness Color");
				matEditor.RangeProperty (getProperty ("_wetnessSmoothnessModifier3"), "Wetness Smoothness");
				matEditor.RangeProperty (getProperty ("_wetnessNormalModifier3"), "Flatten Normals");
			}
		}

		/*
		showDetailBlue = EditorGUILayout.Foldout (showDetailBlue, "Detail Maps");
		if (showDetailBlue) {
			matEditor.TexturePropertySingleLine (new GUIContent ("Detail Mask"), getProperty ("_DetailMask3"));
			matEditor.TexturePropertySingleLine (new GUIContent ("Detail Albedo"), getProperty ("_DetailTex3"), getProperty ("_DetailScaleU3"), getProperty ("_DetailScaleV3"));
			matEditor.TexturePropertySingleLine (new GUIContent ("Detail Normal"), getProperty ("_DetailBumpMap3"), getProperty ("_DetailNormalScale3"));
		}

*/


	}



	void guiForGeneral() {

		showGeneralSettings = targetMat.GetFloat("_showGeneral") == 1;
		showGeneralSettings = EditorGUILayout.Foldout(showGeneralSettings, "General Settings");
		targetMat.SetFloat ("_showGeneral", showGeneralSettings ? 1f : 0f );

		useHeightbasedBlending = targetMat.GetFloat("_useHeightBasedBlending") == 1;

		if (!showGeneralSettings)
			return;




		useHeightbasedBlending = EditorGUILayout.Toggle ("Heightbased Blending", useHeightbasedBlending);
		targetMat.SetFloat ("_useHeightBasedBlending", useHeightbasedBlending ? 1f : 0f );
		if( useHeightbasedBlending )
			targetMat.EnableKeyword ("_HEIGHTBASED_BLENDING");
		else
			targetMat.DisableKeyword ("_HEIGHTBASED_BLENDING");


		if (targetMat.shader.name == "VTP/V2/Mobile/Specular")
			return;

		if (!useFlow) {

			useWetness = targetMat.GetFloat ("_useWetness") == 1;
			useWetness = EditorGUILayout.Toggle ("Use Wetness", useWetness);
			targetMat.SetFloat ("_useWetness", useWetness ? 1f : 0f);
			if (useWetness) {
				targetMat.EnableKeyword ("_WETNESS");
				useFlow = false;
				targetMat.SetFloat ("_useFlow", 0);
			} else {
				targetMat.DisableKeyword ("_WETNESS");
			}

			if( useWetness ) {

				matEditor.RangeProperty (getProperty ("_wetnessEdgeBlend"), "Wetness Edge Blend");
			}

		}


		if (!useWetness) {

			useFlow = targetMat.GetFloat ("_useFlow") == 1;
			useFlow = EditorGUILayout.Toggle ("Use Flow", useFlow);
			targetMat.SetFloat ("_useFlow", useFlow ? 1f : 0f);
			if (useFlow) {
				targetMat.EnableKeyword ("_FLOW");
				useWetness = false;
				targetMat.SetFloat ("_useWetness", 0);
			} else {
				targetMat.DisableKeyword ("_FLOW");
			}


			if (useFlow) {


				useDrift = targetMat.GetFloat ("_useDrift") == 1;
				useDrift = EditorGUILayout.Toggle ("Use Height Drift", useDrift);
				targetMat.SetFloat ("_useDrift", useDrift ? 1f : 0f);
				if (useDrift)
					targetMat.EnableKeyword ("_FLOW_DRIFT");
				else
					targetMat.DisableKeyword ("_FLOW_DRIFT");


				_flowNormals = targetMat.GetFloat ("_flowNormals") == 1;
				_flowNormals = EditorGUILayout.Toggle ("Only flow Normals", _flowNormals);
				targetMat.SetFloat ("_flowNormals", _flowNormals ? 1f : 0f);
				if (_flowNormals)
					targetMat.EnableKeyword ("_FLOW_NORMALS");
				else
					targetMat.DisableKeyword ("_FLOW_NORMALS");
			}
		}




	}

	void guiForParallax() {

	
		showParallaxSetting = targetMat.GetFloat("_showParallax") == 1;
		showParallaxSetting = EditorGUILayout.Foldout(showParallaxSetting, "Parallax Settings");
		targetMat.SetFloat ("_showParallax", showParallaxSetting ? 1f : 0f );

		useParallax = targetMat.GetFloat("_useParallax") == 1;

		if (!showParallaxSetting)
			return;

		useParallax = EditorGUILayout.Toggle ("Use Parallax", useParallax);
		targetMat.SetFloat ("_useParallax", useParallax ? 1f : 0f );
		if( useParallax )
			targetMat.EnableKeyword ("_PARALLAX");
		else
			targetMat.DisableKeyword ("_PARALLAX");

		if (!useParallax)
			return;

		EditorGUILayout.BeginHorizontal ();
		{
			parallaxInterpolation = targetMat.GetInt ("_ParallaxInterpolation");
			EditorGUILayout.LabelField ("Parallax interpolations", GUILayout.Width (150));
			parallaxInterpolation = EditorGUILayout.IntSlider (parallaxInterpolation, 5, 100);
			targetMat.SetInt ("_ParallaxInterpolation", parallaxInterpolation);
		}
		EditorGUILayout.EndHorizontal ();

		useDepthBias = targetMat.GetFloat("_useDepthBias") == 1;
		useDepthBias = EditorGUILayout.Toggle ("Use depth bias", useDepthBias);
		targetMat.SetFloat ("_useDepthBias", useDepthBias ? 1f : 0f );


		if (!useDepthBias)
			return;


		depthBiasDistance = targetMat.GetFloat ("_DepthBias");
		depthBiasDistance = EditorGUILayout.FloatField ("Depth-Bias Distance", depthBiasDistance);
		targetMat.SetFloat ("_DepthBias", depthBiasDistance);


		depthBiasPower = targetMat.GetFloat ("_DepthBiasPower");
		depthBiasPower = EditorGUILayout.FloatField ("Depth-Bias Power", depthBiasPower);
		targetMat.SetFloat ("_DepthBiasPower", depthBiasPower);

		depthBiasThreshold = targetMat.GetFloat ("_DepthBiasThreshold");
		depthBiasThreshold = EditorGUILayout.FloatField ("Depth-Bias Threshold", depthBiasThreshold);
		targetMat.SetFloat ("_DepthBiasThreshold", depthBiasThreshold);


	}


	void generateStyles() {

		bool hasPro = UnityEditorInternal.InternalEditorUtility.HasPro ();

		redChannelGUIStyle = new GUIStyle ();
		if( !hasPro )
			redChannelGUIStyle.normal.background = (Texture2D)Resources.Load ("Textures/redChannelBG");
		else
			redChannelGUIStyle.normal.background = (Texture2D)Resources.Load ("Textures/redChannelBGPro");
		//redChannelGUIStyle.margin = new RectOffset (0, 0, 0, 0);
		redChannelGUIStyle.padding = new RectOffset (12, 0, 0, 0);


		greenChannelGUIStyle = new GUIStyle ();
		if( !hasPro )
			greenChannelGUIStyle.normal.background = (Texture2D)Resources.Load ("Textures/greenChannelBG");
		else
			greenChannelGUIStyle.normal.background = (Texture2D)Resources.Load ("Textures/greenChannelBGPro");
		//greenChannelGUIStyle.margin = new RectOffset (0, 0, 0, 0);
		greenChannelGUIStyle.padding = new RectOffset (12, 0, 0, 0);

		blueChannelGUIStyle = new GUIStyle ();
		if( !hasPro )
			blueChannelGUIStyle.normal.background = (Texture2D)Resources.Load ("Textures/blueChannelBG");
		else
			blueChannelGUIStyle.normal.background = (Texture2D)Resources.Load ("Textures/blueChannelBGPro");
		//blueChannelGUIStyle.margin = new RectOffset (0, 0, 0, 0);
		blueChannelGUIStyle.padding = new RectOffset (12, 0, 0, 0);


		neutralGUIStyle = new GUIStyle ();
		if( !hasPro )
			neutralGUIStyle.normal.background = (Texture2D)Resources.Load ("Textures/neutralBG");
		else 
			neutralGUIStyle.normal.background = (Texture2D)Resources.Load ("Textures/neutralBGPro");
		//neutralGUIStyle.margin = new RectOffset (0, 0, 0, 0);
		neutralGUIStyle.padding = new RectOffset (12, 0, 0, 0);

		headerBoxStyle = new GUIStyle ();
		headerBoxStyle.normal.background = (Texture2D)Resources.Load ("Textures/box_bg");
		headerBoxStyle.normal.textColor = Color.black;
		headerBoxStyle.fontSize = 20;
		headerBoxStyle.alignment = TextAnchor.MiddleCenter;
		headerBoxStyle.border = new RectOffset (3, 3, 3, 3);
		headerBoxStyle.margin = new RectOffset (0, 0, 0, 0);

		bodyH1Style = new GUIStyle ();
		bodyH1Style.normal.background = (Texture2D)Resources.Load ("Textures/box_bg");
		bodyH1Style.normal.textColor = Color.black;
		bodyH1Style.fontSize = 14;
		bodyH1Style.alignment = TextAnchor.MiddleCenter;
		bodyH1Style.border = new RectOffset (3, 3, 3, 3);
		bodyH1Style.margin = new RectOffset (60, 3, 3, 3);

		bodyH2Style = new GUIStyle ();
		bodyH2Style.normal.background = (Texture2D)Resources.Load ("Textures/box_bg");
		bodyH2Style.normal.textColor = Color.black;
		bodyH2Style.fontSize = 13;
		bodyH2Style.alignment = TextAnchor.MiddleCenter;
		bodyH2Style.border = new RectOffset (5, 5, 5, 5);
		bodyH2Style.margin = new RectOffset (5, 5, 5, 5);

	}

}
