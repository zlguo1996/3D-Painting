using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class VT_Tree_window : EditorWindow {
	
	/* GUI Styles */
	private GUIStyle headerBoxStyle;
	private GUIStyle bodyH1Style;
	/* END GUI Styles */

	/* Global variables */
	private bool goIsTree = false;
	private string toolState = "null";
	private string lastToolState = "null";
	private GameObject currentGameObject;
	private string gameObjectState = "null";
	bool onFinishDeleteCollider = true;
	bool onFinishConvex = false;

	[SerializeField]
	private Vector2 scrollPosition;
	/* END Global variables */

	/* Paint Mode */

		/* General Settings */
		[SerializeField]
		private bool useAutoFocus = false;
		[SerializeField]
		private bool highlightGameObject = false;
		[SerializeField]
		private bool showVertexIndicators = false;
		[SerializeField]
		private float vertexIndicatorSize = 1f;

		//[SerializeField]
		//private string[] drawChannels = new string[3] {"RGB(A)", "RGB", "Alpha"};
		[SerializeField]
		private int drawIndex = 0;
		[SerializeField]
		private int drawOn = 0;
		/* END General Settings */

		/* Brush Settings */
		[SerializeField]	
		private float brushSize = 0.2f;
		[SerializeField]
		private float brushFalloff = 0.1f;
		[SerializeField]
		private float brushStrength = 0.4f;
		
		[SerializeField]
		private Color drawColor = new Color(1,0,0,1);
		[SerializeField]
		private float drawHeight = 0;
		/* END Brush Settings */

	/* END Paint Mode */





	/* Painter */

		/* Global Variables */
		Tool lastUsedTool;
		int originalLayer;


		List<Color[]> drawJobList;
		int drawJobListStepBack = 0;
		/* END Global Variables */
		
		/* Brush */
		private Vector2 mousePos = Vector2.zero;
		private RaycastHit brushHit;
		private bool brushHitOnObject = false;
		/* END Brush */

		/* Drawing Vertex Color */
		private Color[] cancelColors;
		private Vector3[] currentVertices;
		private Color[] currentColors;
		/* END Drawing Vertex Color */

		float initialWindQuality;

	/* END Painter */



	/* Texture Assistent */
		[SerializeField]
		private string taState = "albedo";



		/* Combined TA */
		[SerializeField]
		private Texture2D thicknessMap;
		[SerializeField]
		private Texture2D OcclusionMap;
		[SerializeField]
		private Texture2D smoothnessMap;
		[SerializeField]
		private Texture2D emissionMap;

		private Vector2 scrollPositionCombined;

		[SerializeField]
		private string[] combinedSize = new string[6] {"64", "128", "256", "512", "1024", "2048"};
		[SerializeField]
		private int combinedSizeIndex = 3;
		[SerializeField]
		private Object objectForPathCombined = null;
		[SerializeField]
		private string outputName = "";
		string pathCombined;


		/* END Combined TA */

		/* END Texture Assistent */

		



	void checkCurrentGameObject() {

		if (currentGameObject && Selection.activeGameObject == currentGameObject && currentGameObject.GetComponent<VertexColorStream> () != null)
			return;
		
		currentGameObject = Selection.activeGameObject;

		if ( !currentGameObject || !currentGameObject.GetComponent<MeshRenderer>()) {
			lastToolState = toolState;

			gameObjectState = "null";

			if (toolState != "texassist") {
				toolState = "null";
			}
				
			return;
		} else {
			if (lastToolState == "null") {
				toolState = "paint";
			} else {
				toolState = lastToolState;
			}
		}

		if (currentGameObject.GetComponent<VertexColorStream> () == null) {

			if( currentGameObject.GetComponent<Tree> () == null ) {
				goIsTree = false;
			} else {
				goIsTree = true;
			}

			gameObjectState = "null";
		} else {
			gameObjectState = "ready";

		}


	}

	void Update() {

		checkCurrentGameObject ();

		Repaint ();

	}

	void guiForHeader() {
		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Vertex Tools Pro - For SpeedTree", headerBoxStyle, GUILayout.Height(40), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();
	}

	void guiForPlayState() {
		GUI.Box(new Rect(10, 45, Screen.width-20, 25), "Vertex Tools are not available in playmode.");
	}


	void guiForNullState() {
		GUI.Box(new Rect(10, 90, Screen.width-20, 45), "In order to use Vertex Painter Pro's Paint Features you first have to select a gameobject in your scene.");
	}

	void guiForGONullState() {


		if (!goIsTree) {
			GUI.Box(new Rect(10, 90, Screen.width-20, 45), "The selected gameobject is no SpeedTree. In order to paint regular meshes please use Vertex Tools Painter");
			return;
		}



		GUI.Box(new Rect(10, 90, Screen.width-20, 45), "The selected gameobject has to be prepared to use Vertex Painting. Just Click below to have everything setup automatically for you.");
		GUILayout.Space (65);

		GUILayout.Space (5);
		if (GUILayout.Button ("Initialize everything now.", GUI.skin.button, GUILayout.Height (30))) {

			if (currentGameObject.GetComponent<VertexColorStream> ())
				return;

			currentGameObject.AddComponent<VertexColorStream> ();


			currentGameObject.GetComponent<VertexColorStream> ().init (currentGameObject.GetComponent<MeshFilter> ().sharedMesh, false);

			Color[] initColors = currentGameObject.GetComponent<VertexColorStream>().getColors();
			for (int i = 0; i < initColors.Length; i++) {
				initColors [i].g = 0;
				initColors [i].b = 0;
			}

			currentGameObject.GetComponent<VertexColorStream> ().setColors (initColors);

			foreach (Material mat in currentGameObject.GetComponent<Renderer>().sharedMaterials) {

				if (mat.IsKeywordEnabled ("GEOM_TYPE_BRANCH") || mat.IsKeywordEnabled ("GEOM_TYPE_BRANCH_DETAIL") || mat.IsKeywordEnabled ("GEOM_TYPE_FROND")) {

					Texture tmpBase = mat.GetTexture ("_MainTex");
					Texture tmpNormal = mat.GetTexture ("_BumpMap");

					mat.shader = Shader.Find ("VTP/SpeedTree/Branch");
					mat.SetTexture ("_Albedo", tmpBase);
					mat.SetTexture ("_Normal", tmpNormal);
					mat.SetTexture ("_Albedo2", tmpBase);
					mat.SetTexture ("_Normal2", tmpNormal);
					mat.SetTexture ("_Albedo3", tmpBase);
					mat.SetTexture ("_Normal3", tmpNormal);
					mat.SetTexture ("_DetailTex", null);

				} else if ( mat.IsKeywordEnabled ("GEOM_TYPE_LEAF") ) {
					Texture tmpBase = mat.GetTexture ("_MainTex");
					Texture tmpNormal = mat.GetTexture ("_BumpMap");

					mat.shader = Shader.Find ("VTP/SpeedTree/Leaf");
					mat.SetTexture ("_Albedo", tmpBase);
					mat.SetTexture ("_Normal", tmpNormal);
					mat.SetTexture ("_Albedo2", tmpBase);
					mat.SetTexture ("_Normal2", tmpNormal);
					mat.SetTexture ("_Albedo3", tmpBase);
					mat.SetTexture ("_Normal3", tmpNormal);
					mat.SetTexture ("_DetailTex", null);
				}

			}


			List<GameObject> tmpLodGOs = new List<GameObject> ();
			LOD[] lods =  currentGameObject.GetComponentInParent<LODGroup> ().GetLODs();
			for (int i = 0; i < lods.Length; i++) {
				if( lods[i].renderers[0].GetType() == typeof(MeshRenderer) )  {
					tmpLodGOs.Add (lods [i].renderers [0].gameObject);
				}
			}
				
			for (int i = 0; i < tmpLodGOs.Count - 1; i++) {
				doColorTransfer (tmpLodGOs[i],tmpLodGOs[i+1]);
			}





			EditorUtility.SetDirty(currentGameObject.GetComponent<VertexColorStream> ());
			//EditorSceneManager.MarkSceneDirty(currentGameObject.GetComponent<VertexColorStream> ().gameObject.scene);
			Undo.RegisterCompleteObjectUndo (currentGameObject, "Initialize");


			//if (currentGameObject != currentGameObject.transform.root.gameObject && !currentGameObject.transform.root.gameObject.GetComponent<VertexStreamChildrenRebuilder>() ) {
			//	currentGameObject.transform.root.gameObject.AddComponent<VertexStreamChildrenRebuilder> ();
			//}


		
	

			toolState = "paint";
			gameObjectState = "ready";

		}
	}


	void guiForPaintState() {

		GUILayout.Space (15);
		guiForGeneralSettings ();
	
		GUILayout.Space (15);
		guiForBrushSettings ();

		GUILayout.Space (15);
		guiForPaintButton ();

		GUILayout.Space (15);
		guiForColorTransfer ();

		GUILayout.Space (15);
		guiForTerrainStream ();


	}



	void guiForGeneralSettings () {

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("General Settings", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);



		useAutoFocus = EditorGUILayout.Toggle ("Auto Focus",useAutoFocus, GUI.skin.toggle);
		highlightGameObject = EditorGUILayout.Toggle ("Highlight Gameobject",highlightGameObject, GUI.skin.toggle);



	}

	void guiForBrushSettings () {

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Brush Settings", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);

		brushSize = EditorGUILayout.Slider ("Size", brushSize, 0.01f, 5f);
		GUILayout.Space (2);
		brushFalloff = EditorGUILayout.Slider ("Falloff", brushFalloff, 0.005f, brushSize);
		GUILayout.Space (2);
		brushStrength = EditorGUILayout.Slider ("Strength", brushStrength, 0f, 1f);
		GUILayout.Space (2);
		//drawColor = EditorGUILayout.ColorField ("Draw with color: ", drawColor);
		//GUILayout.Space (2);


		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Draw Mode", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();
		GUILayout.Space (5);



		GUILayout.BeginHorizontal ();

		if (GUILayout.Toggle (drawOn == 0, "Leaves", GUI.skin.button, GUILayout.Width (position.width/3.33f), GUILayout.Height (position.width/6/2) )) {
			drawOn = 0;
		}
		if (GUILayout.Toggle (drawOn == 1, "Branches", GUI.skin.button, GUILayout.Width (position.width/3.33f), GUILayout.Height (position.width/6/2) )) {
			drawOn = 1;
		}
		if (GUILayout.Toggle (drawOn == 2, "Both", GUI.skin.button, GUILayout.Width (position.width/3.33f), GUILayout.Height (position.width/6/2) )) {
			drawOn = 2;
		}

		GUILayout.EndHorizontal ();


		GUILayout.BeginHorizontal ();

		if (GUILayout.Toggle (drawIndex == 0, "Add Occlusion", GUI.skin.button, GUILayout.Width (position.width/2.2f), GUILayout.Height (position.width/6/2) )) {
			drawColor = new Color (0, 0, 0, 0);
			drawIndex = 0;
		}
		if (GUILayout.Toggle (drawIndex == 1, "Reduce Occlusion", GUI.skin.button, GUILayout.Width (position.width/2.2f), GUILayout.Height (position.width/6/2) )) {
			drawColor = new Color (1f, 0, 0, 0);
			drawIndex = 1;
		}

		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();

		if (GUILayout.Toggle (drawIndex == 2, "First Layer (R)", GUI.skin.button, GUILayout.Width (position.width / 3.33f ), GUILayout.Height (position.width / 6 ))) {
			drawColor = new Color (0, 0, 0, 0);
			drawIndex = 2;
		}
		if (GUILayout.Toggle (drawIndex == 3, "Second Layer (G)", GUI.skin.button, GUILayout.Width (position.width / 3.33f ), GUILayout.Height (position.width / 6 ))) {
			drawColor = new Color (0, 1f, 0, 0);
			drawIndex = 3;
		}
		if (GUILayout.Toggle (drawIndex == 4, "Third Layer (B)", GUI.skin.button, GUILayout.Width (position.width / 3.33f ), GUILayout.Height (position.width / 6 ))) {
			drawColor = new Color (0, 0, 1f, 0);
			drawIndex = 4;
		}

	


		GUILayout.EndHorizontal ();

		GUILayout.Space (10);

		if (drawOn == 1) {

			GUILayout.BeginHorizontal ();
			GUILayout.Box ("Height to draw with", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal ();
			GUILayout.Space (5);


			drawHeight = EditorGUILayout.Slider ("", drawHeight, 0, 1);
			drawColor.a = drawHeight;

		}

		GUILayout.Space (10);
	


	}

	void guiForPaintButton() {
		
		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Vertex Painting", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);


		if ( (toolState == "paint") && GUILayout.Button ("Paint '"+currentGameObject.name+"'", GUI.skin.button, GUILayout.Height (40))) {
			//Start painting
			toolState = "curPainting";

			initialWindQuality = currentGameObject.GetComponent<MeshRenderer> ().sharedMaterial.GetFloat ("_WindQuality");
			currentGameObject.GetComponent<MeshRenderer> ().sharedMaterial.SetFloat ("_WindQuality", 0);

			if (!currentGameObject.GetComponent<MeshCollider> ()) {
				onFinishDeleteCollider = true;
				currentGameObject.AddComponent<MeshCollider> ();
			} else {
				onFinishDeleteCollider = false;

				if (currentGameObject.GetComponent<MeshCollider> ().convex) {
					onFinishConvex = true;
					currentGameObject.GetComponent<MeshCollider> ().convex = false;
				} else {
					onFinishConvex = false;
				}
			
			}

			if( useAutoFocus )
				SceneView.lastActiveSceneView.FrameSelected();

			if( highlightGameObject )
				SetSearchFilter (currentGameObject.name, 1);

			originalLayer = currentGameObject.layer;
			currentGameObject.layer = 31;

			lastUsedTool = Tools.current;
			Tools.current = Tool.None;

			drawJobList = new List<Color[]>();
			getCurrentColorsFromStream ();
			drawJobListStepBack = 0;
			addJobToDrawJobList (true);


		}
			
		if (toolState == "curPainting" && GUILayout.Button ("Save '"+currentGameObject.name+"'", GUI.skin.button, GUILayout.Height (40))) {
			//Save painting
			saveColorsToStream();

			if( highlightGameObject )
				SetSearchFilter ("", 0);
			
			Tools.current = lastUsedTool;

			toolState = "paint";

			currentGameObject.GetComponent<MeshRenderer> ().sharedMaterial.SetFloat ("_WindQuality", initialWindQuality);

			if (onFinishDeleteCollider)
				DestroyImmediate (currentGameObject.GetComponent<MeshCollider> ());

			if (onFinishConvex && currentGameObject.GetComponent<MeshCollider> ())
				currentGameObject.GetComponent<MeshCollider> ().convex = true;

			currentGameObject.layer = originalLayer;
		}


		if (toolState == "curPainting" && GUILayout.Button ("Cancel without save", GUI.skin.button, GUILayout.Height (40))) {
			//Cancel painting
			cancelDrawing();

			if( highlightGameObject )
				SetSearchFilter ("", 0);
			
			Tools.current = lastUsedTool;

			toolState = "paint";

			currentGameObject.GetComponent<MeshRenderer> ().sharedMaterial.SetFloat ("_WindQuality", initialWindQuality);

			if (onFinishDeleteCollider)
				DestroyImmediate (currentGameObject.GetComponent<MeshCollider> ());

			if (onFinishConvex && currentGameObject.GetComponent<MeshCollider> ())
				currentGameObject.GetComponent<MeshCollider> ().convex = true;

			currentGameObject.layer = originalLayer;

		}

		if (toolState != "curPainting")
			return;
		
		GUILayout.Space (15);

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Draw Commands", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);

		GUILayout.BeginHorizontal ();
		if (toolState == "curPainting" && GUILayout.Button ("Undo", GUI.skin.button, GUILayout.Height (30))) {
			//Undo
			undoDrawJob();
		}
		if (toolState == "curPainting" && GUILayout.Button ("Redo", GUI.skin.button, GUILayout.Height (30))) {
			//Redo
			redoDrawJob();
		}

		GUILayout.EndHorizontal ();

		if (toolState == "curPainting" && GUILayout.Button ("Flood Fill with Color", GUI.skin.button, GUILayout.Height (40))) {
			//Flood Fill
			floodFillVertexColor ();
		}



	}


	GameObject[] LODs;
	List<GameObject> lodGOs;

	void guiForColorTransfer() {

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Color Transfer to LODs", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);


		lodGOs = new List<GameObject> ();
		if (currentGameObject) {
			LOD[] lods = currentGameObject.GetComponentInParent<LODGroup> ().GetLODs ();
			for (int i = 0; i < lods.Length; i++) {
				if (lods [i].renderers [0].GetType () == typeof(MeshRenderer)) {
					lodGOs.Add (lods [i].renderers [0].gameObject);
				}
			}
		}

		EditorGUIUtility.labelWidth = 40;
		GUILayout.Space (10);

		for (int i = 0; i < lodGOs.Count ; i++) {
			GUILayout.Space (25);
			Rect latestRect = GUILayoutUtility.GetLastRect ();
			EditorGUI.ObjectField (new Rect (5, latestRect.position.y, 310, 20),"LOD"+i+": ", lodGOs[i], typeof(GameObject), true);

		}


		GUILayout.Space (10);
		if (GUILayout.Button ("Transfer Colors", GUI.skin.button, GUILayout.Width (position.width - 10), GUILayout.Height (40))) {
			//drawColor = new Color (0, 0, 1f, 1);
			for (int i = 0; i < lodGOs.Count - 1; i++) {
				doColorTransfer (lodGOs[i],lodGOs[i+1]);
			}
		}
	}


	void guiForTerrainStream() {

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Stream to Prefab & Terrain", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);

		if (GUILayout.Button ("Stream data to Prefab & Terrain", GUI.skin.button, GUILayout.Width (position.width - 10), GUILayout.Height (40))) {
			GameObject parent = currentGameObject.transform.parent.gameObject;
			PrefabUtility.ReplacePrefab(parent, PrefabUtility.GetPrefabParent(parent), ReplacePrefabOptions.Default); 

			Terrain[] terrains = (Terrain[])GameObject.FindObjectsOfType<Terrain> () ;

			for (int i = 0; i < terrains.Length; i++) {
				if (!terrains [i].gameObject.GetComponent<terrainStream> ()) {
					terrains [i].gameObject.AddComponent<terrainStream> ();
				}
				terrains [i].gameObject.GetComponent<terrainStream> ().streamToTerrainTrees ();

			}

			EditorSceneManager.MarkAllScenesDirty ();
		}





		GUILayout.FlexibleSpace ();
	}



	void guiForTAState() {


		scrollPositionCombined = GUILayout.BeginScrollView (scrollPositionCombined, false, false, GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true));

		GUILayout.Space (15);
		GUI.Box(new Rect(10, 5, Screen.width-20, 45), "The Texture Assistant allows you to create the required packed textures out of single grayscaled maps.");
		GUILayout.Space (45);

		GUILayout.BeginVertical();
		{
			GUILayout.BeginHorizontal();
			{

				if (GUILayout.Toggle (taState == "combined", "Combined Texture Packer", EditorStyles.toolbarButton))
					taState = "combined";

			}
			GUILayout.EndHorizontal(); 
		}
		GUILayout.EndVertical ();

	
		if (taState == "combined")
			guiForCombinedTAState ();

		GUILayout.EndScrollView ();

	}




	void guiForCombinedTAState() {

		float editorWidth = position.width;

		float textureWidthAndHeight = editorWidth / 2 - 40;
		float editorGUIHeight = 130;

		GUILayout.Space (15);

		GUILayout.BeginHorizontal ();
		{

			GUILayout.BeginVertical ();
			{
				GUILayout.Box ("Thickness Map", bodyH1Style, GUILayout.Height (20), GUILayout.ExpandWidth (true));
				thicknessMap =  (Texture2D)EditorGUI.ObjectField(new Rect(15,editorGUIHeight,textureWidthAndHeight,textureWidthAndHeight), thicknessMap, typeof(Texture2D), false );
			}
			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			{
				GUILayout.Box ("Occlusion Map", bodyH1Style, GUILayout.Height (20), GUILayout.ExpandWidth (true));
				OcclusionMap =  (Texture2D)EditorGUI.ObjectField(new Rect(15+editorWidth/2,editorGUIHeight,textureWidthAndHeight,textureWidthAndHeight), OcclusionMap, typeof(Texture2D), false );
			}
			GUILayout.EndVertical ();

		}
		GUILayout.EndHorizontal ();

		editorGUIHeight += textureWidthAndHeight + 40 + 25;
		GUILayout.Space (textureWidthAndHeight+40);

		GUILayout.BeginHorizontal ();
		{

			GUILayout.BeginVertical ();
			{
				GUILayout.Box ("Smoothness Map", bodyH1Style, GUILayout.Height (20), GUILayout.ExpandWidth (true));
				smoothnessMap =  (Texture2D)EditorGUI.ObjectField(new Rect(15,editorGUIHeight,textureWidthAndHeight,textureWidthAndHeight), smoothnessMap, typeof(Texture2D), false );
			}
			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			{
				GUILayout.Box ("Emission Map", bodyH1Style, GUILayout.Height (20), GUILayout.ExpandWidth (true));
				emissionMap =  (Texture2D)EditorGUI.ObjectField(new Rect(15+editorWidth/2,editorGUIHeight,textureWidthAndHeight,textureWidthAndHeight), emissionMap, typeof(Texture2D), false );
			}
			GUILayout.EndVertical ();

		}
		GUILayout.EndHorizontal ();

		editorGUIHeight += textureWidthAndHeight + 10;
		GUILayout.Space (textureWidthAndHeight+40);

		if (!thicknessMap || !OcclusionMap || !smoothnessMap || !emissionMap) {
			GUI.Box(new Rect(10, editorGUIHeight, editorWidth-20-15, 30), "Warning: You are missing one or more maps.");
			GUILayout.Space (40);
			editorGUIHeight += 40;
		}



		GUILayout.Box ("Generate Map", bodyH1Style, GUILayout.Height (20), GUILayout.ExpandWidth (true));


		editorGUIHeight += 45;

		combinedSizeIndex = EditorGUI.Popup(
			new Rect(3,editorGUIHeight, editorWidth - 6 , 20),
			"Combined Map Size:",
			combinedSizeIndex, 
			combinedSize);

		editorGUIHeight += 25;

		if ( (thicknessMap || OcclusionMap || smoothnessMap || emissionMap)) {

			if (thicknessMap) {
				objectForPathCombined = thicknessMap;
			} else if (OcclusionMap) {
				objectForPathCombined = OcclusionMap;
			} else if (smoothnessMap) {
				objectForPathCombined = smoothnessMap;
			} else if (emissionMap) {
				objectForPathCombined = emissionMap;
			}

		}


		if (Path.GetExtension (AssetDatabase.GetAssetPath (objectForPathCombined)) != "") {

			objectForPathCombined = AssetDatabase.LoadAssetAtPath( Path.GetDirectoryName(AssetDatabase.GetAssetPath (objectForPathCombined)), typeof(Object));

		}

		objectForPathCombined = (Object)EditorGUI.ObjectField (new Rect (3, editorGUIHeight, editorWidth - 6, 20), "Drag Folder to save to:", objectForPathCombined, typeof(Object), false);
		editorGUIHeight += 25;
		outputName = (string)EditorGUI.TextField (new Rect (3, editorGUIHeight, editorWidth - 6, 20),"Save name:", outputName);
		GUILayout.Space (25);

		pathCombined = AssetDatabase.GetAssetPath (objectForPathCombined);
	


		GUILayout.Space (45);


		GUILayout.Space (20);



		if (objectForPathCombined && GUILayout.Button ("Generate packed Combined Map", GUI.skin.button, GUILayout.Height (40))) {
			generatePackedCombinedTexture();
		}
		GUILayout.Space (10);



	}





	void guiForTabs() {

		if (toolState != "null" && currentGameObject) {

			GUILayout.BeginVertical ();
			{
				GUILayout.BeginHorizontal ();
				{
					if (GUILayout.Toggle (toolState == "paint", "Paint", EditorStyles.toolbarButton) && toolState != "curPainting" && toolState != "curDeforming" && toolState != "curFlowing") {
						toolState = "paint";
					}

			
					if (GUILayout.Toggle (toolState == "texassist", "Texture Assistant", EditorStyles.toolbarButton) && toolState != "curPainting" && toolState != "curDeforming" && toolState != "curFlowing")
						toolState = "texassist";
				}
				GUILayout.EndHorizontal (); 
			}
			GUILayout.EndVertical ();



		} 

	}








	void OnGUI() {


		/* Draw Header */
		guiForHeader ();

		if (Application.isPlaying) {
			guiForPlayState ();
			return;
		}


		if ( (!currentGameObject || ( currentGameObject && !currentGameObject.GetComponent<MeshRenderer>()))  && toolState != "texassist" ) {
			GUILayout.Space (5);
			guiForTabs ();
			guiForNullState ();
			return;
		}
			


		if (toolState == "null" && toolState != "texassist") {
			GUILayout.Space (5);
			guiForTabs ();
			guiForNullState ();
			return;
		}

		if (gameObjectState == "null"  && toolState != "texassist") {
			GUILayout.Space (5);
			guiForTabs ();
			guiForGONullState ();
			return;
		}


		if (currentGameObject) {

			bool isPrefabInstance = PrefabUtility.GetPrefabParent(currentGameObject) != null && PrefabUtility.GetPrefabObject(currentGameObject.transform) != null;
			bool isDisconnectedPrefabInstance = PrefabUtility.GetPrefabParent(currentGameObject) != null && PrefabUtility.GetPrefabObject(currentGameObject.transform) == null;

			if (isPrefabInstance && !isDisconnectedPrefabInstance) {
				PrefabUtility.DisconnectPrefabInstance(currentGameObject);
			}

		}


		GUILayout.Space (10);
		guiForTabs ();

		scrollPosition = GUILayout.BeginScrollView (scrollPosition, false, false, GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true));

		if ((toolState == "paint" || toolState == "curPainting") && gameObjectState != "null") {
			guiForPaintState ();
		}




		if (toolState == "texassist")
			guiForTAState ();



		GUILayout.EndScrollView ();

		GUILayout.FlexibleSpace ();
		guiForVersion ();


	}

	void guiForVersion() {

		GUILayout.Box ("", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.Box ("Version 2.0.0f1", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));

	}

	void OnSceneGUI(SceneView sceneView) {

		ProcessInputs ();

		if (toolState == "curPainting" || toolState == "curWetting") {
			drawBrush ();
			drawAffectedVertices ();
		}
			


		sceneView.Repaint ();
	}
		
	public static void LaunchVT_Tree_window() {

		var win = EditorWindow.GetWindow<VT_Tree_window> (false, "VertexToolsPro - SpeedTree", true);
		win.generateStyles ();

	}

	void OnEnable() {

		generateStyles ();

		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;

	}


	void OnDestroy() {

		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	}

	void generateStyles() {

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
		bodyH1Style.margin = new RectOffset (3, 3, 3, 3);

	}


	bool textureIsAccessible(Texture2D texture) {
		try {
			texture.GetPixel (0, 0);
		} catch (UnityException e) {
			if (e.Message.StartsWith ("Texture '" + texture.name + "' is not readable")) {
				return false;
			}
		}

		return true;
	}

	void ProcessInputs() {

		Event e = Event.current;
		mousePos = e.mousePosition;


		if (e.type == EventType.MouseUp) {

			if( toolState == "curPainting" && brushHitOnObject )
				addJobToDrawJobList (true);
		}



		if( (toolState == "curPainting") && brushHitOnObject &&  (e.type == EventType.MouseDrag || e.type == EventType.mouseDown) && e.button == 0 && !e.shift && !e.alt && !e.control) {

			drawVertexColor ();
			drawJobListStepBack = 0;
		}



	}



	void drawAffectedVertices() {
	
		if (!showVertexIndicators && toolState != "curDeforming")
			return;

		if (!brushHitOnObject || brushHit.transform != currentGameObject.transform)
			return;


	

		for( int i = 0 ; i < currentVertices.Length ; i++ ) {


			Vector3 vertPos = currentGameObject.transform.TransformPoint(currentVertices[i]);
			float sqrMag = Vector3.Distance(vertPos, brushHit.point);

			float usedBrushSize = 0;
			if (toolState == "curPainting" ) {
				usedBrushSize = brushSize;
			} 
				


			float usedBrushFalloff = 0;
			if (toolState == "curPainting") {
				usedBrushFalloff = brushFalloff;
			} 



			float falloff;
			if (sqrMag > usedBrushFalloff) {
				falloff = VPP_Utils.linearFalloff (sqrMag-usedBrushFalloff, usedBrushSize);
			} else {
				falloff = 1f;
			}

			if (showVertexIndicators) {

				Handles.color = new Color (falloff, falloff, falloff);
				Handles.SphereCap (0, vertPos, Quaternion.identity, vertexIndicatorSize * falloff);

			}



		}

	}






