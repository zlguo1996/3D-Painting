using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;

public class ShaderEditor_SpeedTree_Leaf : ShaderGUI
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
	private bool showTranslucencySettings = false;

	[SerializeField]
	private bool showRedChannel = false;

	[SerializeField]
	private bool showGreenChannel = false;

	[SerializeField]
	private bool showBlueChannel = false;



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


		GUILayout.BeginVertical (neutralGUIStyle);
		guiForTranslucency ();
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
		if (GUILayout.Button ("Synchronize settings to all leaves & LODs")) {
			syncSettings ();
		}
		GUILayout.EndVertical ();



	}



	MaterialProperty getProperty( string propertyName ) {

		foreach (MaterialProperty matProp in matProperties) {

			if (matProp.name == propertyName)
				return matProp;

		}

		return null;

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

				if (mat.IsKeywordEnabled ("GEOM_TYPE_LEAF")) {
					
					mat.SetFloat("_TransNormalDistortion", targetMat.GetFloat("_TransNormalDistortion"));
					mat.SetFloat("_TransScattering", targetMat.GetFloat("_TransScattering"));
					mat.SetFloat("_TransDirect", targetMat.GetFloat("_TransDirect"));
					mat.SetFloat("_TransAmbient", targetMat.GetFloat("_TransAmbient"));
					mat.SetFloat("_TransShadow", targetMat.GetFloat("_TransShadow"));

					mat.SetFloat("_Smoothness", targetMat.GetFloat("_Smoothness"));
					mat.SetFloat("_Occlusion", targetMat.GetFloat("_Occlusion"));
					mat.SetFloat("_Translucency", targetMat.GetFloat("_Translucency"));

					mat.SetFloat("_Smoothness2", targetMat.GetFloat("_Smoothness2"));
					mat.SetFloat("_Occlusion2", targetMat.GetFloat("_Occlusion2"));
					mat.SetFloat("_Translucency2", targetMat.GetFloat("_Translucency2"));

					mat.SetFloat("_Smoothness3", targetMat.GetFloat("_Smoothness3"));
					mat.SetFloat("_Occlusion3", targetMat.GetFloat("_Occlusion3"));
					mat.SetFloat("_Translucency3", targetMat.GetFloat("_Translucency3"));





				}

			}

		}





	}


	void guiForRedChannel() {


		showRedChannel = targetMat.GetFloat("_showRed") == 1;

		showRedChannel = EditorGUILayout.Foldout(showRedChannel, "Red Channel Settings");

		targetMat.SetFloat ("_showRed", showRedChannel ? 1f : 0f );

		if (!showRedChannel)
			return;

		matEditor.TexturePropertySingleLine (new GUIContent ("Albedo Map"), getProperty ("_Albedo"));
		matEditor.ColorProperty ( getProperty ("_AlbedoColor"), "Albedo Color");

		matEditor.TexturePropertySingleLine (new GUIContent ("Normal Map"), getProperty ("_Normal"), getProperty ("_NormalScale"));

		matEditor.TexturePropertySingleLine (new GUIContent ("Combined Map"), getProperty ("_Combined"));

		matEditor.RangeProperty (getProperty ("_Smoothness"), "Smoothness Strength");
		matEditor.RangeProperty (getProperty ("_Occlusion"), "Occlusion Strength");
		matEditor.RangeProperty (getProperty ("_Translucency"), "Translucency Strength");

	}

	void guiForGreenChannel() {

		showGreenChannel = targetMat.GetFloat("_showGreen") == 1;
		showGreenChannel = EditorGUILayout.Foldout(showGreenChannel, "Green Channel Settings");
		targetMat.SetFloat ("_showGreen", showGreenChannel ? 1f : 0f );

		if (!showGreenChannel)
			return;

		matEditor.TexturePropertySingleLine (new GUIContent ("Albedo Map"), getProperty ("_Albedo2"));
		matEditor.ColorProperty ( getProperty ("_AlbedoColor2"), "Albedo Color");

		matEditor.TexturePropertySingleLine (new GUIContent ("Normal Map"), getProperty ("_Normal2"), getProperty ("_NormalScale2"));

		matEditor.TexturePropertySingleLine (new GUIContent ("Combined Map"), getProperty ("_Combined2"));

		matEditor.RangeProperty (getProperty ("_Smoothness2"), "Smoothness Strength");
		matEditor.RangeProperty (getProperty ("_Occlusion2"), "Occlusion Strength");
		matEditor.RangeProperty (getProperty ("_Translucency2"), "Translucency Strength");







	}

	void guiForBlueChannel() {

		showBlueChannel = targetMat.GetFloat("_showBlue") == 1;
		showBlueChannel = EditorGUILayout.Foldout(showBlueChannel, "Blue Channel Settings");
		targetMat.SetFloat ("_showBlue", showBlueChannel ? 1f : 0f );

		if (!showBlueChannel)
			return;

		matEditor.TexturePropertySingleLine (new GUIContent ("Albedo Map"), getProperty ("_Albedo3"));
		matEditor.ColorProperty ( getProperty ("_AlbedoColor3"), "Albedo Color");

		matEditor.TexturePropertySingleLine (new GUIContent ("Normal Map"), getProperty ("_Normal3"), getProperty ("_NormalScale3"));
		matEditor.TexturePropertySingleLine (new GUIContent ("Combined Map"), getProperty ("_Combined3"));

		matEditor.RangeProperty (getProperty ("_Smoothness3"), "Smoothness Strength");
		matEditor.RangeProperty (getProperty ("_Occlusion3"), "Occlusion Strength");
		matEditor.RangeProperty (getProperty ("_Translucency3"), "Translucency Strength");




	}



	void guiForGeneral() {

		showGeneralSettings = targetMat.GetFloat("_showGeneral") == 1;
		showGeneralSettings = EditorGUILayout.Foldout(showGeneralSettings, "General Settings");
		targetMat.SetFloat ("_showGeneral", showGeneralSettings ? 1f : 0f );

		//useHeightbasedBlending = targetMat.GetFloat("_useHeightBasedBlending") == 1;

		if (!showGeneralSettings)
			return;



		matEditor.RangeProperty (getProperty ("_MaskClipValue"), "Mask Clip Value");


		windQuality = (WindQuality)targetMat.GetFloat ("_WindQuality");
		windQuality = (WindQuality) EditorGUILayout.EnumPopup("Wind Quality:", windQuality);
		targetMat.SetFloat ("_WindQuality", (float)windQuality);



	}


	void guiForTranslucency() {

		showTranslucencySettings = targetMat.GetFloat("_showTranslucency") == 1;
		showTranslucencySettings = EditorGUILayout.Foldout(showTranslucencySettings, "Translucency Settings");
		targetMat.SetFloat ("_showTranslucency", showTranslucencySettings ? 1f : 0f );

		//useHeightbasedBlending = targetMat.GetFloat("_useHeightBasedBlending") == 1;

		if (!showTranslucencySettings)
			return;


		matEditor.RangeProperty (getProperty ("_TransNormalDistortion"), "Normal Distortion");
		matEditor.RangeProperty (getProperty ("_TransScattering"), "Scaterring Falloff");
		matEditor.RangeProperty (getProperty ("_TransDirect"), "Direct Intensity");
		matEditor.RangeProperty (getProperty ("_TransAmbient"), "Ambient Intensity");
		matEditor.RangeProperty (getProperty ("_TransShadow"), "Shadow Effector");




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
