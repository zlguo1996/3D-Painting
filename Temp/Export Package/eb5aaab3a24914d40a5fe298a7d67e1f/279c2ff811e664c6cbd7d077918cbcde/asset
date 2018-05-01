using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;

public class ShaderEditor_SpeedTree_Branch : ShaderGUI
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




	//[SerializeField]
	//private bool useFlowMapGlobal = false;
	//[SerializeField]
	//private bool flowOnlyNormals = false;

	/*[SerializeField]
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
	private float depthBiasThreshold = 0f;*/

	[SerializeField]
	private bool showRedChannel = false;
	[SerializeField]
	private bool showDetailRed = false;



	[SerializeField]
	private bool showGreenChannel = false;



	[SerializeField]
	private bool showBlueChannel = false;

	private bool useHeightBasedBlending = false;



	public enum WindQuality
	{
		None,
		Fastest,
		Fast,
		Better,
		Best,
		Palm
	}

	private WindQuality windQuality;


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

		GUILayout.BeginVertical (neutralGUIStyle);
		if (GUILayout.Button ("Synchronize settings to all branches & LODs")) {
			syncSettings ();
		}
		GUILayout.EndVertical ();

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

	void syncSettings() {

		List<GameObject> tmpLodGOs = new List<GameObject> ();
		LOD[] lods =  Selection.activeGameObject.GetComponentInParent<LODGroup> ().GetLODs();
		for (int i = 0; i < lods.Length; i++) {
			if( lods[i].renderers[0].GetType() == typeof(MeshRenderer) )  {
				tmpLodGOs.Add (lods [i].renderers [0].gameObject);
			}
		}

		foreach (GameObject currentGameObject in tmpLodGOs) {

			foreach (Material mat in currentGameObject.GetComponent<Renderer>().sharedMaterials) {

				if (mat.IsKeywordEnabled ("GEOM_TYPE_BRANCH")) {



					mat.SetFloat("_Occlusion", targetMat.GetFloat("_Occlusion"));
					mat.SetFloat("_Metallic", targetMat.GetFloat("_Metallic"));
					mat.SetFloat("_Smoothness", targetMat.GetFloat("_Smoothness"));

					mat.SetFloat("_Occlusion2", targetMat.GetFloat("_Occlusion2"));
					mat.SetFloat("_Metallic2", targetMat.GetFloat("_Metallic2"));
					mat.SetFloat("_Smoothness2", targetMat.GetFloat("_Smoothness2"));

					mat.SetFloat("_Occlusion3", targetMat.GetFloat("_Occlusion3"));
					mat.SetFloat("_Metallic3", targetMat.GetFloat("_Metallic3"));
					mat.SetFloat("_Smoothness3", targetMat.GetFloat("_Smoothness3"));





				}

			}

		}





	}

	void shaderKeywords () {



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

		matEditor.TexturePropertySingleLine (new GUIContent ("Albedo Map"), getProperty ("_MainTex"), getProperty ("_TilingU"), getProperty ("_TilingV"));
		matEditor.ColorProperty ( getProperty ("_AlbedoColor"), "Albedo Color");
		matEditor.RangeProperty (getProperty ("_Metallic"), "Metallic Strength");

		matEditor.TexturePropertySingleLine (new GUIContent ("Normal Map"), getProperty ("_BumpMap"), getProperty ("_NormalScale"));
		matEditor.TexturePropertySingleLine (new GUIContent ("Combined Map"), getProperty ("_CombinedMap"));

		matEditor.RangeProperty (getProperty ("_Smoothness"), "Smoothness Strength");
		matEditor.RangeProperty (getProperty ("_Occlusion"), "Occlusion Strength Strength");

		//matEditor.RangeProperty ( getProperty ("_Emission"), "Emission Strength");

		matEditor.ColorProperty ( getProperty ("_EmissionColor"), "Emission Color");



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

		matEditor.TexturePropertySingleLine (new GUIContent ("Albedo Map"), getProperty ("_MainTex2"), getProperty ("_TilingU2"), getProperty ("_TilingV2"));
		matEditor.ColorProperty ( getProperty ("_AlbedoColor2"), "Albedo Color");
		matEditor.RangeProperty (getProperty ("_Metallic2"), "Metallic Strength");

		matEditor.TexturePropertySingleLine (new GUIContent ("Normal Map"), getProperty ("_BumpMap2"), getProperty ("_NormalScale2"));
		matEditor.TexturePropertySingleLine (new GUIContent ("Combined Map"), getProperty ("_CombinedMap2"));

		matEditor.RangeProperty (getProperty ("_Smoothness2"), "Smoothness Strength");
		matEditor.RangeProperty (getProperty ("_Occlusion2"), "Occlusion Strength Strength");


		//matEditor.RangeProperty ( getProperty ("_Emission2"), "Emission Strength");
		matEditor.ColorProperty ( getProperty ("_EmissionColor2"), "Emission Color");

		matEditor.RangeProperty (getProperty ("_BaseHeight2"), "Base Height of Layer");
		matEditor.RangeProperty (getProperty ("_HeightmapFactor2"), "HeightMap Multiplier");
		matEditor.RangeProperty (getProperty ("_BlendSmooth2"), "Edge Blend Strength");
		matEditor.RangeProperty (getProperty ("_HeightBasedTransparency2"), "Height-Based Transparency");






	}

	void guiForBlueChannel() {

		showBlueChannel = targetMat.GetFloat("_showBlue") == 1;
		showBlueChannel = EditorGUILayout.Foldout(showBlueChannel, "Blue Channel Settings");
		targetMat.SetFloat ("_showBlue", showBlueChannel ? 1f : 0f );

		if (!showBlueChannel)
			return;

		matEditor.TexturePropertySingleLine (new GUIContent ("Albedo Map"), getProperty ("_MainTex3"), getProperty ("_TilingU3"), getProperty ("_TilingV3"));
		matEditor.ColorProperty ( getProperty ("_AlbedoColor3"), "Albedo Color");
		matEditor.RangeProperty (getProperty ("_Metallic3"), "Metallic Strength");

		matEditor.TexturePropertySingleLine (new GUIContent ("Normal Map"), getProperty ("_BumpMap3"), getProperty ("_NormalScale3"));
		matEditor.TexturePropertySingleLine (new GUIContent ("Combined Map"), getProperty ("_CombinedMap3"));

		matEditor.RangeProperty (getProperty ("_Smoothness3"), "Smoothness Strength");
		matEditor.RangeProperty (getProperty ("_Occlusion3"), "Occlusion Strength Strength");


		//matEditor.RangeProperty ( getProperty ("_Emission3"), "Emission Strength");
		matEditor.ColorProperty ( getProperty ("_EmissionColor3"), "Emission Color");

		matEditor.RangeProperty (getProperty ("_BaseHeight3"), "Base Height of Layer");
		matEditor.RangeProperty (getProperty ("_HeightmapFactor3"), "HeightMap Multiplier");
		matEditor.RangeProperty (getProperty ("_BlendSmooth3"), "Edge Blend Strength");
		matEditor.RangeProperty (getProperty ("_HeightBasedTransparency3"), "Height-Based Transparency");




	}



	void guiForGeneral() {

		showGeneralSettings = targetMat.GetFloat("_showGeneral") == 1;
		showGeneralSettings = EditorGUILayout.Foldout(showGeneralSettings, "General Settings");
		targetMat.SetFloat ("_showGeneral", showGeneralSettings ? 1f : 0f );

		//useHeightbasedBlending = targetMat.GetFloat("_useHeightBasedBlending") == 1;

		if (!showGeneralSettings)
			return;



		/*useHeightbasedBlending = EditorGUILayout.Toggle ("Heightbased Blending", useHeightbasedBlending);
		targetMat.SetFloat ("_useHeightBasedBlending", useHeightbasedBlending ? 1f : 0f );
		if( useHeightbasedBlending )
			targetMat.EnableKeyword ("_HEIGHTBASED_BLENDING");
		else
			targetMat.DisableKeyword ("_HEIGHTBASED_BLENDING");*/


		windQuality = (WindQuality)targetMat.GetFloat ("_WindQuality");
		windQuality = (WindQuality) EditorGUILayout.EnumPopup("Wind Quality:", windQuality);
		targetMat.SetFloat ("_WindQuality", (float)windQuality);


		useHeightBasedBlending = targetMat.GetFloat ("_Heightbased_Blending") == 1;
		useHeightBasedBlending = EditorGUILayout.Toggle ("Heightbased Blending", useHeightBasedBlending);
		targetMat.SetFloat ("_Heightbased_Blending", useHeightBasedBlending ? 1f : 0f);
		if (useHeightBasedBlending) {
			targetMat.EnableKeyword ("_HEIGHTBASED_BLENDING");
		} else {
			targetMat.DisableKeyword ("_HEIGHTBASED_BLENDING");
		}



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