/*
 *
 *		.----------------.  .----------------.  .----------------.  .-----------------. .----------------.  .----------------.  .----------------. 
 *		| .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. |
 *		| |   ______     | || |      __      | || |     _____    | || | ____  _____  | || |  _________   | || |  _________   | || |  _______     | |
 *		| |  |_   __ \   | || |     /  \     | || |    |_   _|   | || ||_   \|_   _| | || | |  _   _  |  | || | |_   ___  |  | || | |_   __ \    | |
 *		| |    | |__) |  | || |    / /\ \    | || |      | |     | || |  |   \ | |   | || | |_/ | | \_|  | || |   | |_  \_|  | || |   | |__) |   | |
 *		| |    |  ___/   | || |   / ____ \   | || |      | |     | || |  | |\ \| |   | || |     | |      | || |   |  _|  _   | || |   |  __ /    | |
 *		| |   _| |_      | || | _/ /    \ \_ | || |     _| |_    | || | _| |_\   |_  | || |    _| |_     | || |  _| |___/ |  | || |  _| |  \ \_  | |
 *		| |  |_____|     | || ||____|  |____|| || |    |_____|   | || ||_____|\____| | || |   |_____|    | || | |_________|  | || | |____| |___| | |
 *		| |              | || |              | || |              | || |              | || |              | || |              | || |              | |
 *		| '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' |
 *		'----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------' 
 */


	void addJobToDrawJobList(bool stepBackReset) {


		drawJobList.Add ((Color[])currentColors.Clone());

		if( stepBackReset )
			drawJobListStepBack = 0;
	}

	void undoDrawJob() {
		if (drawJobList.Count <= drawJobListStepBack + 1)
			return;

		drawJobListStepBack++;
		currentColors = drawJobList [drawJobList.Count - drawJobListStepBack - 1];


		currentGameObject.GetComponent<VertexColorStream> ().setColors (currentColors);



	}

	void redoDrawJob() {
		if (drawJobListStepBack < 1)
			return;

		drawJobListStepBack--;
		currentColors = drawJobList [drawJobList.Count - drawJobListStepBack - 1];

		currentGameObject.GetComponent<VertexColorStream> ().setColors (currentColors);


	}

	void saveColorsToStream() {

		EditorGUI.BeginChangeCheck();


		currentGameObject.GetComponent<VertexColorStream> ().setColors (currentColors);

			
		Undo.RegisterCompleteObjectUndo (currentGameObject, "Painted colors");

		

	}

	void cancelDrawing() {


		currentGameObject.GetComponent<VertexColorStream> ().setColors (cancelColors);
	
	}


	void getCurrentColorsFromStream () {


		currentColors = new Color[currentGameObject.GetComponent<VertexColorStream> ().getVertices().Length];
		cancelColors = new Color[currentGameObject.GetComponent<VertexColorStream> ().getVertices().Length];
		currentVertices = new Vector3[currentGameObject.GetComponent<VertexColorStream> ().getVertices().Length];

		currentGameObject.GetComponent<VertexColorStream> ().getColors().CopyTo(currentColors,0);
		currentGameObject.GetComponent<VertexColorStream> ().getColors().CopyTo(cancelColors,0);
		currentGameObject.GetComponent<VertexColorStream> ().getVertices().CopyTo(currentVertices,0);




	
	



	}

	bool hitWasOnLeaf = false;

	void drawBrush() {

		HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));

		Ray worldRay = HandleUtility.GUIPointToWorldRay (mousePos);
		if (Physics.Raycast (worldRay, out brushHit, Mathf.Infinity, 1 << 31)) {

			brushHitOnObject = true;

		} else {
			brushHitOnObject = false;

		}


		if (brushHit.transform != currentGameObject.transform.parent.transform)
			return;

		hitWasOnLeaf = hitIsLeaf (brushHit.triangleIndex);

		if ( drawOn != 2 && (drawOn == 0 && !hitWasOnLeaf) || (drawOn == 1 && hitWasOnLeaf))
			return;


		Handles.color = new Color (drawColor.r, drawColor.g, drawColor.b, Mathf.Pow(brushStrength,2f));
		Handles.DrawSolidDisc (brushHit.point, brushHit.normal, brushSize);

		Handles.color = Color.red;
		Handles.DrawWireDisc (brushHit.point, brushHit.normal, brushSize);
		Handles.DrawWireDisc (brushHit.point, brushHit.normal, brushFalloff);

	}

	bool hitIsLeaf(int triIndex) {

		Mesh tmpMesh = currentGameObject.GetComponent<MeshFilter> ().sharedMesh;
		Material[] tmpMaterials = currentGameObject.GetComponent<Renderer> ().sharedMaterials;
		int subMeshCount = tmpMesh.subMeshCount;

		int[] hittedTriangle = new int[] 
		{
			tmpMesh.triangles[triIndex * 3], 
			tmpMesh.triangles[triIndex * 3 + 1], 
			tmpMesh.triangles[triIndex * 3 + 2] 
		};
		for (int i = 0; i < subMeshCount; i++)
		{

			if (tmpMaterials [i].shader.name != "VTP/SpeedTree/Leaf")
				continue;

			int[] subMeshTris = tmpMesh.GetTriangles(i);
			for (int j = 0; j < subMeshTris.Length; j += 3)
			{
				if (subMeshTris[j] == hittedTriangle[0] &&
					subMeshTris[j + 1] == hittedTriangle[1] &&
					subMeshTris[j + 2] == hittedTriangle[2])
				{
					return true;
				}
			}
		}

		return false;


	}

	bool vertexIsLeaf(int vIndex, Mesh tmpMesh, Material[] tmpMaterials, int subMeshCount) {


		for (int i = 0; i < subMeshCount; i++)
		{

			if (tmpMaterials [i].shader.name != "VTP/SpeedTree/Leaf")
				continue;

			int[] subMeshTris = tmpMesh.GetTriangles(i);
			for (int j = 0; j < subMeshTris.Length; j += 3)
			{
				if (subMeshTris[j] == vIndex ||
					subMeshTris[j + 1] == vIndex ||
					subMeshTris[j + 2] == vIndex)
				{
					return true;
				}
			}
		}

		return false;


	}


	void drawVertexColor() {

		if ( drawOn != 2 && (drawOn == 0 && !hitWasOnLeaf) || (drawOn == 1 && hitWasOnLeaf))
			return;


		for( int i = 0 ; i < currentVertices.Length ; i++ ) {
			Vector3 vertPos = currentGameObject.transform.TransformPoint(currentVertices[i]);
			float sqrMag = Vector3.Distance(vertPos, brushHit.point);

			if( sqrMag > brushSize /*|| Mathf.Abs( Vector3.Angle( hit.normal, normals[i]) ) > 80*/ ) {
				continue;
			}

			//Debug.Log ("draw");

			float falloff = VPP_Utils.linearFalloff(sqrMag, brushSize);

			if (drawIndex == 0 || drawIndex == 1) {
				currentColors [i].r = VPP_Utils.VertexColorLerp (currentColors [i], drawColor, brushStrength * falloff).r;
			} else if (drawIndex > 1) {
				currentColors [i].g = VPP_Utils.VertexColorLerp (currentColors [i], drawColor, brushStrength * falloff).g;
				currentColors [i].b = VPP_Utils.VertexColorLerp (currentColors [i], drawColor, brushStrength * falloff).b;

				if (drawOn == 1 && !hitWasOnLeaf) {
					currentColors [i].a = VPP_Utils.VertexColorLerp (currentColors [i], drawColor, brushStrength * falloff).a;
				}

			} 

		}

		currentGameObject.GetComponent<VertexColorStream> ().setColors (currentColors);
	

	}

	void floodFillVertexColor() {

		//Debug.Log ("FloodFill");
		getCurrentColorsFromStream();

		Mesh tmpMesh = currentGameObject.GetComponent<MeshFilter> ().sharedMesh;
		Material[] tmpMaterials = currentGameObject.GetComponent<Renderer> ().sharedMaterials;
		int subMeshCount = tmpMesh.subMeshCount;


		for (int i = 0; i < currentColors.Length; i++) {


			if ( drawOn != 2 && (drawOn == 0 && !vertexIsLeaf(i,tmpMesh,tmpMaterials,subMeshCount)) || (drawOn == 1 && vertexIsLeaf(i,tmpMesh,tmpMaterials,subMeshCount)) )
				continue;

			if(drawOn != 2)
				EditorUtility.DisplayProgressBar("Casting Leaves and Branches", "Please wait", (float)i/(float)currentColors.Length);


			if (drawIndex == 0 || drawIndex == 1) {
				currentColors [i].r = drawColor.r;
			} else if (drawIndex > 1) {
				currentColors [i].g = drawColor.g;
				currentColors [i].b = drawColor.b;
			} 

		}

		currentGameObject.GetComponent<VertexColorStream> ().setColors (currentColors);

		EditorUtility.ClearProgressBar ();

		addJobToDrawJobList (true);

	}




	void doColorTransfer(GameObject source, GameObject target) {

		if (!source || !target) {
			Debug.LogWarning ("Source or Target GO not set");
			return;
		}

		if (!source.GetComponent<VertexColorStream>() ) {
			Debug.LogWarning ("Source not initialized yet. Trying to automatically instanciate it.");
			doTargetStreamSetup (source);
			return;
		}

		if (!target.GetComponent<VertexColorStream> ()) {
			doTargetStreamSetup (target);
		}

		Vector3[] targetVertices = new Vector3[target.GetComponent<VertexColorStream> ().getVertices().Length];
		target.GetComponent<VertexColorStream> ().getVertices().CopyTo(targetVertices,0);
		Color[] targetColors= new Color[target.GetComponent<VertexColorStream> ().getVertices().Length];
		target.GetComponent<VertexColorStream> ().getColors().CopyTo(targetColors,0);


		Vector3[] sourceVertices = new Vector3[source.GetComponent<VertexColorStream> ().getVertices().Length];
		source.GetComponent<VertexColorStream> ().getVertices().CopyTo(sourceVertices,0);
		Color[] sourceColors= new Color[source.GetComponent<VertexColorStream> ().getVertices().Length];
		source.GetComponent<VertexColorStream> ().getColors().CopyTo(sourceColors,0);

		for (int ti = 0; ti < targetVertices.Length; ti++) {

			EditorUtility.DisplayProgressBar("Casting Vertex Colors", "Please wait", (float)ti/(float)targetVertices.Length);

			float closestDistance = Mathf.Infinity;
			int closestIndex = -1;

			for (int si = 0; si < sourceVertices.Length; si++) {

				if( Vector3.Distance(sourceVertices[si], targetVertices[ti]) < closestDistance ) {
					closestDistance = Vector3.Distance (sourceVertices [si], targetVertices [ti]);
					closestIndex = si;
				}

			}

			targetColors[ti].r = sourceColors[closestIndex].r;
			targetColors[ti].g = sourceColors[closestIndex].g;
			targetColors[ti].b = sourceColors[closestIndex].b;


		}

		target.GetComponent<VertexColorStream> ().setColors (targetColors);


		foreach (Material mat in target.GetComponent<Renderer>().sharedMaterials) {

			if (mat.IsKeywordEnabled ("GEOM_TYPE_BRANCH") || mat.IsKeywordEnabled ("GEOM_TYPE_BRANCH_DETAIL") || mat.IsKeywordEnabled ("GEOM_TYPE_FROND")) {

				Texture tmpBase = mat.GetTexture ("_MainTex");
				Texture tmpNormal = mat.GetTexture ("_BumpMap");

				mat.shader = Shader.Find ("VTP/SpeedTree/Branch");
				mat.SetTexture ("_Albedo", tmpBase);
				mat.SetTexture ("_Normal", tmpNormal);
				mat.SetTexture ("_Albedo2", tmpBase);
				mat.SetTexture ("_Normal2", tmpNormal);
				mat.SetTexture ("_Albedo3", tmpBase);
				mat.SetTexture ("_Normal3", tmpNormal);
				mat.SetTexture ("_DetailTex", null);

			} else if ( mat.IsKeywordEnabled ("GEOM_TYPE_LEAF") ) {
				Texture tmpBase = mat.GetTexture ("_MainTex");
				Texture tmpNormal = mat.GetTexture ("_BumpMap");

				mat.shader = Shader.Find ("VTP/SpeedTree/Leaf");
				mat.SetTexture ("_Albedo", tmpBase);
				mat.SetTexture ("_Normal", tmpNormal);
				mat.SetTexture ("_Albedo2", tmpBase);
				mat.SetTexture ("_Normal2", tmpNormal);
				mat.SetTexture ("_Albedo3", tmpBase);
				mat.SetTexture ("_Normal3", tmpNormal);
				mat.SetTexture ("_DetailTex", null);
			}

		}



			

		EditorUtility.ClearProgressBar ();


	}

	void doTargetStreamSetup(GameObject target) {


		if (!target.GetComponent<MeshFilter> ()) {
			Debug.LogWarning ("No MeshFilter is attached to the target");
			return;
		}

		target.AddComponent<VertexColorStream> ();
		target.GetComponent<VertexColorStream> ().init (target.GetComponent<MeshFilter> ().sharedMesh, false);


		EditorUtility.SetDirty(target.GetComponent<VertexColorStream> ());


	}





