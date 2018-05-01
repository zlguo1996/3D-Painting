using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
//using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class VT_window : EditorWindow {

	/* GUI Styles */
	private GUIStyle headerBoxStyle;
	private GUIStyle bodyH1Style;
	/* END GUI Styles */

	/* Global variables */
	private string toolState = "null";
	private string lastToolState = "null";
	private GameObject[] currentGameObjects;
	private string gameObjectState = "null";
	bool[] onFinishDeleteCollider;
	bool[] onFinishConvex;

	[SerializeField]
	private Vector2 scrollPosition;
	/* END Global variables */

	/* Paint Mode */

	/* General Settings */
	[SerializeField]
	private bool useAutoFocus = false;
	[SerializeField]
	private bool highlightGameObject = false;
	//[SerializeField]
	//	private bool showVertexColor = false;
	//[SerializeField]
	//private bool showHeightColor = false;
	[SerializeField]
	private bool showVertexIndicators = false;
	[SerializeField]
	private float vertexIndicatorSize = 1f;

	//[SerializeField]
	//private string[] drawChannels = new string[3] {"RGB(A)", "RGB", "Alpha"};
	[SerializeField]
	private int drawIndex = 0;
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
	/* END Brush Settings */

	/* END Paint Mode */


	/* Flow Mode */

	[SerializeField]
	private bool showFlowDataMesh = false;
	[SerializeField]
	private Texture2D flowMap;
	[SerializeField]
	bool paintFlow = false ;

	/* END Flow Mode */


	/* Painter */

	/* Global Variables */
	Tool lastUsedTool;
	int[] originalLayer;

	List<Color[][]> drawJobList;
	int drawJobListStepBack = 0;
	/* END Global Variables */

	/* Brush */
	private Vector2 mousePos = Vector2.zero;
	private RaycastHit brushHit;
	private bool brushHitOnObject = false;
	/* END Brush */

	/* Drawing Vertex Color */
	private Color[][] cancelColors;
	private Vector3[][] currentVertices;
	private Color[][] currentColors;
	/* END Drawing Vertex Color */

	/* END Painter */

	/* Wetness */
	private Vector2[][] cancelUV4s;
	private float wetnessStrength = 0.5f;
	private float wetnessHeight = 0.5f;

	/* END Wetness */

	/* Flow Mapper*/
	private Vector2[][] currentUVs;
	private Vector2[][] currentUV4s;
	private Vector4[][] currentTangents;



	private Vector2[][] isFlowPaintAffected;
	private float[][] flowPaintStartTimes; 
	private float paintFlowTotalTime;
	private Vector2 oldTexcoordHit = new Vector2 (-99, -99);

	[SerializeField]	
	private float flowBrushSize = 0.2f;
	[SerializeField]
	private float flowBrushStrength = 0.4f;

	/* END Flow Mapper*/

	/* Texture Assistent */
	[SerializeField]
	private string taState = "albedo";

	/* Albedo TA */
	[SerializeField]
	private Texture2D albedoMap;
	[SerializeField]
	private Texture2D specMetMap;
	[SerializeField]
	private ProceduralTexture albedoMapSubstance;
	[SerializeField]
	private ProceduralTexture specMetMapSubstance;
	private Vector2 scrollPositionAlbedo;
	/* END Albedo TA */

	/* Combined TA */
	[SerializeField]
	private Texture2D heightMap;
	[SerializeField]
	private Texture2D OcclusionMap;
	[SerializeField]
	private Texture2D smoothnessMap;
	[SerializeField]
	private Texture2D emissionMap;
	[SerializeField]
	private ProceduralTexture heightMapSubstance;
	[SerializeField]
	private ProceduralTexture OcclusionMapSubstance;
	[SerializeField]
	private ProceduralTexture smoothnessMapSubstance;
	[SerializeField]
	private ProceduralTexture emissionMapSubstance;

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

	/* Deformer */

	List<int[]> weldVertices;

	List<Vector3[][]> deformJobList;
	int deformJobListStepBack = 0;

	[SerializeField]
	private string deformMode = "intrude";

	[SerializeField]	
	private float deformBrushSize = 0.2f;
	[SerializeField]
	private float deformBrushStrength = 0.4f;
	private Vector3[][] cancelVertices;
	private Vector3[][] currentNormals;
	private int[][] currentTriangles;
	private Vector4[][] cancelTangents;

	private bool[][] affectedVerticesToSmooth;

	/* END Deformer */


	/* Refiner */

	private Vector2[][] currentUV2s;
	private Vector2[][] currentUV3s;
	private Color highlightColor = new Color(1f,0,0,0.25f);
	private Color edgeColor = Color.black;


	private Vector3[][] cancelNormals;
	private Vector2[][] cancelUVs;
	private Vector2[][] cancelUV2s;
	private Vector2[][] cancelUV3s;
	private int[][] cancelTriangles;

	/* END Refiner */


	void checkCurrentGameObject() {




		if (currentGameObjects != null && currentGameObjects.Length > 0 && Selection.gameObjects != null && Selection.gameObjects.Length > 0) {


			bool sameArray = true;
			for (int i = 0; i < currentGameObjects.Length; i++) {
				if (Selection.gameObjects.Length > i && currentGameObjects [i].gameObject != Selection.gameObjects[i])
					sameArray = false;
			}

			if (Selection.gameObjects.Length != currentGameObjects.Length)
				sameArray = false;

			if (sameArray) {

				bool everythingReady = true;
				for (int i = 0; i < currentGameObjects.Length; i++) {
					if (currentGameObjects [i].GetComponent<VertexColorStream> () == null)
						everythingReady = false;
				}

				if (everythingReady)
					return;

			}
		}

		currentGameObjects = Selection.gameObjects;



		bool hasMeshRenderer = true;
		for (int i = 0; i < currentGameObjects.Length; i++) {
			if (currentGameObjects [i].GetComponent<MeshRenderer> () == null)
				hasMeshRenderer = false;
		}


		if ( currentGameObjects == null || currentGameObjects.Length == 0 || !hasMeshRenderer) {
			lastToolState = toolState;

			gameObjectState = "null";

			if (toolState != "texassist") {
				toolState = "null";
			}

			return;
		} else {


			if (lastToolState == "null") {
				toolState = "paint";
				lastToolState = "paint";

			} else {
				toolState = lastToolState;
			}
		}

		bool hasVertexColorStream = true;
		for (int i = 0; i < currentGameObjects.Length; i++) {
			if (currentGameObjects [i].GetComponent<VertexColorStream> () == null)
				hasVertexColorStream = false;
		}



		if (!hasVertexColorStream) {
			gameObjectState = "null";
		} else {
			gameObjectState = "ready";
			if (lastToolState == "null") {
				toolState = "paint";
				lastToolState = "paint";

			} else {
				toolState = lastToolState;
			}

			for (int i = 0; i < currentGameObjects.Length; i++) {
				GameObject currentGameObject = currentGameObjects [i];
				if (currentGameObject != currentGameObject.transform.root.gameObject && !currentGameObject.transform.root.gameObject.GetComponent<VertexStreamChildrenRebuilder>() ) {
					currentGameObject.transform.root.gameObject.AddComponent<VertexStreamChildrenRebuilder> ();
				}
			}


		}


	}

	void Update() {

		checkCurrentGameObject ();

		Repaint ();

	}

	void guiForHeader() {
		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Vertex Tools Pro - Early Access", headerBoxStyle, GUILayout.Height(40), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();
	}

	void guiForPlayState() {
		GUI.Box(new Rect(10, 45, Screen.width-20, 25), "Vertex Tools are not available in playmode.");
	}


	void guiForNullState() {
		GUI.Box(new Rect(10, 90, Screen.width-20, 45), "In order to use Vertex Painter Pro 'Paint' and 'Flowmap' Features you first have to select a gameobject in your scene.");
	}

	void guiForGONullState() {
		GUI.Box(new Rect(10, 90, Screen.width-20, 45), "The selected gameobject has to be prepared to use Vertex Painting. Just Click below to have everything setup automatically for you.");
		GUILayout.Space (65);

		GUILayout.Space (5);
		if (GUILayout.Button ("Initialize everything now.", GUI.skin.button, GUILayout.Height (30))) {

			for (int i = 0; i < currentGameObjects.Length; i++) {
				GameObject currentGameObject = currentGameObjects [i];


				if (currentGameObject.GetComponent<VertexColorStream> ())
					continue;

				currentGameObject.AddComponent<VertexColorStream> ();

				Color[] _vertexColors = new Color[ currentGameObject.GetComponent<MeshFilter> ().sharedMesh.vertexCount];
				if( currentGameObject.GetComponent<MeshFilter> ().sharedMesh.colors.Length == 0 ) {
					for (int c = 0; c < _vertexColors.Length; c++) {

						_vertexColors [c] = new Color (1f, 0, 0, 1f);

					}

					currentGameObject.GetComponent<VertexColorStream> ().init (currentGameObject.GetComponent<MeshFilter> ().sharedMesh, false);
					currentGameObject.GetComponent<VertexColorStream> ().setColors (_vertexColors);
				} else {
					currentGameObject.GetComponent<VertexColorStream> ().init (currentGameObject.GetComponent<MeshFilter> ().sharedMesh, false);
					currentGameObject.GetComponent<VertexColorStream> ().setColors ( currentGameObject.GetComponent<MeshFilter> ().sharedMesh.colors );
				}




				EditorUtility.SetDirty(currentGameObject.GetComponent<VertexColorStream> ());
				//EditorSceneManager.MarkSceneDirty(currentGameObject.GetComponent<VertexColorStream> ().gameObject.scene);
				Undo.RegisterCompleteObjectUndo (currentGameObject, "Initialize");


				if (currentGameObject != currentGameObject.transform.root.gameObject && !currentGameObject.transform.root.gameObject.GetComponent<VertexStreamChildrenRebuilder>() ) {
					currentGameObject.transform.root.gameObject.AddComponent<VertexStreamChildrenRebuilder> ();
				}

				clearFlowData ();

			}






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



	}



	void guiForGeneralSettings () {

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("General Settings", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);



		useAutoFocus = EditorGUILayout.Toggle ("Auto Focus",useAutoFocus, GUI.skin.toggle);
		//highlightGameObject = EditorGUILayout.Toggle ("Highlight Gameobject",highlightGameObject, GUI.skin.toggle);
		//showVertexColor = EditorGUILayout.Toggle ("Show Vertex Color",showVertexColor && !showHeightColor, GUI.skin.toggle);
		//showHeightColor = EditorGUILayout.Toggle ("Show Height Data",showHeightColor && !showVertexColor, GUI.skin.toggle);
		showVertexIndicators = EditorGUILayout.Toggle ("Show Vertex Indicators",showVertexIndicators, GUI.skin.toggle);

		if( showVertexIndicators )
			vertexIndicatorSize = EditorGUILayout.Slider ("Vertex Indicator Size", vertexIndicatorSize, 0.01f, 1f);

		//useAnimator = EditorGUILayout.Toggle ("Use Vertex Color Animator", useAnimator, GUI.skin.toggle);

		//currentGameObject.GetComponent<MeshRenderer> ().sharedMaterial.SetFloat ("_showVertexColor", showVertexColor? 1f : 0f);
		//currentGameObject.GetComponent<MeshRenderer> ().sharedMaterial.SetFloat ("_showAlpha", showHeightColor? 1f : 0f );

		/*
		 * drawIndex = EditorGUI.Popup(
			new Rect(3,205, this.position.size.x - 6 , 20),
			"Draw to Channel:",
			drawIndex, 
			drawChannels);
		*/
		//GUILayout.Space (20);


	}

	void guiForBrushSettings () {

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Brush Settings", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);

		brushSize = EditorGUILayout.Slider ("Size", brushSize, 0.01f, 3f);
		GUILayout.Space (2);
		brushFalloff = EditorGUILayout.Slider ("Falloff", brushFalloff, 0.005f, brushSize);
		GUILayout.Space (2);
		brushStrength = EditorGUILayout.Slider ("Strength", brushStrength, 0f, 1f);
		GUILayout.Space (2);
		drawColor = EditorGUILayout.ColorField ("Draw with color: ", drawColor);
		GUILayout.Space (2);


		GUILayout.BeginHorizontal ();


		if (GUILayout.Button ("Red", GUILayout.Width (position.width/3/1.8f), GUILayout.Height (position.width/3/1.8f) )) {
			drawColor = new Color (1f, 0, 0, 1);
		}

		GUILayout.FlexibleSpace();

		if (GUILayout.Button ("Green", GUI.skin.button, GUILayout.Width (position.width / 3 / 1.8f), GUILayout.Height (position.width / 3 / 1.8f))) {
			drawColor = new Color (0, 1f, 0, 1);
		}

		GUILayout.FlexibleSpace();

		if (GUILayout.Button ("Blue", GUI.skin.button, GUILayout.Width (position.width / 3 / 1.8f), GUILayout.Height (position.width / 3 / 1.8f))) {
			drawColor = new Color (0, 0, 1f, 1);
		}

		GUILayout.FlexibleSpace();
		if (GUILayout.Button ("Toggle Alpha", GUI.skin.button,  GUILayout.Width (position.width/3), GUILayout.Height (position.width/3/1.75f) )) {
			drawColor = new Color ( drawColor.r, drawColor.g, drawColor.b, 1 - drawColor.a  );
		}
		GUILayout.EndHorizontal ();

		GUILayout.Space (10);

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Draw to Channel", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);

		GUILayout.BeginHorizontal ();
		{
			if ( GUILayout.Toggle (drawIndex == 0, "RGB(A)", GUI.skin.button, GUILayout.Height (30))) {
				drawIndex = 0;
			}
			if ( GUILayout.Toggle (drawIndex == 1, "RGB", GUI.skin.button, GUILayout.Height (30))) {
				drawIndex = 1;
			}
			if ( GUILayout.Toggle (drawIndex == 2, "Alpha (Height)", GUI.skin.button, GUILayout.Height (30))) {
				drawIndex = 2;
			}

		}
		GUILayout.EndHorizontal ();


	}

	void guiForPaintButton() {

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Vertex Painting", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);


		if ( (toolState == "paint") && GUILayout.Button ("Paint "+currentGameObjects.Length+" Objects", GUI.skin.button, GUILayout.Height (40))) {
			//Start painting
			toolState = "curPainting";


			onFinishDeleteCollider = new bool[currentGameObjects.Length];
			onFinishConvex = new bool[currentGameObjects.Length];
			originalLayer = new int[currentGameObjects.Length];

			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (!currentGameObject.GetComponent<MeshCollider> ()) {
					onFinishDeleteCollider[i] = true;
					currentGameObject.AddComponent<MeshCollider> ();
				} else {
					onFinishDeleteCollider[i] = false;

					if (currentGameObject.GetComponent<MeshCollider> ().convex) {
						onFinishConvex[i] = true;
						currentGameObject.GetComponent<MeshCollider> ().convex = false;
					} else {
						onFinishConvex[i] = false;
					}

				}


				originalLayer[i] = currentGameObject.layer;
				currentGameObject.layer = 31;

			}

			if( useAutoFocus )
				SceneView.lastActiveSceneView.FrameSelected();

			//if( highlightGameObject )
			//	SetSearchFilter (currentGameObject.name, 1);



			lastUsedTool = Tools.current;
			Tools.current = Tool.None;

			drawJobList = new List<Color[][]>();
			getCurrentColorsFromStream ();
			drawJobListStepBack = 0;
			addJobToDrawJobList (true);


		}

		if (toolState == "curPainting" && GUILayout.Button ("Save "+currentGameObjects.Length+" Objects", GUI.skin.button, GUILayout.Height (40))) {
			//Save painting
			saveColorsToStream();

			//if( highlightGameObject )
			//	SetSearchFilter ("", 0);

			Tools.current = lastUsedTool;

			toolState = "paint";


			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (onFinishDeleteCollider[i])
					DestroyImmediate (currentGameObject.GetComponent<MeshCollider> ());

				if (onFinishConvex[i] && currentGameObject.GetComponent<MeshCollider> ())
					currentGameObject.GetComponent<MeshCollider> ().convex = true;

				currentGameObject.layer = originalLayer[i];

			}
		}


		if (toolState == "curPainting" && GUILayout.Button ("Cancel without save", GUI.skin.button, GUILayout.Height (40))) {
			//Cancel painting
			cancelDrawing();

			if( highlightGameObject )
				SetSearchFilter ("", 0);

			Tools.current = lastUsedTool;

			toolState = "paint";

			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (onFinishDeleteCollider[i])
					DestroyImmediate (currentGameObject.GetComponent<MeshCollider> ());

				if (onFinishConvex[i] && currentGameObject.GetComponent<MeshCollider> ())
					currentGameObject.GetComponent<MeshCollider> ().convex = true;

				currentGameObject.layer = originalLayer[i];

			}

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

	void guiForFlowState() {


		GUILayout.BeginHorizontal ();
		GUILayout.Box ("", bodyH1Style, GUILayout.Height(10), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (2);

		if ( GUILayout.Button ("Clear Flow Data", GUI.skin.button, GUILayout.Height (30))) {
			clearFlowData ();
		}

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("", bodyH1Style, GUILayout.Height(5), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (15);

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Flowmap Processing", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (15);
		GUI.Box(new Rect(10, 105, Screen.width-20, 45), "FlowMap processing allows you to transform a given FlowMap (r:u, g:v) to vertex data to be used with the supplied shader.");
		GUILayout.Space (45);



		GUILayout.Space (5);

		//showFlowDataMesh = EditorGUILayout.Toggle ("Show Vertex Flow Data",showFlowDataMesh, GUI.skin.toggle);
		//currentGameObject.GetComponent<MeshRenderer> ().sharedMaterial.SetFloat ("_showVertexFlow", showFlowDataMesh? 1f : 0f );


		flowMap = (Texture2D) EditorGUILayout.ObjectField("FlowMap Texture", flowMap, typeof (Texture2D), false); 

		GUILayout.Space (5);

		if ( GUILayout.Button ("Transform FlowMap to vertex data", GUI.skin.button, GUILayout.Height (30))) {
			processFlowMap ();
		}

		GUILayout.Space (15);


		/*
		if (flowMap == null || !textureIsAccessible (flowMap)) {

			if( flowMap != null )
				GUI.Box(new Rect(10, 245, Screen.width-20, 45), "The selected FlowMap has no read/write access. Please check the import settings of the FlowMap and make sure its type is set to 'advanced' and read/write access is enabled.");

			return;
		}*/

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Mesh Processing", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();




		GUILayout.Space (5);



		if ( GUILayout.Button ("Process Mesh for FlowMap Data", GUI.skin.button, GUILayout.Height (30))) {
			processMeshForFlowData ();
		}

		GUILayout.Space (15);

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Flow Painting", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);


		flowBrushSize = EditorGUILayout.Slider ("Size", flowBrushSize, 0.01f, 3f);
		GUILayout.Space (2);
		flowBrushStrength = EditorGUILayout.Slider ("Strength", flowBrushStrength, 0f, 1f);
		GUILayout.Space (5);

		string flowPaintaLabel = "Paint flow data";
		if (paintFlow)
			flowPaintaLabel = "Save flow data";

		paintFlow = GUILayout.Toggle (paintFlow, flowPaintaLabel, GUI.skin.button ,GUILayout.Height(30));

		if (paintFlow) {
			toolState = "curFlowing";

			onFinishDeleteCollider = new bool[currentGameObjects.Length];
			onFinishConvex = new bool[currentGameObjects.Length];
			originalLayer = new int[currentGameObjects.Length];

			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (!currentGameObject.GetComponent<MeshCollider> ()) {
					onFinishDeleteCollider[i] = true;
					currentGameObject.AddComponent<MeshCollider> ();
				} else {
					onFinishDeleteCollider[i] = false;

					if (currentGameObject.GetComponent<MeshCollider> ().convex) {
						onFinishConvex[i] = true;
						currentGameObject.GetComponent<MeshCollider> ().convex = false;
					} else {
						onFinishConvex[i] = false;
					}

				}


				originalLayer[i] = currentGameObject.layer;
				currentGameObject.layer = 31;

			}

			if( useAutoFocus )
				SceneView.lastActiveSceneView.FrameSelected();



			lastUsedTool = Tools.current;
			Tools.current = Tool.None;

		} else {
			if( highlightGameObject )
				SetSearchFilter ("", 0);

			Tools.current = lastUsedTool;

			toolState = "flow";

			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (onFinishDeleteCollider.Length > i && onFinishDeleteCollider[i])
					DestroyImmediate (currentGameObject.GetComponent<MeshCollider> ());

				if (onFinishConvex.Length > i && onFinishConvex[i] && currentGameObject.GetComponent<MeshCollider> ())
					currentGameObject.GetComponent<MeshCollider> ().convex = true;

				if(originalLayer.Length > i )
					currentGameObject.layer = originalLayer[i];

			}

		}



	}

	void guiForTAState() {

		if (taState == "albedo") {
			scrollPositionAlbedo = GUILayout.BeginScrollView (scrollPositionAlbedo, false, false, GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true));
		} else {
			scrollPositionCombined = GUILayout.BeginScrollView (scrollPositionCombined, false, false, GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true));
		}
		GUILayout.Space (15);
		GUI.Box(new Rect(10, 5, Screen.width-20, 45), "The Texture Assistant allows you to create the required packed textures out of single grayscaled maps.");
		GUILayout.Space (45);

		GUILayout.BeginVertical();
		{
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Toggle(taState == "albedo", "Albedo Texture Packer", EditorStyles.toolbarButton))
					taState = "albedo";

				if (GUILayout.Toggle (taState == "combined", "Combined Texture Packer", EditorStyles.toolbarButton))
					taState = "combined";

				if (GUILayout.Toggle (taState == "substance", "Substance Texture Packer", EditorStyles.toolbarButton))
					taState = "substance";

			}
			GUILayout.EndHorizontal(); 
		}
		GUILayout.EndVertical ();

		if (taState == "albedo")
			guiForAlbedoTAState ();

		if (taState == "combined")
			guiForCombinedTAState ();

		if (taState == "substance") 
			guiForTAAlbedoSubstance ();

		GUILayout.EndScrollView ();

	}



	void guiForAlbedoTAState() {

		GUILayout.Space (5);

		/*GUILayout.BeginVertical();
		{
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Toggle(taMode == "default", "Default Textures", EditorStyles.toolbarButton))
					taMode = "default";

				if (GUILayout.Toggle (taMode == "substance", "Substance Textures", EditorStyles.toolbarButton))
					taMode = "substance";

			}
			GUILayout.EndHorizontal(); 
		}
		GUILayout.EndVertical ();*/



		//if (taMode == "default") {
		guiForTAAlbedoDefault ();
		//} else {
		//	guiForTAAlbedoSubstance ();
		//}


	}

	void guiForTAAlbedoDefault() {
		float editorWidth = position.width;

		float textureWidthAndHeight = editorWidth / 2 - 40;
		float editorGUIHeight = 145;

		GUILayout.Space (15);

		GUILayout.BeginHorizontal ();
		{

			GUILayout.BeginVertical ();
			{
				GUILayout.Box ("Albedo Map", bodyH1Style, GUILayout.Height (20), GUILayout.ExpandWidth (true));
				albedoMap =  (Texture2D)EditorGUI.ObjectField(new Rect(15,editorGUIHeight,textureWidthAndHeight,textureWidthAndHeight), albedoMap, typeof(Texture2D), false );
			}
			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			{
				GUILayout.Box ("Metallic Map", bodyH1Style, GUILayout.Height (20), GUILayout.ExpandWidth (true));
				specMetMap =  (Texture2D)EditorGUI.ObjectField(new Rect(15+editorWidth/2,editorGUIHeight,textureWidthAndHeight,textureWidthAndHeight), specMetMap, typeof(Texture2D), false );
			}
			GUILayout.EndVertical ();

		}
		GUILayout.EndHorizontal ();

		editorGUIHeight += textureWidthAndHeight ;
		GUILayout.Space (textureWidthAndHeight+25);


		if (!albedoMap || !specMetMap ) {
			editorGUIHeight += 10;
			GUI.Box(new Rect(10, editorGUIHeight, editorWidth-20-15, 30), "Warning: You are missing one or more maps.");
			GUILayout.Space (40);
			editorGUIHeight += 25;
		}



		GUILayout.Box ("Generate Map", bodyH1Style, GUILayout.Height (20), GUILayout.ExpandWidth (true));


		editorGUIHeight += 60;

		combinedSizeIndex = EditorGUI.Popup(
			new Rect(3,editorGUIHeight, editorWidth - 6 , 20),
			"Combined Map Size:",
			combinedSizeIndex, 
			combinedSize);

		editorGUIHeight += 25;

		if ( (albedoMap || specMetMap ) ) {

			if (albedoMap) {
				objectForPathCombined = albedoMap;
			} else if (specMetMap) {
				objectForPathCombined = specMetMap;
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
			generatePackedAlbedoTexture();
		}
		GUILayout.Space (10);
	}

	ProceduralMaterial pM;

	void guiForTAAlbedoSubstance() {
		float editorWidth = position.width;

		float editorGUIHeight = 125;

		GUILayout.Space (15);
		GUILayout.Box ("Substance Procedural Material", bodyH1Style, GUILayout.Height (20), GUILayout.ExpandWidth (true));

		pM = (ProceduralMaterial)EditorGUI.ObjectField(new Rect(15,editorGUIHeight,300,15), pM, typeof(ProceduralMaterial), false );

		GUILayout.Space (30);

		if (!pM ) {
			return;
		}



		GUILayout.Box ("Generate Map", bodyH1Style, GUILayout.Height (20), GUILayout.ExpandWidth (true));


		editorGUIHeight += 60;

		combinedSizeIndex = EditorGUI.Popup(
			new Rect(3,editorGUIHeight, editorWidth - 6 , 20),
			"Combined Map Size:",
			combinedSizeIndex, 
			combinedSize);

		editorGUIHeight += 25;

		if ( pM ) {

			objectForPathCombined = pM;

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

		GUILayout.Space (50);



		if (objectForPathCombined && GUILayout.Button ("Generate packed Combined Map", GUI.skin.button, GUILayout.Height (40))) {
			generatePackedAlbedoSubstanceTexture();
			generatePackedCombinedSubstanceTexture();
		}
		GUILayout.Space (10);
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
				GUILayout.Box ("Height Map", bodyH1Style, GUILayout.Height (20), GUILayout.ExpandWidth (true));
				heightMap =  (Texture2D)EditorGUI.ObjectField(new Rect(15,editorGUIHeight,textureWidthAndHeight,textureWidthAndHeight), heightMap, typeof(Texture2D), false );

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

		if (!heightMap || !OcclusionMap || !smoothnessMap || !emissionMap) {
			GUI.Box(new Rect(10, editorGUIHeight, editorWidth-20-15, 30), "Warning: You are missing one or more maps.");
			GUILayout.Space (40);
			editorGUIHeight += 40;
		}

		/*bool allAccessible = true;

		if( (heightMap && !textureIsAccessible(heightMap)) || (OcclusionMap && !textureIsAccessible(OcclusionMap)) || (smoothnessMap && !textureIsAccessible(smoothnessMap)) || (emissionMap && !textureIsAccessible(emissionMap)) ) {
			GUI.Box(new Rect(10, editorGUIHeight, editorWidth-20-15, 60), "One or more selected Maps have no read/write access. Please check the import settings of all Maps and make sure its type is set to 'advanced' and read/write access is enabled.");
			GUILayout.Space (70);
			editorGUIHeight += 70;
			allAccessible = false;
		}

		if (!allAccessible)
			return;*/

		GUILayout.Box ("Generate Map", bodyH1Style, GUILayout.Height (20), GUILayout.ExpandWidth (true));


		editorGUIHeight += 45;

		combinedSizeIndex = EditorGUI.Popup(
			new Rect(3,editorGUIHeight, editorWidth - 6 , 20),
			"Combined Map Size:",
			combinedSizeIndex, 
			combinedSize);

		editorGUIHeight += 25;

		if ( (heightMap || OcclusionMap || smoothnessMap || emissionMap)) {

			if (heightMap) {
				objectForPathCombined = heightMap;
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

		if (toolState != "null" && currentGameObjects != null) {

			GUILayout.BeginVertical ();
			{
				GUILayout.BeginHorizontal ();
				{
					if (GUILayout.Toggle (toolState == "paint", "Paint", EditorStyles.toolbarButton) && toolState != "curPainting" && toolState != "curDeforming" && toolState != "curFlowing") {
						toolState = "paint";
					}

					//if (!currentGameObject.GetComponent<Renderer> ().sharedMaterial.HasProperty ("_wetnessEdgeBlend")) {
					if (GUILayout.Toggle (toolState == "flow", "Flow Mapping", EditorStyles.toolbarButton) && toolState != "curPainting" && toolState != "curDeforming" && toolState != "curFlowing")
						toolState = "flow";
					//} else {
					if (GUILayout.Toggle (toolState == "wet", "Wetness Painting", EditorStyles.toolbarButton) && toolState != "curWetting" && toolState != "curDeforming" && toolState != "curFlowing") {
						toolState = "wet";
					}
					//}

					if (GUILayout.Toggle (toolState == "texassist", "Texture Assistant", EditorStyles.toolbarButton) && toolState != "curPainting" && toolState != "curDeforming" && toolState != "curFlowing")
						toolState = "texassist";
				}
				GUILayout.EndHorizontal (); 
			}
			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			{
				GUILayout.BeginHorizontal ();
				{
					if (GUILayout.Toggle (toolState == "deform", "Deform", EditorStyles.toolbarButton) && toolState != "curPainting" && toolState != "curDeforming" && toolState != "curFlowing")
						toolState = "deform";

					if (GUILayout.Toggle (toolState == "refine", "Refine", EditorStyles.toolbarButton) && toolState != "curPainting" && toolState != "curDeforming" && toolState != "curFlowing")
						toolState = "refine";

				}
				GUILayout.EndHorizontal (); 
			}
			GUILayout.EndVertical ();

		} else {

			GUILayout.BeginVertical ();
			{
				GUILayout.BeginHorizontal ();
				{
					if (GUILayout.Toggle (toolState == "texassist", "Texture Assistant", EditorStyles.toolbarButton) && toolState != "curPainting" && toolState != "curDeforming" && toolState != "curFlowing")
						toolState = "texassist";
				}
				GUILayout.EndHorizontal (); 
			}
			GUILayout.EndVertical ();


		}

	}

	void guiForDeform() {

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("General Settings", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (2);
		showVertexIndicators = EditorGUILayout.Toggle ("Show Vertex Indicators",showVertexIndicators, GUI.skin.toggle);

		if( showVertexIndicators )
			vertexIndicatorSize = EditorGUILayout.Slider ("Vertex Indicator Size", vertexIndicatorSize, 0.01f, 1f);
		GUILayout.Space (10);


		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Brush Settings", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();


		deformBrushSize = EditorGUILayout.Slider ("Size", deformBrushSize, 0.01f, 3f);
		GUILayout.Space (2);
		deformBrushStrength = EditorGUILayout.Slider ("Strength", deformBrushStrength, 0f, 1f);
		GUILayout.Space (2);

		GUILayout.BeginHorizontal ();
		{
			if ( GUILayout.Toggle (deformMode == "intrude", "Extrude", GUI.skin.button, GUILayout.Height (30))) {
				//Undo
				deformMode = "intrude";
			}
			if ( GUILayout.Toggle (deformMode == "raise", "Raise", GUI.skin.button, GUILayout.Height (30))) {
				//Undo
				deformMode = "raise";
			}
			if ( GUILayout.Toggle (deformMode == "pinch", "Pinch", GUI.skin.button, GUILayout.Height (30))) {
				//Redo
				deformMode = "pinch";
			}
			if ( GUILayout.Toggle (deformMode == "push", "Push", GUI.skin.button, GUILayout.Height (30))) {
				//Redo
				deformMode = "push";
			}
			if ( GUILayout.Toggle (deformMode == "smooth", "Smooth", GUI.skin.button, GUILayout.Height (30))) {
				//Redo
				deformMode = "smooth";
			}
		}
		GUILayout.EndHorizontal ();

		GUILayout.Space (10);


		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Vertex Mesh Deforming", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);


		if (toolState == "deform" && GUILayout.Button ("Deform "+currentGameObjects.Length+" Objects", GUI.skin.button, GUILayout.Height (40))) {
			//Start painting
			toolState = "curDeforming";

			onFinishDeleteCollider = new bool[currentGameObjects.Length];
			onFinishConvex = new bool[currentGameObjects.Length];
			originalLayer = new int[currentGameObjects.Length];


			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (!currentGameObject.GetComponent<MeshCollider> ()) {
					onFinishDeleteCollider[i] = true;
					currentGameObject.AddComponent<MeshCollider> ();
				} else {
					onFinishDeleteCollider[i] = false;

					if (currentGameObject.GetComponent<MeshCollider> ().convex) {
						onFinishConvex[i] = true;
						currentGameObject.GetComponent<MeshCollider> ().convex = false;
					} else {
						onFinishConvex[i] = false;
					}

				}

				//getVerticesToWeld ();


				originalLayer[i] = currentGameObject.layer;
				currentGameObject.layer = 31;

			}

			if( useAutoFocus )
				SceneView.lastActiveSceneView.FrameSelected();


			lastUsedTool = Tools.current;
			Tools.current = Tool.None;


			deformJobList = new List<Vector3[][]>();
			getCurrentVerticesFromStream();
			deformJobListStepBack = 0;
			addJobToDeformJobList (true);


		}

		if (toolState == "curDeforming" && GUILayout.Button ("Save '"+currentGameObjects.Length+" Objects", GUI.skin.button, GUILayout.Height (40))) {
			//Save deforming
			saveVerticesToStream();

			if( highlightGameObject )
				SetSearchFilter ("", 0);

			Tools.current = lastUsedTool;

			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (onFinishDeleteCollider[i])
					DestroyImmediate (currentGameObject.GetComponent<MeshCollider> ());

				if (onFinishConvex[i] && currentGameObject.GetComponent<MeshCollider> ())
					currentGameObject.GetComponent<MeshCollider> ().convex = true;

				currentGameObject.layer = originalLayer[i];

			}

			toolState = "deform";
		}


		if (toolState == "curDeforming" && GUILayout.Button ("Cancel without save", GUI.skin.button, GUILayout.Height (40))) {
			//Cancel deforming
			cancelDeforming();

			if( highlightGameObject )
				SetSearchFilter ("", 0);

			Tools.current = lastUsedTool;

			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (onFinishDeleteCollider[i])
					DestroyImmediate (currentGameObject.GetComponent<MeshCollider> ());

				if (onFinishConvex[i] && currentGameObject.GetComponent<MeshCollider> ())
					currentGameObject.GetComponent<MeshCollider> ().convex = true;

				currentGameObject.layer = originalLayer[i];

			}

			toolState = "deform";

		}


		if (toolState != "curDeforming")
			return;

		GUILayout.Space (15);

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Deform Commands", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);




		GUILayout.BeginHorizontal ();
		{
			if (toolState == "curDeforming" && GUILayout.Button ("Undo", GUI.skin.button, GUILayout.Height (30))) {
				//Undo
				undoDeformJob();
			}
			if (toolState == "curDeforming" && GUILayout.Button ("Redo", GUI.skin.button, GUILayout.Height (30))) {
				//Redo
				redoDeformJob ();
			}
		}
		GUILayout.EndHorizontal ();

	}


	void guiForRefine() {


		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Brush Settings", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();


		highlightColor = EditorGUILayout.ColorField ("Highlight Color: ", highlightColor);
		GUILayout.Space (2);
		edgeColor = EditorGUILayout.ColorField ("Edge Color: ", edgeColor);
		GUILayout.Space (2);


		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Vertex Mesh Refining", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);


		if (toolState == "refine" && GUILayout.Button ("Refine "+currentGameObjects.Length+" Objects", GUI.skin.button, GUILayout.Height (40))) {
			//Start painting
			toolState = "curRefining";

			onFinishDeleteCollider = new bool[currentGameObjects.Length];
			onFinishConvex = new bool[currentGameObjects.Length];
			originalLayer = new int[currentGameObjects.Length];


			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (!currentGameObject.GetComponent<MeshCollider> ()) {
					onFinishDeleteCollider[i] = true;
					currentGameObject.AddComponent<MeshCollider> ();
				} else {
					onFinishDeleteCollider[i] = false;

					if (currentGameObject.GetComponent<MeshCollider> ().convex) {
						onFinishConvex[i] = true;
						currentGameObject.GetComponent<MeshCollider> ().convex = false;
					} else {
						onFinishConvex[i] = false;
					}

				}

				getCurrentVerticesFromStreamForRefine ();

				originalLayer[i] = currentGameObject.layer;
				currentGameObject.layer = 31;

			}

			if( useAutoFocus )
				SceneView.lastActiveSceneView.FrameSelected();


			lastUsedTool = Tools.current;
			Tools.current = Tool.None;


			deformJobList = new List<Vector3[][]>();
			getCurrentVerticesFromStream();
			deformJobListStepBack = 0;
			addJobToDeformJobList (true);


		}

		if (toolState == "curRefining" && GUILayout.Button ("Save '"+currentGameObjects.Length+" Objects", GUI.skin.button, GUILayout.Height (40))) {
			//Save deforming
			saveVerticesToStreamFromRefine();

			if( highlightGameObject )
				SetSearchFilter ("", 0);

			Tools.current = lastUsedTool;

			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (onFinishDeleteCollider[i])
					DestroyImmediate (currentGameObject.GetComponent<MeshCollider> ());

				if (onFinishConvex[i] && currentGameObject.GetComponent<MeshCollider> ())
					currentGameObject.GetComponent<MeshCollider> ().convex = true;

				currentGameObject.layer = originalLayer[i];

			}

			toolState = "refine";
		}


		if (toolState == "curRefining" && GUILayout.Button ("Cancel without save", GUI.skin.button, GUILayout.Height (40))) {
			//Cancel deforming
			cancelRefining();

			if( highlightGameObject )
				SetSearchFilter ("", 0);

			Tools.current = lastUsedTool;

			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (onFinishDeleteCollider[i])
					DestroyImmediate (currentGameObject.GetComponent<MeshCollider> ());

				if (onFinishConvex[i] && currentGameObject.GetComponent<MeshCollider> ())
					currentGameObject.GetComponent<MeshCollider> ().convex = true;

				currentGameObject.layer = originalLayer[i];

			}

			toolState = "refine";

		}



	}



	void guiForWetState() {


		guiForGeneralSettingsWet();
		GUILayout.Space (15);

		guiForBrushSettingsWet ();
		GUILayout.Space (15);

		guiForWetnessSettings ();
		GUILayout.Space (15);

		guiForPaintButtonWet ();
		GUILayout.Space (15);

	}

	void guiForGeneralSettingsWet () {

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("General Settings", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);



		useAutoFocus = EditorGUILayout.Toggle ("Auto Focus",useAutoFocus, GUI.skin.toggle);
		highlightGameObject = EditorGUILayout.Toggle ("Highlight Gameobject",highlightGameObject, GUI.skin.toggle);
		showVertexIndicators = EditorGUILayout.Toggle ("Show Vertex Indicators",showVertexIndicators, GUI.skin.toggle);

		if( showVertexIndicators )
			vertexIndicatorSize = EditorGUILayout.Slider ("Vertex Indicator Size", vertexIndicatorSize, 0.01f, 1f);

		//useAnimator = EditorGUILayout.Toggle ("Use Vertex Color Animator", useAnimator, GUI.skin.toggle);

		//currentGameObject.GetComponent<MeshRenderer> ().sharedMaterial.SetFloat ("_showVertexColor", showVertexColor? 1f : 0f);
		//currentGameObject.GetComponent<MeshRenderer> ().sharedMaterial.SetFloat ("_showAlpha", showHeightColor? 1f : 0f );

		/*
		 * drawIndex = EditorGUI.Popup(
			new Rect(3,205, this.position.size.x - 6 , 20),
			"Draw to Channel:",
			drawIndex, 
			drawChannels);
		*/
		//GUILayout.Space (20);


	}

	void guiForBrushSettingsWet () {

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Brush Settings", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);

		brushSize = EditorGUILayout.Slider ("Size", brushSize, 0.01f, 3f);
		GUILayout.Space (2);
		brushFalloff = EditorGUILayout.Slider ("Falloff", brushFalloff, 0.005f, brushSize);
		GUILayout.Space (2);
		brushStrength = EditorGUILayout.Slider ("Strength", brushStrength, 0f, 1f);
		GUILayout.Space (2);




	}


	void guiForWetnessSettings () {

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Wetness Settings", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);

		wetnessStrength = EditorGUILayout.Slider ("Wetness Strength", wetnessStrength, 0f, 1f);
		GUILayout.Space (2);
		wetnessHeight = EditorGUILayout.Slider ("Wetness Height", wetnessHeight, 0f, 1f);
		GUILayout.Space (2);





	}


	void guiForPaintButtonWet() {

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Wetness Painting", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);


		if ( (toolState == "wet") && GUILayout.Button ("Paint wetness on "+currentGameObjects.Length+" Objects", GUI.skin.button, GUILayout.Height (40))) {
			//Start painting
			toolState = "curWetting";

			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (!currentGameObject.GetComponent<MeshCollider> ()) {
					onFinishDeleteCollider[i] = true;
					currentGameObject.AddComponent<MeshCollider> ();
				} else {
					onFinishDeleteCollider[i] = false;

					if (currentGameObject.GetComponent<MeshCollider> ().convex) {
						onFinishConvex[i] = true;
						currentGameObject.GetComponent<MeshCollider> ().convex = false;
					} else {
						onFinishConvex[i] = false;
					}

				}


				originalLayer[i] = currentGameObject.layer;
				currentGameObject.layer = 31;

			}

			if( useAutoFocus )
				SceneView.lastActiveSceneView.FrameSelected();


			lastUsedTool = Tools.current;
			Tools.current = Tool.None;

			drawJobList = new List<Color[][]>();
			getCurrentColorsFromStream ();
			drawJobListStepBack = 0;
			addJobToDrawJobList (true);


		}

		if (toolState == "curWetting" && GUILayout.Button ("Save "+currentGameObjects.Length+" Objects", GUI.skin.button, GUILayout.Height (40))) {
			//Save painting
			saveColorsToStream();

			if( highlightGameObject )
				SetSearchFilter ("", 0);

			Tools.current = lastUsedTool;

			toolState = "wet";

			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (onFinishDeleteCollider[i])
					DestroyImmediate (currentGameObject.GetComponent<MeshCollider> ());

				if (onFinishConvex[i] && currentGameObject.GetComponent<MeshCollider> ())
					currentGameObject.GetComponent<MeshCollider> ().convex = true;

				currentGameObject.layer = originalLayer[i];

			}
		}


		if (toolState == "curWetting" && GUILayout.Button ("Cancel without save", GUI.skin.button, GUILayout.Height (40))) {
			//Cancel painting
			cancelDrawing();

			if( highlightGameObject )
				SetSearchFilter ("", 0);

			Tools.current = lastUsedTool;

			toolState = "wet";

			for (int i = 0; i < currentGameObjects.Length; i++) {

				GameObject currentGameObject = currentGameObjects [i];

				if (onFinishDeleteCollider[i])
					DestroyImmediate (currentGameObject.GetComponent<MeshCollider> ());

				if (onFinishConvex[i] && currentGameObject.GetComponent<MeshCollider> ())
					currentGameObject.GetComponent<MeshCollider> ().convex = true;

				currentGameObject.layer = originalLayer[i];

			}

		}

		if (toolState != "curWetting")
			return;

		GUILayout.Space (15);

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("Draw Commands", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);


		if (toolState == "curWetting" && GUILayout.Button ("Flood Fill with Color", GUI.skin.button, GUILayout.Height (40))) {
			//Flood Fill
			floodFillVertexColor ();
		}



	}




	void OnGUI() {



		/* Draw Header */
		guiForHeader ();

		if (Application.isPlaying) {
			guiForPlayState ();
			return;
		}


		if ( currentGameObjects == null && toolState != "texassist" ) {
			GUILayout.Space (5);
			guiForTabs ();
			guiForNullState ();
			return;
		}

		bool hasMeshRenderer = true;
		for (int i = 0; i < currentGameObjects.Length; i++) {
			if (currentGameObjects [i].GetComponent<MeshRenderer> () == null)
				hasMeshRenderer = false;
		}


		if ( (currentGameObjects == null || ( currentGameObjects != null && !hasMeshRenderer))  && toolState != "texassist" ) {
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

		GUILayout.Space (10);
		guiForTabs ();

		scrollPosition = GUILayout.BeginScrollView (scrollPosition, false, false, GUILayout.ExpandWidth (true), GUILayout.ExpandHeight (true));

		if ((toolState == "paint" || toolState == "curPainting") && gameObjectState != "null") {
			if (drawIndex == 3) {
				drawIndex = 0;
			}

			guiForPaintState ();
		}


		if ((toolState == "wet" || toolState == "curWetting" ) && gameObjectState != "null") {
			if (drawIndex != 3) {
				drawIndex = 3;
			}

			guiForWetState ();
		}

		if ( (toolState == "flow" || toolState == "curFlowing") && gameObjectState != "null" )
			guiForFlowState ();

		if (toolState == "texassist")
			guiForTAState ();

		if ( (toolState == "deform" || toolState == "curDeforming") && gameObjectState != "null")
			guiForDeform();

		if ( (toolState == "refine" || toolState == "curRefining") && gameObjectState != "null")
			guiForRefine();

		lastToolState = toolState;

		GUILayout.EndScrollView ();

		GUILayout.FlexibleSpace ();
		guiForVersion ();


	}

	void guiForVersion() {

		GUILayout.Box ("", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.Box ("Version 2.0.3b1", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));

	}

	void OnSceneGUI(SceneView sceneView) {

		ProcessInputs ();

		if (toolState == "curPainting" || toolState == "curWetting") {
			drawBrush ();
			drawAffectedVertices ();
		}

		if (toolState != "null" && gameObjectState != "null" && showFlowDataMesh )
			drawFlowData ();

		if (toolState == "curPainting" && drawIndex == 2) {

			Handles.BeginGUI ();
			GUILayout.BeginArea (new Rect (10, sceneView.position.height-70, sceneView.position.width-20, 40));

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Warning: You are drawing to the alpha (height) channel only. Click here to switch back to rgb(a) and alpha back to 1.", GUILayout.Height (40))) {
				drawIndex = 0;
				drawColor.a = 1f;
			}

			GUILayout.EndHorizontal ();

			GUILayout.EndArea ();
			Handles.EndGUI ();

		}


		if (toolState == "curDeforming") {
			drawDeformBrush ();
			drawAffectedVertices ();
		}

		if (toolState == "curRefining") {
			drawRefineBrush ();
		}

		if( toolState == "curFlowing" )
			drawFlowBrush ();

		sceneView.Repaint ();
	}

	public static void LaunchVT_window() {

		var win = EditorWindow.GetWindow<VT_window> (false, "VertexToolsPro - Early Access", true);
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

			if( toolState == "curDeforming" && brushHitOnObject )
				addJobToDeformJobList (true);

			isFlowPaintAffected = new Vector2[0][];
			flowPaintStartTimes = new float[0][];
			paintFlowTotalTime = 0;

		}

		if (toolState == "curFlowing" && brushHitOnObject &&  (e.type == EventType.MouseDrag || e.type == EventType.mouseDown) && e.button == 0 && !e.shift && !e.alt && !e.control) {



			if (oldTexcoordHit.x == -99f && oldTexcoordHit.y == -99f)
				oldTexcoordHit = brushHit.textureCoord;


			paintFlowDataV3();

			//paintFlowData (mouseDif.x, mouseDif.z);
			//paintFlowDataV2 (hitPoint, mouseDif.magnitude);

			//Debug.Log ("X:" + mouseDif.x + " | " + "Z:" + mouseDif.z);
			//oldHitPoint = hitPoint;

		}

		if( (toolState == "curPainting" || toolState == "curWetting") && brushHitOnObject &&  (e.type == EventType.MouseDrag || e.type == EventType.mouseDown) && e.button == 0 && !e.shift && !e.alt && !e.control) {

			drawVertexColor ();
			drawJobListStepBack = 0;
		}

		if( toolState == "curDeforming" && deformMode != "smooth" && brushHitOnObject &&  (e.type == EventType.MouseDrag || e.type == EventType.mouseDown) && e.button == 0 && !e.shift && !e.alt && !e.control) {

			deformVertices (1);
			deformJobListStepBack = 0;
		} else if ( toolState == "curDeforming" && deformMode != "smooth" && brushHitOnObject &&  (e.type == EventType.MouseDrag || e.type == EventType.mouseDown) && e.button == 0 && e.control && !e.shift && !e.alt) {

			deformVertices (-1);
			deformJobListStepBack = 0;
		}

		if( toolState == "curDeforming" && deformMode == "smooth" && brushHitOnObject &&  (e.type == EventType.MouseDrag || e.type == EventType.mouseDown ) && e.button == 0 && !e.shift && !e.alt && !e.control) {
			smoothMesh ();
			deformJobListStepBack = 0;
		}

		if( toolState == "curRefining" && brushHitOnObject &&  (e.type == EventType.MouseDrag || e.type == EventType.mouseDown ) && e.button == 0 && !e.shift && !e.alt && !e.control) {
			refineTriangle ();
		}

	}



	void drawAffectedVertices() {

		if (!showVertexIndicators && toolState != "curDeforming")
			return;


		bool hitTransform = false;
		for (int i = 0; i < currentGameObjects.Length; i++) {
			if (brushHit.transform == currentGameObjects[i].transform)
				hitTransform = true;
		}


		if (!brushHitOnObject && !hitTransform)
			return;


		affectedVerticesToSmooth = new bool[ currentGameObjects.Length ][];
		//getCurrentVerticesFromStream ();
		for (int g = 0; g < currentGameObjects.Length; g++) {

			GameObject currentGameObject = currentGameObjects [g];
			affectedVerticesToSmooth[g] = new bool[ currentVertices[g].Length ];

			for (int i = 0; i < currentVertices[g].Length; i++) {


				Vector3 vertPos = currentGameObject.transform.TransformPoint (currentVertices [g][i]);
				float sqrMag = Vector3.Distance (vertPos, brushHit.point);

				float usedBrushSize = 0;
				if (toolState == "curPainting" || toolState == "curWetting") {
					usedBrushSize = brushSize;
				} else if (toolState == "curDeforming") {
					usedBrushSize = deformBrushSize;
				}

				if (sqrMag > usedBrushSize /*|| Mathf.Abs( Vector3.Angle( hit.normal, normals[i]) ) > 80*/) {
					affectedVerticesToSmooth [g][i] = false;
					continue;
				}

				affectedVerticesToSmooth [g][i] = true;



				float usedBrushFalloff = 0;
				if (toolState == "curPainting" || toolState == "curWetting") {
					usedBrushFalloff = brushFalloff;
				} else if (toolState == "curDeforming") {
					usedBrushFalloff = 0;
				}



				float falloff;
				if (sqrMag > usedBrushFalloff) {
					falloff = VPP_Utils.linearFalloff (sqrMag - usedBrushFalloff, usedBrushSize);
				} else {
					falloff = 1f;
				}

				if (showVertexIndicators) {

					Handles.color = new Color (falloff, falloff, falloff);
					Handles.SphereCap (0, vertPos, Quaternion.identity, vertexIndicatorSize * falloff);

				}



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
		Color[][] tmp = new Color[currentGameObjects.Length][];
		for (int g = 0; g < currentGameObjects.Length; g++) {
			tmp [g] = (Color[])currentColors [g].Clone ();
		}

		drawJobList.Add (tmp);


		if( stepBackReset )
			drawJobListStepBack = 0;
	}

	void undoDrawJob() {
		if (drawJobList.Count <= drawJobListStepBack + 1)
			return;

		drawJobListStepBack++;
		currentColors = drawJobList [drawJobList.Count - drawJobListStepBack - 1];

		for (int g = 0; g < currentGameObjects.Length; g++) {

			currentGameObjects[g].GetComponent<VertexColorStream> ().setColors (currentColors[g]);
		}


		//currentGameObject.GetComponent<VertexColorStream> ()._vertexColors = currentColors;
		//currentGameObject.GetComponent<VertexColorStream> ().Upload ();


	}

	void redoDrawJob() {
		if (drawJobListStepBack < 1)
			return;

		drawJobListStepBack--;
		currentColors = drawJobList [drawJobList.Count - drawJobListStepBack - 1];



		for (int g = 0; g < currentGameObjects.Length; g++) {

			currentGameObjects[g].GetComponent<VertexColorStream> ().setColors (currentColors[g]);
		}

		//currentGameObject.GetComponent<VertexColorStream> ()._vertexColors = currentColors;
		//currentGameObject.GetComponent<VertexColorStream> ().Upload ();


	}

	void saveColorsToStream() {

		EditorGUI.BeginChangeCheck();

		if (drawIndex != 3) {
			for (int g = 0; g < currentGameObjects.Length; g++) {
				currentGameObjects [g].GetComponent<VertexColorStream> ().setColors (currentColors [g]);
			}
		} else { 
			for (int g = 0; g < currentGameObjects.Length; g++) {
				currentGameObjects [g].GetComponent<VertexColorStream> ().setUV4s (currentUV4s [g]);
			}
		}

		for (int g = 0; g < currentGameObjects.Length; g++) {
			EditorUtility.SetDirty (currentGameObjects [g].GetComponent<VertexColorStream> ());
			//EditorSceneManager.MarkSceneDirty (currentGameObject.GetComponent<VertexColorStream> ().gameObject.scene);
			Undo.RegisterCompleteObjectUndo (currentGameObjects [g], "Painted colors");
		}




	}

	void cancelDrawing() {

		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];

			currentGameObject.GetComponent<VertexColorStream> ().setColors (cancelColors[g]);
			currentGameObject.GetComponent<VertexColorStream> ().setUV4s (cancelUV4s[g]);
		}
		//currentGameObject.GetComponent<VertexColorStream> ()._vertexColors = cancelColors;
		//currentGameObject.GetComponent<VertexColorStream> ().Upload ();
	}


	void getCurrentColorsFromStream () {

		currentColors = new Color[currentGameObjects.Length][];
		cancelColors = new Color[currentGameObjects.Length][];
		currentVertices = new Vector3[currentGameObjects.Length][];
		currentUV4s = new Vector2[currentGameObjects.Length][];
		cancelUV4s = new Vector2[currentGameObjects.Length][];


		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];

			currentColors[g] = new Color[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getColors ().CopyTo (currentColors[g], 0);

			cancelColors[g] = new Color[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getColors ().CopyTo (cancelColors[g], 0);

			currentVertices[g] = new Vector3[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getVertices ().CopyTo (currentVertices[g], 0);

			currentUV4s[g] = new Vector2[currentGameObject.GetComponent<VertexColorStream> ().getUV4s ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getUV4s ().CopyTo (currentUV4s[g], 0);

			cancelUV4s[g] = new Vector2[currentGameObject.GetComponent<VertexColorStream> ().getUV4s ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getUV4s ().CopyTo (cancelUV4s[g], 0);

		}





	}


	void drawBrush() {

		HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));

		Ray worldRay = HandleUtility.GUIPointToWorldRay (mousePos);
		if (Physics.Raycast (worldRay, out brushHit, Mathf.Infinity, 1 << 31)) {

			brushHitOnObject = true;

		} else {
			brushHitOnObject = false;

		}

		bool hitTransform = false;
		for (int i = 0; i < currentGameObjects.Length; i++) {
			if (brushHit.transform == currentGameObjects[i].transform)
				hitTransform = true;
		}


		if (!hitTransform)
			return;

		Handles.color = new Color (drawColor.r, drawColor.g, drawColor.b, Mathf.Pow(brushStrength,2f));
		Handles.DrawSolidDisc (brushHit.point, brushHit.normal, brushSize);

		Handles.color = Color.red;
		Handles.DrawWireDisc (brushHit.point, brushHit.normal, brushSize);
		Handles.DrawWireDisc (brushHit.point, brushHit.normal, brushFalloff);

	}

	void drawVertexColor() {


		for (int g = 0; g < currentGameObjects.Length; g++) {

			GameObject currentGameObject = currentGameObjects [g];

			for( int i = 0 ; i < currentVertices[g].Length ; i++ ) {
				Vector3 vertPos = currentGameObject.transform.TransformPoint(currentVertices[g][i]);
				float sqrMag = Vector3.Distance(vertPos, brushHit.point);

				if( sqrMag > brushSize /*|| Mathf.Abs( Vector3.Angle( hit.normal, normals[i]) ) > 80*/ ) {
					continue;
				}

				//Debug.Log ("draw");

				float falloff = VPP_Utils.linearFalloff(sqrMag, brushSize);


				if (drawIndex == 0) {
					currentColors [g][i] = VPP_Utils.VertexColorLerp (currentColors [g][i], drawColor, brushStrength * falloff);
				} else if (drawIndex == 1) {
					currentColors [g][i].r = VPP_Utils.VertexColorLerp (currentColors [g][i], drawColor, brushStrength * falloff).r;
					currentColors [g][i].g = VPP_Utils.VertexColorLerp (currentColors [g][i], drawColor, brushStrength * falloff).g;
					currentColors [g][i].b = VPP_Utils.VertexColorLerp (currentColors [g][i], drawColor, brushStrength * falloff).b;
				} else if (drawIndex == 2) {
					currentColors [g][i].a = VPP_Utils.VertexColorLerp (currentColors [g][i], drawColor, brushStrength * falloff).a;
				} else if (drawIndex == 3) {

					currentUV4s [g][i] = new Vector2 (VPP_Utils.VertexColorLerp (new Color(currentUV4s[g][i].x,currentUV4s[g][i].x,currentUV4s[g][i].x,currentUV4s[g][i].y), new Color(wetnessStrength,wetnessStrength,wetnessStrength,wetnessHeight), brushStrength * falloff).r, VPP_Utils.VertexColorLerp (new Color(currentUV4s[g][i].x,currentUV4s[g][i].x,currentUV4s[g][i].x,currentUV4s[g][i].y), new Color(wetnessStrength,wetnessStrength,wetnessStrength,wetnessHeight), brushStrength * falloff).a);

				}

			}

			if (drawIndex == 3) {
				currentGameObject.GetComponent<VertexColorStream> ().setUV4s (currentUV4s[g]);
			}else{
				currentGameObject.GetComponent<VertexColorStream> ().setColors (currentColors[g]);
			}
		}



		//currentGameObject.GetComponent<VertexColorStream> ()._vertexColors = currentColors;
		//currentGameObject.GetComponent<VertexColorStream> ().Upload ();

	}

	void floodFillVertexColor() {

		//Debug.Log ("FloodFill");

		for (int g = 0; g < currentGameObjects.Length; g++) {

			GameObject currentGameObject = currentGameObjects [g];

			for (int i = 0; i < currentColors[g].Length; i++) {

				//currentColors [i] = drawColor;
				if (drawIndex == 3) {

					currentUV4s [g][i] = new Vector2 (wetnessStrength, wetnessHeight);

				} else {
					currentColors [g][i] = drawColor;
				}

			}



			if (drawIndex == 3)
				currentGameObject.GetComponent<VertexColorStream> ().setUV4s (currentUV4s[g]);
			else
				currentGameObject.GetComponent<VertexColorStream> ().setColors (currentColors[g]);

		}
		//currentGameObject.GetComponent<VertexColorStream> ()._vertexColors = currentColors;
		//currentGameObject.GetComponent<VertexColorStream> ().Upload ();

		addJobToDrawJobList (true);

	}




	/*
 *		.----------------.  .----------------.  .----------------.  .----------------.   .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------. 
 *		| .--------------. || .--------------. || .--------------. || .--------------. | | .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. |
 *		| |  _________   | || |   _____      | || |     ____     | || | _____  _____ | | | | ____    ____ | || |      __      | || |   ______     | || |   ______     | || |  _________   | || |  _______     | |
 *		| | |_   ___  |  | || |  |_   _|     | || |   .'    `.   | || ||_   _||_   _|| | | ||_   \  /   _|| || |     /  \     | || |  |_   __ \   | || |  |_   __ \   | || | |_   ___  |  | || | |_   __ \    | |
 *		| |   | |_  \_|  | || |    | |       | || |  /  .--.  \  | || |  | | /\ | |  | | | |  |   \/   |  | || |    / /\ \    | || |    | |__) |  | || |    | |__) |  | || |   | |_  \_|  | || |   | |__) |   | |
 *		| |   |  _|      | || |    | |   _   | || |  | |    | |  | || |  | |/  \| |  | | | |  | |\  /| |  | || |   / ____ \   | || |    |  ___/   | || |    |  ___/   | || |   |  _|  _   | || |   |  __ /    | |
 *		| |  _| |_       | || |   _| |__/ |  | || |  \  `--'  /  | || |  |   /\   |  | | | | _| |_\/_| |_ | || | _/ /    \ \_ | || |   _| |_      | || |   _| |_      | || |  _| |___/ |  | || |  _| |  \ \_  | |
 *		| | |_____|      | || |  |________|  | || |   `.____.'   | || |  |__/  \__|  | | | ||_____||_____|| || ||____|  |____|| || |  |_____|     | || |  |_____|     | || | |_________|  | || | |____| |___| | |
 *		| |              | || |              | || |              | || |              | | | |              | || |              | || |              | || |              | || |              | || |              | |
 *		| '--------------' || '--------------' || '--------------' || '--------------' | | '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' |
 *		'----------------'  '----------------'  '----------------'  '----------------'   '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------' 
 */


	int hittedGameObject = -1;
	void drawFlowBrush() {

		HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));

		Ray worldRay = HandleUtility.GUIPointToWorldRay (mousePos);
		if (Physics.Raycast (worldRay, out brushHit, Mathf.Infinity, 1 << 31)) {

			brushHitOnObject = true;

		} else {
			brushHitOnObject = false;

		}

		bool hitTransform = false;
		hittedGameObject = -1;
		for (int i = 0; i < currentGameObjects.Length; i++) {
			if (brushHit.transform == currentGameObjects [i].transform) {
				hitTransform = true;
				hittedGameObject = i;
			}
		}

		if (!hitTransform)
			return;

		Handles.color = new Color (0.8f, 0.8f, 0.8f, flowBrushStrength*0.75f);
		Handles.DrawSolidDisc (brushHit.point, brushHit.normal, flowBrushSize);

		Handles.color = Color.red;
		Handles.DrawWireDisc (brushHit.point, brushHit.normal, flowBrushSize);

	}



	void drawFlowData() {

		getCurrentUVsFromStream ();
		getCurrentVerticesFromStream ();

		for (int g = 0; g < currentGameObjects.Length; g++) {

			GameObject currentGameObject = currentGameObjects [g];

			for (int i = 0; i < currentVertices[g].Length; i++) {



				Vector3 vertPosition = currentGameObject.transform.TransformPoint (currentVertices [g][i]);

				Vector3 binormal = Vector3.Cross (currentNormals [g][i], new Vector3 (currentTangents [g][i].x, currentTangents [g][i].y, currentTangents [g][i].z)) * currentTangents [g][i].w;
				Vector3 tangent = currentTangents [g][i];

				if (binormal.y > 0)
					binormal *= -1;

				if (tangent.y > 0)
					tangent *= -1;


				Handles.DrawLine (vertPosition, vertPosition + currentGameObject.transform.TransformDirection ((new Vector3 (tangent.x, tangent.y, tangent.z).normalized)) * 0.2f);
				Handles.color = Color.blue;
				Handles.DrawLine (vertPosition, vertPosition + currentGameObject.transform.TransformDirection (binormal.normalized) * 0.2f);


			}
		}

	}


	void getCurrentUVsFromStream () {



		currentVertices = new Vector3[currentGameObjects.Length][];
		currentUVs = new Vector2[currentGameObjects.Length][];
		currentUV4s = new Vector2[currentGameObjects.Length][];
		currentTangents = new Vector4[currentGameObjects.Length][];


		for (int g = 0; g < currentGameObjects.Length; g++) {

			if (!currentGameObjects [g].GetComponent<VertexColorStream> ())
				continue;

			GameObject currentGameObject = currentGameObjects [g];

			currentVertices[g] = new Vector3[currentGameObject.GetComponent<VertexColorStream> ().getVertices().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getVertices ().CopyTo (currentVertices[g], 0);

			currentUVs[g] = new Vector2[currentGameObject.GetComponent<VertexColorStream> ().getVertices().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getUVs ().CopyTo (currentUVs[g], 0);

			currentUV4s[g] = new Vector2[currentGameObject.GetComponent<VertexColorStream> ().getVertices().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getUV4s ().CopyTo (currentUV4s[g], 0);


			currentTangents[g] =  new Vector4[currentGameObject.GetComponent<VertexColorStream> ().getVertices().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getTangents ().CopyTo (currentTangents[g], 0);

		}







	}


	void clearFlowData() {

		getCurrentUVsFromStream ();

		for (int g = 0; g < currentGameObjects.Length; g++) {

			if (!currentGameObjects [g].GetComponent<VertexColorStream> ())
				continue;


			GameObject currentGameObject = currentGameObjects [g];


			for (int i = 0; i < currentVertices[g].Length; i++) {

				float red = 0.5f;
				float green = 0.5f;

				currentUV4s [g][i] = new Vector2 (red, green);

			}

			currentGameObject.GetComponent<VertexColorStream> ().setUV4s (currentUV4s[g]);


			EditorUtility.SetDirty (currentGameObject.GetComponent<VertexColorStream> ());
			Undo.RegisterCompleteObjectUndo (currentGameObject, "Clear Flow Data");

		}

	}

	void paintFlowDataV3() {

		getCurrentUVsFromStream ();

		if (isFlowPaintAffected == null || isFlowPaintAffected.Length == 0) {
			isFlowPaintAffected = new Vector2[currentGameObjects.Length][];
		}
		if (flowPaintStartTimes == null || flowPaintStartTimes.Length == 0) {
			flowPaintStartTimes = new float[currentGameObjects.Length][];
		}

		for (int g = 0; g < currentGameObjects.Length; g++) {



			if (isFlowPaintAffected[g] == null || isFlowPaintAffected[g].Length == 0) {

				isFlowPaintAffected[g] = new Vector2[currentVertices [g].Length];
				flowPaintStartTimes[g] = new float[currentVertices [g].Length];

				paintFlowTotalTime = 0;

				for (int i = 0; i < currentVertices [g].Length; i++) {
					isFlowPaintAffected [g][i] = new Vector2 (-99f, -99f);
					flowPaintStartTimes [g][i] = -1f;
				}

			}





		}

		paintFlowTotalTime += Time.fixedDeltaTime;

		//for (int g = 0; g < currentGameObjects.Length; g++) {
		GameObject currentGameObject = currentGameObjects [hittedGameObject];

		for (int i = 0; i < currentVertices[hittedGameObject].Length; i++) {

			Vector3 vertPos = currentGameObject.transform.TransformPoint (currentVertices [hittedGameObject][i]);
			float sqrMag = Vector3.Distance (vertPos, brushHit.point);

			//outside and not affected
			if (sqrMag > flowBrushSize && isFlowPaintAffected[hittedGameObject][i].x == -99f && isFlowPaintAffected[hittedGameObject][i].y == -99f) {
				continue;
			}

			//stayed inside (affected)
			if (sqrMag <= flowBrushSize && isFlowPaintAffected[hittedGameObject][i].x != -99f && isFlowPaintAffected[hittedGameObject][i].y != -99f) {

				continue;
			}


			//went from outside (unaffected) to inside (affected)
			if (sqrMag <= flowBrushSize && isFlowPaintAffected[hittedGameObject][i].x == -99f && isFlowPaintAffected[hittedGameObject][i].y == -99f) {

				isFlowPaintAffected [hittedGameObject] [i] = brushHit.textureCoord;
				flowPaintStartTimes [hittedGameObject] [i] = paintFlowTotalTime;

				continue;
			}

			//Went from inside (affected) to outside (unaffected)
			if (sqrMag > flowBrushSize && isFlowPaintAffected[hittedGameObject][i].x != -99f && isFlowPaintAffected[hittedGameObject][i].y != -99f) {

				float timeDiff = 2f*(paintFlowTotalTime - flowPaintStartTimes[hittedGameObject][i]);
				Vector2 flowDiff =  isFlowPaintAffected[hittedGameObject][i] - brushHit.textureCoord;


				if( flowDiff.x > 0.5f ) flowDiff.x = 0.5f;
				if( flowDiff.x < -0.5f ) flowDiff.x = -0.5f;
				if( flowDiff.y > 0.5f ) flowDiff.y = 0.5f;
				if( flowDiff.y < -0.5f ) flowDiff.y = -0.5f;

				flowDiff /= timeDiff;

				currentUV4s [hittedGameObject][i] =  ( (1f - flowBrushStrength) * currentUV4s[hittedGameObject][i] + flowBrushStrength * new Vector2 (0.5f + flowDiff.x, 0.5f + flowDiff.y));

				for (int j = 0; j < currentGameObjects.Length; j++) {
					if (j == hittedGameObject)
						continue;

					for (int k = 0; k < currentVertices [j].Length; k++) {
						Vector3 j_vertPos = currentGameObjects[j].transform.TransformPoint (currentVertices [j] [k]);
						float j_sqrMag = Vector3.Distance (j_vertPos, brushHit.point);

						if (j_sqrMag <= flowBrushSize) {
							Debug.Log ("test");
							currentUV4s [j] [k] = ((1f - flowBrushStrength) * currentUV4s [j] [k] + flowBrushStrength * new Vector2 (0.5f + flowDiff.x, 0.5f + flowDiff.y));

						}

					}

					currentGameObjects[j].GetComponent<VertexColorStream> ().setUV4s (currentUV4s[j]);

				}

				isFlowPaintAffected[hittedGameObject][i] = new Vector2(-99f,-99f);
				continue;
			}


		}

		currentGameObject.GetComponent<VertexColorStream> ().setUV4s (currentUV4s[hittedGameObject]);


		EditorUtility.SetDirty(currentGameObject.GetComponent<VertexColorStream> ());
		//EditorSceneManager.MarkSceneDirty(currentGameObject.GetComponent<VertexColorStream> ().gameObject.scene);
		Undo.RegisterCompleteObjectUndo (currentGameObject, "Painted Flow Data");

		//Debug.Log (uvDiff.x + " | " + uvDiff.y);

		//}

		oldTexcoordHit = brushHit.textureCoord;


	}





	void processFlowMap() {

		getCurrentUVsFromStream ();

		#if UNITY_5_5_OR_NEWER
		TextureImporterCompression flowMapFormat = GetTextureFormatSettings (flowMap);
		SelectedChangeTextureFormatSettings (flowMap, TextureImporterCompression.Compressed);
		#else
		TextureImporterFormat flowMapFormat = GetTextureFormatSettings (flowMap);
		SelectedChangeTextureFormatSettings (flowMap, TextureImporterFormat.RGBA32);
		#endif

		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];

			for (int i = 0; i < currentVertices[g].Length; i++) {

				Color tmpColor = flowMap.GetPixel ((int)(currentUVs [g] [i].x * flowMap.width), (int)(currentUVs [g] [i].y * flowMap.height));

				float red = Mathf.Round (tmpColor.r * 100f) / 100f;
				float green = Mathf.Round (tmpColor.g * 100f) / 100f;

				currentUV4s [g][i] = new Vector2 (red, green);

			}

			SelectedChangeTextureFormatSettings (flowMap, flowMapFormat);

			currentGameObject.GetComponent<VertexColorStream> ().setUV4s (currentUV4s[g]);
			//currentGameObject.GetComponent<VertexColorStream> ()._uv3 = currentUV3s;
			//currentGameObject.GetComponent<VertexColorStream> ().Upload ();

			EditorUtility.SetDirty (currentGameObject.GetComponent<VertexColorStream> ());
			//EditorSceneManager.MarkSceneDirty(currentGameObject.GetComponent<VertexColorStream> ().gameObject.scene);
			Undo.RegisterCompleteObjectUndo (currentGameObject, "Process Flow Map");

		}

	}


	void processMeshForFlowData() {

		getCurrentUVsFromStream ();
		getCurrentVerticesFromStream ();

		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];


			for (int i = 0; i < currentVertices[g].Length; i++) {

				Vector3 binormal = currentGameObject.transform.TransformDirection (Vector3.Cross (currentNormals [g][i], new Vector3 (currentTangents [g][i].x, currentTangents [g][i].y, currentTangents [g][i].z)).normalized * currentTangents [g][i].w);
				Vector3 tangent = currentGameObject.transform.TransformDirection (currentTangents [g][i].normalized);

				float xFlow = 0.5f + 0.5f * tangent.y;
				float yFlow = 0.5f + 0.5f * binormal.y;

				currentUV4s [g][i] = new Vector2 (xFlow, yFlow);

			}

			currentGameObject.GetComponent<VertexColorStream> ().setUV4s (currentUV4s[g]);

			EditorUtility.SetDirty (currentGameObject.GetComponent<VertexColorStream> ());
			//EditorSceneManager.MarkSceneDirty(currentGameObject.GetComponent<VertexColorStream> ().gameObject.scene);
			Undo.RegisterCompleteObjectUndo (currentGameObject, "Process Mesh");


		}
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


	void generatePackedAlbedoTexture() {

		EditorUtility.DisplayProgressBar("Packing Albedo Map", "Resizing Albedo Map", 0f);

		int textureSize = int.Parse(combinedSize [combinedSizeIndex]);


		Color[] albedoMapColors = { Color.red };
		if (albedoMap) {

			#if UNITY_5_5_OR_NEWER
			TextureImporterCompression albedoMapFormat = GetTextureFormatSettings (albedoMap);
			SelectedChangeTextureFormatSettings (albedoMap, TextureImporterCompression.Compressed);
			#else
			TextureImporterFormat albedoMapFormat = GetTextureFormatSettings (albedoMap);
			SelectedChangeTextureFormatSettings (albedoMap, TextureImporterFormat.RGBA32);
			#endif

			Texture2D tmpAlbedoMap = Instantiate (ScaleTexture (albedoMap, textureSize, textureSize));
			albedoMapColors = tmpAlbedoMap.GetPixels ();
			DestroyImmediate (tmpAlbedoMap);
			SelectedChangeTextureFormatSettings (albedoMap, albedoMapFormat);
		}

		EditorUtility.DisplayProgressBar("Packing Albedo Map", "Resizing Specular / Metallic Map", 1f/4f);

		Color[] specMetMapColors = { Color.red };;
		if( specMetMap ) {
			#if UNITY_5_5_OR_NEWER
			TextureImporterCompression occlusionMapFormat = GetTextureFormatSettings (specMetMap);
			SelectedChangeTextureFormatSettings (specMetMap, TextureImporterCompression.Compressed);
			#else
			TextureImporterFormat occlusionMapFormat = GetTextureFormatSettings (specMetMap);
			SelectedChangeTextureFormatSettings (specMetMap, TextureImporterFormat.RGBA32);
			#endif
			Texture2D tmpSpecMetMap = Instantiate( ScaleTexture (specMetMap, textureSize, textureSize) );
			specMetMapColors = tmpSpecMetMap.GetPixels ();
			DestroyImmediate (tmpSpecMetMap);
			SelectedChangeTextureFormatSettings (specMetMap, occlusionMapFormat);
		}


		EditorUtility.DisplayProgressBar("Packing Albedo Map", "Packing Albedo Map",  2f/4f);


		Color[] combined_color = new Color[textureSize*textureSize];

		for( int i = 0 ; i < textureSize*textureSize ; i++ ) {

			if (albedoMap) {
				combined_color [i].r = albedoMapColors [i].r;
				combined_color [i].g = albedoMapColors [i].g;
				combined_color [i].b = albedoMapColors [i].b;
			} else {
				combined_color [i].r = 1;
				combined_color [i].g = 1;
				combined_color [i].b = 1;
			}

			if (specMetMap) {
				combined_color [i].a = specMetMapColors [i].r;
			} else {
				combined_color [i].a = 1;
			}




		}

		Texture2D packedAlbedoTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
		packedAlbedoTexture.SetPixels (combined_color);
		packedAlbedoTexture.Apply ();

		EditorUtility.DisplayProgressBar("Packing Albedo Map", "Saving Albedo Map",  3f/4f);


		byte[] bytes = packedAlbedoTexture.EncodeToPNG();

		string savePath = "";
		if (outputName != "") {
			savePath = pathCombined + "/" + outputName + ".png";
		} else {
			savePath = pathCombined + "/combined_Albedo.png";
		}

		File.WriteAllBytes(savePath, bytes);
		AssetDatabase.ImportAsset (savePath);

		EditorUtility.DisplayProgressBar("Packing Albedo Map", "Done", 1f);

		EditorUtility.ClearProgressBar ();


	}

	int defaultWidth = 512;
	int defaultHeight = 512;
	int defaultFormat = 0;
	int defaultLoading = 0;

	void generatePackedAlbedoSubstanceTexture() {



		int textureSize = int.Parse( combinedSize[combinedSizeIndex] );


		string path = AssetDatabase.GetAssetPath(pM);
		SubstanceImporter sI = AssetImporter.GetAtPath(path) as SubstanceImporter;
		sI.GetPlatformTextureSettings (pM.name, "", out defaultWidth, out defaultHeight, out defaultFormat, out defaultLoading);
		//if (defaultFormat == 0) {
		EditorUtility.DisplayProgressBar("Packing Albedo Map", "Marking as Readable", 1f/5f);
		#if UNITY_5_3_OR_NEWER
		sI.SetPlatformTextureSettings (pM, "", textureSize, textureSize, 1, defaultLoading);
		#else
		sI.SetPlatformTextureSettings (pM.name, "", textureSize, textureSize, 1, defaultLoading);
		#endif

		sI.SaveAndReimport ();
		//}


		pM.isReadable = true;
		pM.RebuildTexturesImmediately ();

		Texture[] texList = pM.GetGeneratedTextures();
		for (int i = 0; i < texList.Length; i++) {
			ProceduralTexture tex = texList[i] as ProceduralTexture;
			if (tex.GetProceduralOutputType () == ProceduralOutputType.Diffuse) {
				albedoMapSubstance = tex;
			} else if (tex.GetProceduralOutputType () == ProceduralOutputType.Metallic) {
				specMetMapSubstance = tex;
			}

		}

		EditorUtility.DisplayProgressBar("Packing Albedo Map", "Resizing Albedo Map", 1f/5f);

		Color32[] albedoMapColors = { Color.red };
		if (albedoMapSubstance) {

			albedoMapColors = albedoMapSubstance.GetPixels32 (0,0,textureSize,textureSize);

		}



		EditorUtility.DisplayProgressBar("Packing Albedo Map", "Resizing Specular / Metallic Map", 2f/5f);

		Color32[] specMetMapColors = { Color.red };;
		if( specMetMapSubstance ) {



			specMetMapColors = specMetMapSubstance.GetPixels32 (0,0,textureSize,textureSize);

		}

		//if (defaultFormat == 0) {
		//	sI.SetPlatformTextureSettings (pM, "", defaultWidth, defaultHeight, defaultFormat, defaultLoading);
		//	sI.SaveAndReimport ();
		//}

		EditorUtility.DisplayProgressBar("Packing Albedo Map", "Packing Albedo Map",  3f/5f);



		Color32[] combined_color = new Color32[textureSize*textureSize];

		for( int i = 0 ; i < textureSize*textureSize ; i++ ) {

			if (albedoMapSubstance) {
				combined_color [i].r = albedoMapColors [i].r;
				combined_color [i].g = albedoMapColors [i].g;
				combined_color [i].b = albedoMapColors [i].b;
			} else {
				combined_color [i].r = 255;
				combined_color [i].g = 255;
				combined_color [i].b = 255;
			}

			if (specMetMapSubstance) {
				combined_color [i].a = specMetMapColors [i].r;
			} else {
				combined_color [i].a = 255;
			}




		}

		Texture2D packedAlbedoTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
		packedAlbedoTexture.SetPixels32 (combined_color);
		packedAlbedoTexture.Apply ();

		EditorUtility.DisplayProgressBar("Packing Albedo Map", "Saving Albedo Map",  4f/5f);


		byte[] bytes = packedAlbedoTexture.EncodeToPNG();

		string savePath = "";
		if (outputName != "") {
			savePath = pathCombined + "/" + outputName + "_Alb_Met.png";
		} else {
			savePath = pathCombined + "/Albedo_Metallic.png";
		}

		File.WriteAllBytes(savePath, bytes);
		AssetDatabase.ImportAsset (savePath);

		EditorUtility.DisplayProgressBar("Packing Albedo Map", "Done", 1f);

		EditorUtility.ClearProgressBar ();


	}

	void generatePackedCombinedTexture() {

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Resizing Height Map", 0f);

		int textureSize = int.Parse(combinedSize [combinedSizeIndex]);

		Color[] heightMapColors = { Color.red };
		if (heightMap) {
			#if UNITY_5_5_OR_NEWER
			TextureImporterCompression heightMapFormat = GetTextureFormatSettings (heightMap);
			SelectedChangeTextureFormatSettings (heightMap, TextureImporterCompression.Compressed);
			#else
			TextureImporterFormat heightMapFormat = GetTextureFormatSettings (heightMap);
			SelectedChangeTextureFormatSettings (heightMap, TextureImporterFormat.RGBA32);
			#endif
			Texture2D tmpHeightMap = Instantiate (ScaleTexture (heightMap, textureSize, textureSize));
			heightMapColors = tmpHeightMap.GetPixels ();
			DestroyImmediate (tmpHeightMap);
			SelectedChangeTextureFormatSettings (heightMap, heightMapFormat);
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

			if (heightMap) {
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

		Texture2D combinedTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
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


	void generatePackedCombinedSubstanceTexture() {

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Resizing Height Map", 0f);

		int textureSize = int.Parse( combinedSize[combinedSizeIndex] );



		string path = AssetDatabase.GetAssetPath(pM);
		SubstanceImporter sI = AssetImporter.GetAtPath(path) as SubstanceImporter;
		//sI.GetPlatformTextureSettings (pM.name, "", out defaultWidth, out defaultHeight, out defaultFormat, out defaultLoading);
		///if (defaultFormat == 0) {
		//	EditorUtility.DisplayProgressBar("Packing Combined Map", "Marking as Readable", 1f/5f);
		//	sI.SetPlatformTextureSettings (pM, "", textureSize, textureSize, 1, defaultLoading);
		//	sI.SaveAndReimport ();
		//}


		pM.isReadable = true;
		pM.RebuildTexturesImmediately ();

		heightMapSubstance = null;
		OcclusionMapSubstance = null;
		smoothnessMapSubstance = null;
		emissionMapSubstance = null;

		Texture[] texList = pM.GetGeneratedTextures();
		for (int i = 0; i < texList.Length; i++) {
			ProceduralTexture tex = texList[i] as ProceduralTexture;
			if (tex.GetProceduralOutputType () == ProceduralOutputType.Height) {
				heightMapSubstance = tex;
			} else if (tex.GetProceduralOutputType () == ProceduralOutputType.AmbientOcclusion) {
				OcclusionMapSubstance = tex;
			} else if (tex.GetProceduralOutputType () == ProceduralOutputType.Smoothness) {
				smoothnessMapSubstance = tex;
			} else if (tex.GetProceduralOutputType () == ProceduralOutputType.Emissive) {
				emissionMapSubstance = tex;
			}

		}

		Color32[] heightMapColors = { Color.red };
		if (heightMapSubstance) {
			heightMapColors = heightMapSubstance.GetPixels32 (0,0,textureSize,textureSize);
		}

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Resizing Occlusion Map", 1f/6f);

		Color32[] OcclusionMapColors = { Color.red };
		if( OcclusionMapSubstance ) {
			OcclusionMapColors = OcclusionMapSubstance.GetPixels32 (0,0,textureSize,textureSize);
		}

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Resizing Smoothness Map",  2f/6f);

		Color32[] smoothnessMapColors = { Color.red };
		if (smoothnessMapSubstance) {
			smoothnessMapColors = smoothnessMapSubstance.GetPixels32 (0,0,textureSize,textureSize);
		}

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Resizing emission Map",  3f/6f);

		Color32[] emissionMapColors = { Color.red };
		if (emissionMapSubstance) {
			emissionMapColors = emissionMapSubstance.GetPixels32 (0,0,textureSize,textureSize);
		}

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Packing Combined Map",  4f/6f);


		#if UNITY_5_3_OR_NEWER
		sI.SetPlatformTextureSettings (pM, "", defaultWidth, defaultHeight, defaultFormat, defaultLoading);
		#else
		sI.SetPlatformTextureSettings (pM.name, "", defaultWidth, defaultHeight, defaultFormat, defaultLoading);
		#endif
		sI.SaveAndReimport ();


		Color32[] combined_color = new Color32[textureSize*textureSize];

		for( int i = 0 ; i < textureSize*textureSize ; i++ ) {

			if (heightMapSubstance) {
				combined_color [i].r = heightMapColors [i].r;
			} else {
				combined_color [i].r = 255;
			}

			if (OcclusionMapSubstance) {
				combined_color [i].g = OcclusionMapColors [i].r;
			} else {
				combined_color [i].g = 255;
			}

			if (smoothnessMapSubstance) {
				combined_color [i].b = smoothnessMapColors [i].r;
			} else {
				combined_color [i].b = 255;
			}

			if (emissionMapSubstance) {
				combined_color [i].a = emissionMapColors [i].r;
			} else {
				combined_color [i].a = 255;
			}


		}

		Texture2D combinedTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
		combinedTexture.SetPixels32 (combined_color);
		combinedTexture.Apply ();

		EditorUtility.DisplayProgressBar("Packing Combined Map", "Saving Combined Map",  5f/6f);


		byte[] bytes = combinedTexture.EncodeToPNG();
		string savePath = "";
		if (outputName != "") {
			savePath = pathCombined + "/" + outputName + "_Combined.png";
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
 *		.----------------.  .-----------------. .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------. 
 *		| .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. |
 *		| |      __      | || | ____  _____  | || |     _____    | || | ____    ____ | || |      __      | || |  _________   | || |     ____     | || |  _______     | |
 *		| |     /  \     | || ||_   \|_   _| | || |    |_   _|   | || ||_   \  /   _|| || |     /  \     | || | |  _   _  |  | || |   .'    `.   | || | |_   __ \    | |
 *		| |    / /\ \    | || |  |   \ | |   | || |      | |     | || |  |   \/   |  | || |    / /\ \    | || | |_/ | | \_|  | || |  /  .--.  \  | || |   | |__) |   | |
 *		| |   / ____ \   | || |  | |\ \| |   | || |      | |     | || |  | |\  /| |  | || |   / ____ \   | || |     | |      | || |  | |    | |  | || |   |  __ /    | |
 *		| | _/ /    \ \_ | || | _| |_\   |_  | || |     _| |_    | || | _| |_\/_| |_ | || | _/ /    \ \_ | || |    _| |_     | || |  \  `--'  /  | || |  _| |  \ \_  | |
 *		| ||____|  |____|| || ||_____|\____| | || |    |_____|   | || ||_____||_____|| || ||____|  |____|| || |   |_____|    | || |   `.____.'   | || | |____| |___| | |
 *		| |              | || |              | || |              | || |              | || |              | || |              | || |              | || |              | |
 *		| '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' |
 *		'----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------' 
 */







	/*
	*		.----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------.  .----------------. 
	*		| .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. |
	*		| |  ________    | || |  _________   | || |  _________   | || |     ____     | || |  _______     | || | ____    ____ | || |  _________   | || |  _______     | |
	*		| | |_   ___ `.  | || | |_   ___  |  | || | |_   ___  |  | || |   .'    `.   | || | |_   __ \    | || ||_   \  /   _|| || | |_   ___  |  | || | |_   __ \    | |
	*		| |   | |   `. \ | || |   | |_  \_|  | || |   | |_  \_|  | || |  /  .--.  \  | || |   | |__) |   | || |  |   \/   |  | || |   | |_  \_|  | || |   | |__) |   | |
	*		| |   | |    | | | || |   |  _|  _   | || |   |  _|      | || |  | |    | |  | || |   |  __ /    | || |  | |\  /| |  | || |   |  _|  _   | || |   |  __ /    | |
	*		| |  _| |___.' / | || |  _| |___/ |  | || |  _| |_       | || |  \  `--'  /  | || |  _| |  \ \_  | || | _| |_\/_| |_ | || |  _| |___/ |  | || |  _| |  \ \_  | |
	*		| | |________.'  | || | |_________|  | || | |_____|      | || |   `.____.'   | || | |____| |___| | || ||_____||_____|| || | |_________|  | || | |____| |___| | |
	*		| |              | || |              | || |              | || |              | || |              | || |              | || |              | || |              | |
	*		| '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' |
	*		'----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------' 
	*/

	/*
	void getVerticesToWeld() {

		getCurrentVerticesFromStream ();

		weldVertices = new List<int[]> ();



		for (int g0 = 0; g0 < currentGameObjects.Length; g0++) {
			Vector3[] cVertices = currentVertices [g0];


			for (int c0 = 0; c0 < cVertices.Length; c0++) {

				List<int> closeVertices = new List<int> ();


				for (int g1 = g0 + 1; g1 < currentGameObjects.Length; g1++) {
					Vector3[] cVerticesToCompare = currentVertices [g1];

					for (int c1 = 0; c1 < cVerticesToCompare.Length; c1++) {


						if( Vector3.Distance( cVertices[c0], cVerticesToCompare[c1]) < 0.0001f ) {
							if( closeVertices.Count == 0 ) {
								closeVertices.Add(c0);
							}
							closeVertices.Add(c1);
							break;

						}

					}

				}
				if (closeVertices.Count > 0) {
					weldVertices.Add (closeVertices.ToArray ());
				}

			}



		}





		Debug.Log ("WeldVertices Length: " + weldVertices.Count);



	}

	*/

	void drawDeformBrush() {

		HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));

		Ray worldRay = HandleUtility.GUIPointToWorldRay (mousePos);
		if (Physics.Raycast (worldRay, out brushHit, Mathf.Infinity, 1 << 31)) {

			brushHitOnObject = true;

		} else {
			brushHitOnObject = false;

		}

		bool hitTransform = false;
		for (int i = 0; i < currentGameObjects.Length; i++) {
			if (brushHit.transform == currentGameObjects[i].transform)
				hitTransform = true;
		}

		if (!hitTransform)
			return;

		Handles.color = new Color (0.8f, 0.8f, 0.8f, deformBrushStrength*0.75f);
		Handles.DrawSolidDisc (brushHit.point, brushHit.normal, deformBrushSize);

		Handles.color = Color.red;
		Handles.DrawWireDisc (brushHit.point, brushHit.normal, deformBrushSize);

	}


	void addJobToDeformJobList(bool stepBackReset) {



		Vector3[][] tmp = new Vector3[currentGameObjects.Length][];
		for (int g = 0; g < currentGameObjects.Length; g++) {
			tmp [g] = (Vector3[])currentVertices [g].Clone ();
		}

		deformJobList.Add (tmp);





		if( stepBackReset )
			deformJobListStepBack = 0;
	}

	void undoDeformJob() {
		if (deformJobList.Count <= deformJobListStepBack + 1)
			return;

		deformJobListStepBack++;
		currentVertices = deformJobList [deformJobList.Count - deformJobListStepBack - 1];

		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];

			currentGameObject.GetComponent<VertexColorStream> ().setVertices (currentVertices[g]);

		}

	}

	void redoDeformJob() {
		if (deformJobListStepBack < 1)
			return;

		deformJobListStepBack--;
		currentVertices = deformJobList [deformJobList.Count - deformJobListStepBack - 1];

		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];

			currentGameObject.GetComponent<VertexColorStream> ().setVertices (currentVertices[g]);

		}

	}



	void getCurrentVerticesFromStream () {

		currentVertices = new Vector3[currentGameObjects.Length][];
		cancelVertices = new Vector3[currentGameObjects.Length][];
		currentNormals = new Vector3[currentGameObjects.Length][];
		currentUVs = new Vector2[currentGameObjects.Length][];
		currentTriangles = new int[currentGameObjects.Length][];
		cancelTangents = new Vector4[currentGameObjects.Length][];

		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];


			currentVertices[g] = new Vector3[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getVertices ().CopyTo (currentVertices[g], 0);

			cancelVertices[g] = new Vector3[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getVertices ().CopyTo (cancelVertices[g], 0);

			currentNormals[g] = new Vector3[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getNormals ().CopyTo (currentNormals[g], 0);

			currentUVs[g] = new Vector2[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getUVs ().CopyTo (currentUVs[g], 0);

			currentTriangles[g] = new int[currentGameObject.GetComponent<VertexColorStream> ().getTriangles ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getTriangles ().CopyTo (currentTriangles[g], 0);

			cancelTangents[g] = new Vector4[currentGameObject.GetComponent<VertexColorStream> ().getTangents ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getTangents ().CopyTo (cancelTangents[g], 0);

		}



	}


	void saveVerticesToStream() {

		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];

			currentGameObject.GetComponent<VertexColorStream> ().setVertices (currentVertices[g]);
			currentGameObject.GetComponent<VertexColorStream> ().setTangents (calculateMeshTangents (g));

			EditorUtility.SetDirty (currentGameObject.GetComponent<VertexColorStream> ());
			//EditorSceneManager.MarkSceneDirty (currentGameObject.GetComponent<VertexColorStream> ().gameObject.scene);
			Undo.RegisterCompleteObjectUndo (currentGameObject, "Mesh deform");
		}

	}

	void cancelDeforming() {
		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];
			currentGameObject.GetComponent<VertexColorStream> ().setVertices (cancelVertices[g]);
			currentGameObject.GetComponent<VertexColorStream> ().setTangents (cancelTangents[g]);
		}

	}

	void deformVertices(int direction) {

		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];

			for (int i = 0; i < currentVertices[g].Length; i++) {
				Vector3 vertPos = currentGameObject.transform.TransformPoint (currentVertices [g][i]);
				float sqrMag = Vector3.Distance (vertPos, brushHit.point);

				if (sqrMag > deformBrushSize /*|| Mathf.Abs( Vector3.Angle( hit.normal, normals[i]) ) > 80*/) {
					continue;
				}

				//Debug.Log ("Deform");

				Vector3 normalDirection = currentNormals [g][i];

				if (deformMode == "intrude") {
					// Extrude / Intrude
					normalDirection = currentNormals [g] [i];
				} else if (deformMode == "pinch") {
					// Pinching
					normalDirection = Vector3.Normalize (brushHit.point - vertPos);
				} else if (deformMode == "push") {
					// Push / Pull
					normalDirection = Vector3.Normalize (brushHit.point - Camera.current.transform.position);
				} else if (deformMode == "raise") {
					normalDirection = Vector3.up;

				}

				float falloff = VPP_Utils.linearFalloff (sqrMag, deformBrushSize);



				currentVertices [g][i] += direction * 0.1f * brushStrength * normalDirection * falloff;



			}




			currentNormals[g] = currentGameObject.GetComponent<VertexColorStream> ().setVertices (currentVertices[g]);
			currentGameObject.GetComponent<VertexColorStream> ().setTangents (calculateMeshTangents (g));

		}

	}

	void smoothMesh() {

		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];

			currentVertices[g] = SmoothFilter.hcFilter (currentVertices[g], currentVertices[g], currentTriangles[g], 0, (1f - Mathf.Pow (deformBrushStrength, 8)), affectedVerticesToSmooth[g]);
			currentNormals[g] = currentGameObject.GetComponent<VertexColorStream> ().setVertices (currentVertices[g]);
			currentGameObject.GetComponent<VertexColorStream> ().setTangents (calculateMeshTangents (g));
		}

	}

	/*
	 * http://answers.unity3d.com/questions/7789/calculating-tangents-vector4.html
	 */

	private Vector4[] calculateMeshTangents(int g)
	{

		//speed up math by copying the mesh arrays
		int[] triangles = currentTriangles[g];
		Vector3[] vertices = currentVertices[g];
		Vector2[] uv = currentUVs[g];
		Vector3[] normals = currentNormals[g];

		//variable definitions
		int triangleCount = triangles.Length;
		int vertexCount = vertices.Length;

		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];

		Vector4[] tangents = new Vector4[vertexCount];

		for (long a = 0; a < triangleCount; a += 3)
		{
			long i1 = triangles[a + 0];
			long i2 = triangles[a + 1];
			long i3 = triangles[a + 2];

			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];

			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];

			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;

			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;

			float div = s1 * t2 - s2 * t1;
			float r = div == 0.0f ? 0.0f : 1.0f / div;

			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;

			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}


		for (long a = 0; a < vertexCount; ++a)
		{
			Vector3 n = normals[a];
			Vector3 t = tan1[a];

			//Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
			//tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
			Vector3.OrthoNormalize(ref n, ref t);
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;

			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}

		return tangents;
	}





	/*
 * 
 *  .----------------.  .----------------.  .----------------.  .----------------.  .-----------------. .----------------.  .----------------. 
 *  | .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. || .--------------. |
 *  | |  _______     | || |  _________   | || |  _________   | || |     _____    | || | ____  _____  | || |  _________   | || |  _______     | |
 *  | | |_   __ \    | || | |_   ___  |  | || | |_   ___  |  | || |    |_   _|   | || ||_   \|_   _| | || | |_   ___  |  | || | |_   __ \    | |
 *  | |   | |__) |   | || |   | |_  \_|  | || |   | |_  \_|  | || |      | |     | || |  |   \ | |   | || |   | |_  \_|  | || |   | |__) |   | |
 *  | |   |  __ /    | || |   |  _|  _   | || |   |  _|      | || |      | |     | || |  | |\ \| |   | || |   |  _|  _   | || |   |  __ /    | |
 *  | |  _| |  \ \_  | || |  _| |___/ |  | || |  _| |_       | || |     _| |_    | || | _| |_\   |_  | || |  _| |___/ |  | || |  _| |  \ \_  | |
 *  | | |____| |___| | || | |_________|  | || | |_____|      | || |    |_____|   | || ||_____|\____| | || | |_________|  | || | |____| |___| | |
 *  | |              | || |              | || |              | || |              | || |              | || |              | || |              | |
 *  | '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' || '--------------' |
 *  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------'  '----------------' 
 * 
 */



	int refineOnGameObject = 0;

	void drawRefineBrush() {

		HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));

		Ray worldRay = HandleUtility.GUIPointToWorldRay (mousePos);
		if (Physics.Raycast (worldRay, out brushHit, Mathf.Infinity, 1 << 31)) {

			brushHitOnObject = true;

		} else {
			brushHitOnObject = false;

		}

		bool hitTransform = false;
		for (int i = 0; i < currentGameObjects.Length; i++) {
			if (brushHit.transform == currentGameObjects [i].transform) {
				refineOnGameObject = i;
				hitTransform = true;
			}
		}

		if (!hitTransform)
			return;



		Vector3 p1 = currentVertices [refineOnGameObject][  currentTriangles[refineOnGameObject][brushHit.triangleIndex*3]  ];
		Vector3 p2 = currentVertices [refineOnGameObject][  currentTriangles[refineOnGameObject][brushHit.triangleIndex*3+1]  ];
		Vector3 p3 = currentVertices [refineOnGameObject][  currentTriangles[refineOnGameObject][brushHit.triangleIndex*3+2]  ];
		Vector3 lhp = currentGameObjects [refineOnGameObject].transform.InverseTransformPoint (brushHit.point);

		DrawSolidFace (new Vector3[] { p1, p2, p3  }, lhp, Color.red);

		/*	Handles.color = new Color (0.8f, 0.8f, 0.8f, deformBrushStrength*0.75f);
		Handles.DrawSolidDisc (brushHit.point, brushHit.normal, deformBrushSize);

	

		Handles.color = Color.red;
		Handles.DrawWireDisc (brushHit.point, brushHit.normal, deformBrushSize);*/

	}

	Material lineMaterial;
	void CreateLineMaterial()
	{
		lineMaterial = Resources.Load("faceMat", typeof(Material)) as Material;

		//Debug.Log (lineMaterial);
	}

	void DrawSolidFace(Vector3[] verts, Vector3 lhp, Color faceColor)
	{
		if (Event.current.type == EventType.Repaint)
		{

			CreateLineMaterial();
			lineMaterial.SetPass( 0 );

			//GL.PushMatrix();
			// Set transformation matrix for drawing to
			// match our transform
			GL.LoadProjectionMatrix(Camera.current.projectionMatrix);
			GL.MultMatrix(currentGameObjects[0].transform.localToWorldMatrix);
			// Draw lines



			GL.Begin(GL.TRIANGLES);

			GL.Color( highlightColor );

			//Debug.Log (verts [0]);
			GL.Vertex3(verts[0].x,verts[0].y,verts[0].z);
			GL.Vertex3(verts[1].x,verts[1].y,verts[1].z);
			GL.Vertex3(verts[2].x,verts[2].y,verts[2].z);

			GL.End();

			GL.Begin(GL.LINES);

			GL.Color( edgeColor );

			//Debug.Log (verts [0]);
			GL.Vertex3(verts[0].x,verts[0].y,verts[0].z);
			GL.Vertex3(lhp.x,lhp.y,lhp.z);
			GL.Vertex3(verts[1].x,verts[1].y,verts[1].z);
			GL.Vertex3(lhp.x,lhp.y,lhp.z);
			GL.Vertex3(verts[2].x,verts[2].y,verts[2].z);
			GL.Vertex3(lhp.x,lhp.y,lhp.z);

			GL.End();
			//GL.PopMatrix();
		}
	}





	void getCurrentVerticesFromStreamForRefine () {


		currentVertices = new Vector3[currentGameObjects.Length][];
		currentColors = new Color[currentGameObjects.Length][];
		currentNormals = new Vector3[currentGameObjects.Length][];
		currentUVs = new Vector2[currentGameObjects.Length][];
		currentUV2s = new Vector2[currentGameObjects.Length][];
		currentUV3s = new Vector2[currentGameObjects.Length][];
		currentUV4s = new Vector2[currentGameObjects.Length][];
		currentTriangles = new int[currentGameObjects.Length][];

		cancelVertices = new Vector3[currentGameObjects.Length][];
		cancelColors = new Color[currentGameObjects.Length][];
		cancelNormals = new Vector3[currentGameObjects.Length][];
		cancelUVs = new Vector2[currentGameObjects.Length][];
		cancelUV2s = new Vector2[currentGameObjects.Length][];
		cancelUV3s = new Vector2[currentGameObjects.Length][];
		cancelUV4s = new Vector2[currentGameObjects.Length][];
		cancelTriangles = new int[currentGameObjects.Length][];
		cancelTangents = new Vector4[currentGameObjects.Length][];


		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];


			currentVertices[g] = new Vector3[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getVertices ().CopyTo (currentVertices[g], 0);

			currentColors[g] = new Color[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getColors ().CopyTo (currentColors[g], 0);

			currentNormals[g] = new Vector3[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getNormals ().CopyTo (currentNormals[g], 0);

			currentUVs[g] = new Vector2[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getUVs ().CopyTo (currentUVs[g], 0);

			currentUV2s[g] = new Vector2[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getUV2s ().CopyTo (currentUV2s[g], 0);

			currentUV3s[g] = new Vector2[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getUV3s ().CopyTo (currentUV3s[g], 0);

			currentUV4s[g] = new Vector2[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getUV4s ().CopyTo (currentUV4s[g], 0);

			currentTriangles[g] = new int[currentGameObject.GetComponent<VertexColorStream> ().getTriangles ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getTriangles ().CopyTo (currentTriangles[g], 0);


			cancelVertices[g] = new Vector3[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getVertices ().CopyTo (cancelVertices[g], 0);

			cancelColors[g] = new Color[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getColors ().CopyTo (cancelColors[g], 0);

			cancelNormals[g] = new Vector3[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getNormals ().CopyTo (cancelNormals[g], 0);

			cancelUVs[g] = new Vector2[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getUVs ().CopyTo (cancelUVs[g], 0);

			cancelUV2s[g] = new Vector2[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getUV2s ().CopyTo (cancelUV2s[g], 0);

			cancelUV3s[g] = new Vector2[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getUV3s ().CopyTo (cancelUV3s[g], 0);

			cancelUV4s[g] = new Vector2[currentGameObject.GetComponent<VertexColorStream> ().getVertices ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getUV4s ().CopyTo (cancelUV4s[g], 0);

			cancelTriangles[g] = new int[currentGameObject.GetComponent<VertexColorStream> ().getTriangles ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getTriangles ().CopyTo (cancelTriangles[g], 0);

			cancelTangents[g] = new Vector4[currentGameObject.GetComponent<VertexColorStream> ().getTangents ().Length];
			currentGameObject.GetComponent<VertexColorStream> ().getTangents ().CopyTo (cancelTangents[g], 0);





		}



	}


	void saveVerticesToStreamFromRefine() {

		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];

			currentGameObject.GetComponent<VertexColorStream> ().setVertices (currentVertices[g]);
			currentGameObject.GetComponent<VertexColorStream> ().setTangents (calculateMeshTangents (g));

			EditorUtility.SetDirty (currentGameObject.GetComponent<VertexColorStream> ());
			//EditorSceneManager.MarkSceneDirty (currentGameObject.GetComponent<VertexColorStream> ().gameObject.scene);
			Undo.RegisterCompleteObjectUndo (currentGameObject, "Mesh deform");
		}

	}

	void cancelRefining() {
		for (int g = 0; g < currentGameObjects.Length; g++) {
			GameObject currentGameObject = currentGameObjects [g];
			currentGameObject.GetComponent<VertexColorStream> ().setTriangles (cancelTriangles[g]);
			currentGameObject.GetComponent<VertexColorStream> ().setVertices (cancelVertices[g]);
			currentGameObject.GetComponent<VertexColorStream> ().setColors (cancelColors[g]);
			currentGameObject.GetComponent<VertexColorStream> ().setUVs (cancelUVs[g]);
			currentGameObject.GetComponent<VertexColorStream> ().setUV2s (cancelUV2s[g]);
			currentGameObject.GetComponent<VertexColorStream> ().setUV3s (cancelUV3s[g]);
			currentGameObject.GetComponent<VertexColorStream> ().setUV4s (cancelUV4s[g]);
			currentGameObject.GetComponent<VertexColorStream> ().setNormals (cancelNormals [g]);
			currentGameObject.GetComponent<VertexColorStream> ().setTangents (cancelTangents [g]);
		}

	}



	void refineTriangle() {

		//getCurrentVerticesFromStreamForRefine ();

		int triIndex = brushHit.triangleIndex;
		int v1 = currentTriangles [0] [triIndex*3];
		int v2 = currentTriangles [0] [triIndex*3+1];
		int v3 = currentTriangles [0] [triIndex*3+2];

		Color c1 = currentColors [0] [v1];
		Color c2 = currentColors [0] [v2];
		Color c3 = currentColors [0] [v3];

		Vector2 uv31 = currentUV3s [0] [v1];
		Vector2 uv32 = currentUV3s [0] [v2];
		Vector2 uv33 = currentUV3s [0] [v3];

		Vector2 uv41 = currentUV4s [0] [v1];
		Vector2 uv42 = currentUV4s [0] [v2];
		Vector2 uv43 = currentUV4s [0] [v3];


		Vector3[] newVertices = new Vector3[currentVertices[0].Length + 1];
		int vNew = newVertices.Length - 1;
		currentVertices[0].CopyTo (newVertices, 0);
		newVertices [vNew] = currentGameObjects [0].transform.InverseTransformPoint (brushHit.point);

		Vector2[] newUVs = new Vector2[currentUVs[0].Length + 1];
		currentUVs[0].CopyTo (newUVs, 0);
		newUVs [vNew] = brushHit.textureCoord;

		Vector2[] newUV2s = new Vector2[currentUV2s[0].Length + 1];
		currentUV2s[0].CopyTo (newUV2s, 0);
		newUV2s [vNew] = brushHit.textureCoord2;

		Vector2[] newUV3s = new Vector2[currentUV3s[0].Length + 1];
		currentUV3s[0].CopyTo (newUV3s, 0);
		newUV3s [vNew] = new Vector2 ((uv31.x + uv32.x + uv33.x) / 3f, (uv31.y + uv32.y + uv33.y) / 3f);

		Vector2[] newUV4s = new Vector2[currentUV4s[0].Length + 1];
		currentUV4s[0].CopyTo (newUV4s, 0);
		newUV4s [vNew] = new Vector2 ((uv41.x + uv42.x + uv43.x) / 3f, (uv41.y + uv42.y + uv43.y) / 3f);

		Vector3[] newNormals = new Vector3[currentNormals[0].Length + 1];
		currentNormals[0].CopyTo (newNormals, 0);
		newNormals [vNew] = currentGameObjects [0].transform.InverseTransformVector (brushHit.normal);

		Color[] newColors = new Color[currentColors[0].Length + 1];
		currentColors[0].CopyTo (newColors, 0);
		newColors [vNew] = new Color ((c1.r + c2.r + c3.r) / 3f, 
			(c1.g + c2.g + c3.g) / 3f,
			(c1.b + c2.b + c3.b) / 3f,
			(c1.a + c2.a + c3.a) / 3f);




		List<int> newTriangles = new List<int> ();
		//int[] newTriangles = new int[currentTriangles[0].Length - 3];
		for (int i = 0; i < currentTriangles[0].Length / 3; i++) {

			if (i != triIndex) {

				newTriangles.Add(currentTriangles [0] [i * 3]);
				newTriangles.Add(currentTriangles [0] [i * 3 + 1 ]);
				newTriangles.Add(currentTriangles [0] [i * 3 + 2 ]);


			}

		}

		newTriangles.Add(v1);
		newTriangles.Add(vNew);
		newTriangles.Add(v3);

		newTriangles.Add(v3);
		newTriangles.Add(vNew);
		newTriangles.Add(v2);

		newTriangles.Add(v2);
		newTriangles.Add(vNew);
		newTriangles.Add(v1);


		currentVertices [0] = new Vector3[newVertices.Length];
		newVertices.CopyTo (currentVertices [0], 0);

		currentTriangles[0] = new int[newTriangles.Count];
		newTriangles.ToArray ().CopyTo (currentTriangles[0], 0);

		currentUVs [0] = new Vector2[newUVs.Length];
		newUVs.CopyTo (currentUVs [0], 0);

		currentUV2s [0] = new Vector2[newUV2s.Length];
		newUV2s.CopyTo (currentUV2s [0], 0);

		currentUV3s [0] = new Vector2[newUV3s.Length];
		newUV3s.CopyTo (currentUV3s [0], 0);

		currentUV4s [0] = new Vector2[newUV4s.Length];
		newUV4s.CopyTo (currentUV4s [0], 0);

		currentNormals [0] = new Vector3[newNormals.Length];
		newNormals.CopyTo (currentNormals [0], 0);

		currentColors [0] = new Color[newColors.Length];
		newColors.CopyTo (currentColors [0], 0);



		currentGameObjects [0].GetComponent<VertexColorStream> ().setVertices (currentVertices[0]);
		currentGameObjects [0].GetComponent<VertexColorStream> ().setColors (currentColors[0]);
		currentGameObjects [0].GetComponent<VertexColorStream> ().setUVs (currentUVs[0]);
		currentGameObjects [0].GetComponent<VertexColorStream> ().setUV2s (currentUV2s[0]);
		currentGameObjects [0].GetComponent<VertexColorStream> ().setUV3s (currentUV3s[0]);
		currentGameObjects [0].GetComponent<VertexColorStream> ().setUV4s (currentUV4s[0]);
		currentGameObjects [0].GetComponent<VertexColorStream> ().setNormals (currentNormals [0]);
		currentGameObjects [0].GetComponent<VertexColorStream> ().setTriangles (currentTriangles[0]);
		currentGameObjects [0].GetComponent<VertexColorStream> ().setTangents (calculateMeshTangents (0));
		currentGameObjects [0].GetComponent<MeshCollider> ().convex = true;
		currentGameObjects [0].GetComponent<MeshCollider> ().convex = false;
		//EditorUtility.SetDirty (currentGameObjects[0].GetComponent<VertexColorStream> ());
		//Undo.RegisterCompleteObjectUndo (currentGameObjects[0], "Mesh deform");

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