/*
 *		.----------------.  .----------------.  .----------------.           .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------. 
 *		| .--------------. || .--------------. || .--------------. |         | .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. |
 *		| |  _________   | || |  _________   | || |  ____  ____  | |         | |      __      | || |    _______   | || |    _______   | || |     _____    | || |    _______   | || |  _________   | |
 *		| | |  _   _  |  | || | |_   ___  |  | || | |_  _||_  _| | |         | |     /  \     | || |   /  ___  |  | || |   /  ___  |  | || |    |_   _|   | || |   /  ___  |  | || | |  _   _  |  | |
 *		| | |_/ | | \_|  | || |   | |_  \_|  | || |   \ \  / /   | |         | |    / /\ \    | || |  |  (__ \_|  | || |  |  (__ \_|  | || |      | |     | || |  |  (__ \_|  | || | |_/ | | \_|  | |
 *		| |     | |      | || |   |  _|  _   | || |    > `' <    | |         | |   / ____ \   | || |   '.___`-.   | || |   '.___`-.   | || |      | |     | || |   '.___`-.   | || |     | |      | |
 *		| |    _| |_     | || |  _| |___/ |  | || |  _/ /'`\ \_  | |         | | _/ /    \ \_ | || |  |`\____) |  | || |  |`\____) |  | || |     _| |_    | || |  |`\____) |  | || |    _| |_     | |
 *		| |   |_____|    | || | |_________|  | || | |____||____| | |         | ||____|  |____|| || |  |_______.'  | || |  |_______.'  | || |    |_____|   | || |  |_______.'  | || |   |_____|    | |
 *		| |              | || |              | || |              | |         | |              | || |              | || |              | || |              | || |              | || |              | |
 *		| '--------------' || '--------------' || '--------------' |         | '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' |
 *		'----------------'  '----------------'  '----------------'           '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------' 
 */




	void generatePackedCombinedTexture() {

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Resizing Height Map", 0f);

		int textureSize = int.Parse(combinedSize [combinedSizeIndex]);

		Texture2D combinedTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);

		Color[] heightMapColors = { Color.red };
		if (thicknessMap) {
			#if UNITY_5_5_OR_NEWER
			TextureImporterCompression heightMapFormat = GetTextureFormatSettings (thicknessMap);
			SelectedChangeTextureFormatSettings (thicknessMap, TextureImporterCompression.Compressed);
			#else
			TextureImporterFormat heightMapFormat = GetTextureFormatSettings (heightMap);
			SelectedChangeTextureFormatSettings (heightMap, TextureImporterFormat.RGBA32);
			#endif
			Texture2D tmpHeightMap = Instantiate (ScaleTexture (thicknessMap, textureSize, textureSize));
			heightMapColors = tmpHeightMap.GetPixels ();
			DestroyImmediate (tmpHeightMap);
			SelectedChangeTextureFormatSettings (thicknessMap, heightMapFormat);
		}

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Resizing Occlusion Map", 1f/6f);

		Color[] OcclusionMapColors = { Color.red };
		if( OcclusionMap ) {
			#if UNITY_5_5_OR_NEWER
			TextureImporterCompression occlusionMapFormat = GetTextureFormatSettings (OcclusionMap);
			SelectedChangeTextureFormatSettings (OcclusionMap, TextureImporterCompression.Compressed);
			#else
			TextureImporterFormat occlusionMapFormat = GetTextureFormatSettings (OcclusionMap);
			SelectedChangeTextureFormatSettings (OcclusionMap, TextureImporterFormat.RGBA32);
			#endif
			Texture2D tmpOcclusionMap = Instantiate( ScaleTexture (OcclusionMap, textureSize, textureSize) );
			OcclusionMapColors = tmpOcclusionMap.GetPixels ();
			DestroyImmediate (tmpOcclusionMap);
			SelectedChangeTextureFormatSettings (OcclusionMap, occlusionMapFormat);
		}

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Resizing Smoothness Map",  2f/6f);

		Color[] smoothnessMapColors = { Color.red };
		if (smoothnessMap) {
			#if UNITY_5_5_OR_NEWER
			TextureImporterCompression smoothnessMapFormat = GetTextureFormatSettings (smoothnessMap);
			SelectedChangeTextureFormatSettings (smoothnessMap, TextureImporterCompression.Compressed);
			#else
			TextureImporterFormat smoothnessMapFormat = GetTextureFormatSettings (smoothnessMap);
			SelectedChangeTextureFormatSettings (smoothnessMap, TextureImporterFormat.RGBA32);
			#endif
			Texture2D tmpSmoothnessMap = Instantiate (ScaleTexture (smoothnessMap, textureSize, textureSize));
			smoothnessMapColors = tmpSmoothnessMap.GetPixels ();
			DestroyImmediate (tmpSmoothnessMap);
			SelectedChangeTextureFormatSettings (smoothnessMap, smoothnessMapFormat);
		}

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Resizing emission Map",  3f/6f);

		Color[] emissionMapColors = { Color.red };
		if (emissionMap) {
			#if UNITY_5_5_OR_NEWER
			TextureImporterCompression emissionMapFormat = GetTextureFormatSettings (emissionMap);
			SelectedChangeTextureFormatSettings (emissionMap, TextureImporterCompression.Compressed);
			#else
			TextureImporterFormat emissionMapFormat = GetTextureFormatSettings (emissionMap);
			SelectedChangeTextureFormatSettings (emissionMap, TextureImporterFormat.RGBA32);
			#endif
			Texture2D tmpEmissionMap = Instantiate (ScaleTexture (emissionMap, textureSize, textureSize));
			emissionMapColors = tmpEmissionMap.GetPixels ();
			DestroyImmediate (tmpEmissionMap);
			SelectedChangeTextureFormatSettings (emissionMap, emissionMapFormat);
		}

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Packing Combined Map",  4f/6f);


		Color[] combined_color = new Color[textureSize*textureSize];

		for( int i = 0 ; i < textureSize*textureSize ; i++ ) {

			if (thicknessMap) {
				combined_color [i].r = heightMapColors [i].r;
			} else {
				combined_color [i].r = 1;
			}

			if (OcclusionMap) {
				combined_color [i].g = OcclusionMapColors [i].r;
			} else {
				combined_color [i].g = 1;
			}

			if (smoothnessMap) {
				combined_color [i].b = smoothnessMapColors [i].r;
			} else {
				combined_color [i].b = 1;
			}

			if (emissionMap) {
				combined_color [i].a = emissionMapColors [i].r;
			} else {
				combined_color [i].a = 1;
			}


		}

		combinedTexture.SetPixels (combined_color);
		combinedTexture.Apply ();

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Saving Combined Map",  5f/6f);


		byte[] bytes = combinedTexture.EncodeToPNG();
		string savePath = "";
		if (outputName != "") {
			savePath = pathCombined + "/" + outputName + ".png";
		} else {
			savePath = pathCombined + "/Combined_Map.png";
		}

		File.WriteAllBytes(savePath, bytes);
		AssetDatabase.ImportAsset (savePath);

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Done", 1f);

		EditorUtility.ClearProgressBar ();

	}

	#if UNITY_5_5_OR_NEWER
	static TextureImporterCompression GetTextureFormatSettings(Texture2D texture) { 

		string path = AssetDatabase.GetAssetPath(texture); 
		TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter; 
		return textureImporter.textureCompression;

	}

	static void SelectedChangeTextureFormatSettings(Texture2D texture, TextureImporterCompression newFormat) { 

		string path = AssetDatabase.GetAssetPath(texture); 
		TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter; 
		textureImporter.textureCompression = newFormat;	
		textureImporter.isReadable = true;
		AssetDatabase.ImportAsset(path); 

	}
	#else
	static TextureImporterFormat GetTextureFormatSettings(Texture2D texture) { 

		string path = AssetDatabase.GetAssetPath(texture); 
		TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter; 
		return textureImporter.textureFormat;

	}

	static void SelectedChangeTextureFormatSettings(Texture2D texture, TextureImporterFormat newFormat) { 

		string path = AssetDatabase.GetAssetPath(texture); 
		TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter; 
		textureImporter.textureFormat = newFormat;	
		textureImporter.isReadable = true;
		AssetDatabase.ImportAsset(path); 

	}
	#endif


		

	/*
	 * http://answers.unity3d.com/questions/150942/texture-scale.html
	 */

	private Texture2D ScaleTexture(Texture2D source,int targetWidth,int targetHeight) {

		#if UNITY_5_5_OR_NEWER
		Texture2D result=new Texture2D(targetWidth,targetHeight, TextureFormat.RGBA32, true);
		#else
		Texture2D result=new Texture2D(targetWidth,targetHeight,source.format,true);
		#endif
		Color[] colors = new Color[targetWidth * targetHeight];
		int c = 0;

		for (int i = 0; i < result.height; ++i) {
			for (int j = 0; j < result.width; ++j) {
				colors[c] = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
				c++;
			}
		}

		result.SetPixels (colors);
		result.Apply();
		return result;
	}





















/*
 *		.----------------.  .----------------.  .-----------------. .----------------.  .----------------.  .----------------.  .----------------. 
 *		| .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. |
 *		| |    ______    | || |  _________   | || | ____  _____  | || |  _________   | || |  _______     | || |      __      | || |   _____      | |
 *		| |  .' ___  |   | || | |_   ___  |  | || ||_   \|_   _| | || | |_   ___  |  | || | |_   __ \    | || |     /  \     | || |  |_   _|     | |
 *		| | / .'   \_|   | || |   | |_  \_|  | || |  |   \ | |   | || |   | |_  \_|  | || |   | |__) |   | || |    / /\ \    | || |    | |       | |
 *		| | | |    ____  | || |   |  _|  _   | || |  | |\ \| |   | || |   |  _|  _   | || |   |  __ /    | || |   / ____ \   | || |    | |   _   | |
 *		| | \ `.___]  _| | || |  _| |___/ |  | || | _| |_\   |_  | || |  _| |___/ |  | || |  _| |  \ \_  | || | _/ /    \ \_ | || |   _| |__/ |  | |
 *		| |  `._____.'   | || | |_________|  | || ||_____|\____| | || | |_________|  | || | |____| |___| | || ||____|  |____|| || |  |________|  | |
 *		| |              | || |              | || |              | || |              | || |              | || |              | || |              | |
 *		| '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' |
 *		'----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------' 
 */




	/*
	 * http://stackoverflow.com/questions/29575964/setting-a-hierarchy-filter-via-script
	 */

	public const int FILTERMODE_ALL = 0;
	public const int FILTERMODE_NAME = 1;
	public const int FILTERMODE_TYPE = 2;

	public static SearchableEditorWindow hierarchy;

	public static void SetSearchFilter(string filter, int filterMode) {


		SearchableEditorWindow[] windows = (SearchableEditorWindow[])Resources.FindObjectsOfTypeAll (typeof(SearchableEditorWindow));



		foreach (SearchableEditorWindow window in windows) {

			if(window.GetType().ToString() == "UnityEditor.SceneHierarchyWindow") {

				hierarchy = window;
				break;
			}
		}

		if (hierarchy == null)
			return;

		MethodInfo setSearchType = typeof(SearchableEditorWindow).GetMethod("SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);         
		object[] parameters = new object[]{filter, filterMode, true};

		setSearchType.Invoke(hierarchy, parameters);

	}



}