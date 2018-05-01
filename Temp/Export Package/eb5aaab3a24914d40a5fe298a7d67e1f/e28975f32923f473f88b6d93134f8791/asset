using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
//using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class VCA_window : EditorWindow {
	
	/* GUI Styles */
	private GUIStyle headerBoxStyle;
	private GUIStyle bodyH1Style;

	private Texture2D backgroundTex;
	private GUIStyle backgroundStyle;

	private Texture2D animationSliderTex;

	private GUIStyle keyframeStyle;
	private Texture2D keyframeTexSel;
	private Texture2D keyframeTexUnSel;

	/* END GUI Styles */

	/* Global variables */
	private GameObject currentGameObject;
	private string gameObjectState = "null";

	private List<float> keyframes;

	private int isDraggingKeyFrame = -1;
	private int selectedKeyFrame = -1;
	private bool changedKeyFrame = false;

	private float animationSlider = 0f;
	private float oldAnimationSlider = 0f;
	private bool isDraggingAnimationSlider = false;

	private float elapsedTime;
	private float totalElapsedTime;

	private Texture2D playTex;
	private bool isPlaying = false;
	private Texture2D toStartTex;
	private Texture2D toEndTex;

	private string[] loopMode = new string[3] {"Clamp", "Loop", "PingPong"};
	[SerializeField]
	private int loopModeIndex = 2;
	[SerializeField]
	private float playLength = 2f;


	void checkCurrentGameObject() {

		if( currentGameObject && !currentGameObject.GetComponent<VertexColorAnimator>() )
			gameObjectState = "noVCA";

		if (Selection.activeGameObject == currentGameObject) {
			return;
		}

		currentGameObject = Selection.activeGameObject;

		if ( !currentGameObject || !currentGameObject.GetComponent<MeshRenderer>()) {

			gameObjectState = "null";
			isPlaying = false;


			return;
		} 

		if (currentGameObject.GetComponent<VertexColorAnimator> () == null) {
			gameObjectState = "noVCA";
		} else {
			gameObjectState = "ready";

			keyframes = currentGameObject.GetComponent<VertexColorAnimator> ().animationKeyframes;
			currentGameObject.GetComponent<VertexColorAnimator> ().scrobble (animationSlider);
		}


	}


	void playAtRuntime() {

		totalElapsedTime += Time.fixedDeltaTime;

		if (loopModeIndex == 0) {
			//Clamp
			elapsedTime += Time.fixedDeltaTime / playLength;
			if (elapsedTime >= 1)
				elapsedTime = 1;
			
		} else if (loopModeIndex == 1) {

			//Loop
			elapsedTime += Time.fixedDeltaTime / playLength;
			if (elapsedTime >= 1)
				elapsedTime = 0;
			
		} else if (loopModeIndex == 2) {

			//PingPong
			if (Mathf.FloorToInt (totalElapsedTime / playLength) % 2 == 0) {
				elapsedTime += Time.fixedDeltaTime / playLength;
			} else {
				elapsedTime -= Time.fixedDeltaTime / playLength;
			}
		}
		if (elapsedTime >= 1)
			elapsedTime = 1;
		else if (elapsedTime <= 0)
			elapsedTime = 0;
		
		animationSlider = elapsedTime;


	}


	void Update() {

		if( isPlaying )
			playAtRuntime ();
		
		checkCurrentGameObject ();

		Repaint ();

	}

	void guiForNoVCA () {

		if (GUILayout.Button ("Create Vertex Color Animator", GUILayout.ExpandWidth (true))) {

			currentGameObject.AddComponent<VertexColorAnimator> ();
			currentGameObject.GetComponent<VertexColorAnimator> ().initLists ();
			keyframes = currentGameObject.GetComponent<VertexColorAnimator> ().animationKeyframes;
			gameObjectState = "ready";

		}

	}


	void guiForReady() {

		if (gameObjectState == "ready" && selectedKeyFrame == -1 && GUILayout.Button ("Add State as keyframe", GUILayout.ExpandWidth (true))) {

			currentGameObject.GetComponent<VertexColorAnimator> ().addMesh (currentGameObject.GetComponent<VertexColorStream> ().paintedMesh, animationSlider);
			sortKeyFrames ();
			Repaint ();
			//Debug.Log("test");
		}

		if (gameObjectState == "ready" && selectedKeyFrame != -1) {

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Replace keyframe", GUILayout.ExpandWidth (true))) {

				currentGameObject.GetComponent<VertexColorAnimator> ().replaceKeyframe (selectedKeyFrame, currentGameObject.GetComponent<VertexColorStream> ().paintedMesh);

			}
			if (GUILayout.Button ("Delete keyframe", GUILayout.ExpandWidth (true))) {

				currentGameObject.GetComponent<VertexColorAnimator> ().deleteKeyframe (selectedKeyFrame);
				currentGameObject.GetComponent<VertexColorAnimator> ().scrobble (animationSlider);
			}

			GUILayout.EndHorizontal ();


		}


		GUILayout.BeginArea (new Rect (10, 45, position.width - 20, 40), "", backgroundStyle);
		GUILayout.EndArea ();

		GUI.Box (new Rect (20 + animationSlider * (position.width-16-40), 25f, 16f, 80f), animationSliderTex, keyframeStyle);


		if (keyframes != null) {
			for (int i = 0; i < keyframes.Count; i++) {

				if (i == selectedKeyFrame) {
					GUI.Box (new Rect (20 + keyframes [i] * (position.width - 16 - 40), 50f, 16f, 32f), keyframeTexSel, keyframeStyle);
				} else {
					GUI.Box (new Rect (20 + keyframes [i] * (position.width - 16 - 40), 50f, 16f, 32f), keyframeTexUnSel, keyframeStyle);
				}

			}
		}

		GUILayout.Space (85);

		GUILayout.BeginHorizontal ();
		GUILayout.Box ("", headerBoxStyle, GUILayout.Height(10), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal ();

		GUILayout.Space (5);


		GUILayout.BeginHorizontal ();
		{
			GUILayout.Space (10);

			isPlaying = GUILayout.Toggle (isPlaying, playTex, GUI.skin.button, GUILayout.Width (29));
			
			if (GUILayout.Button (toStartTex, GUILayout.Width (29))) {
				if( !isPlaying )
					animationSlider = 0f;
			}
			if (GUILayout.Button (toEndTex, GUILayout.Width (29))) {
				if( !isPlaying )
					animationSlider = 1f;
			}

			loopModeIndex = EditorGUI.Popup(
				new Rect(110,127, 70 , 40),
				"",
				loopModeIndex, 
				loopMode);

			currentGameObject.GetComponent<VertexColorAnimator> ().mode = loopModeIndex;

			GUILayout.Space (75);

			GUILayout.BeginHorizontal ();
			{
				EditorGUILayout.LabelField ("Play Length [s]:", GUILayout.Width(85));
				playLength = EditorGUILayout.FloatField ("", playLength, GUILayout.Width(25));
				currentGameObject.GetComponent<VertexColorAnimator> ().timeScale = playLength;
			}
			GUILayout.EndHorizontal ();

		}
		GUILayout.EndHorizontal ();

	}


	void OnGUI() {

		if (Application.isPlaying )
			return;


		if (gameObjectState == "null")
			return;

		if (gameObjectState == "noVCA" ) {
			guiForNoVCA ();
			return;

		}


		guiForReady ();


		ProcessInputs ();

		if( animationSlider != oldAnimationSlider )
			currentGameObject.GetComponent<VertexColorAnimator> ().scrobble (animationSlider);
		
		oldAnimationSlider = animationSlider;

		GUILayout.FlexibleSpace ();

		guiForVersion ();
		//Debug.Log ("Selected keyframe #" + selectedKeyFrame);

	}

	void guiForVersion() {
		
		GUILayout.Box ("", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		GUILayout.Box ("Version 0.8.1b1", bodyH1Style, GUILayout.Height(20), GUILayout.ExpandWidth(true));
		
	}


	void OnSceneGUI(SceneView sceneView) {
		


		sceneView.Repaint ();
	}
		
	public static void LaunchVCA_window() {

		var win = EditorWindow.GetWindow<VCA_window> (false, "VertexColor Animator", true);
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

		keyframeStyle = new GUIStyle();
		headerBoxStyle.margin = new RectOffset (0, 0, 0, 0);
		headerBoxStyle.padding = new RectOffset (0, 0, 0, 0);
		headerBoxStyle.border = new RectOffset (0, 0, 0, 0);

		keyframeTexSel = (Texture2D)Resources.Load ("Animator/VTP_keyframe_selected");
		keyframeTexUnSel = (Texture2D)Resources.Load ("Animator/VTP_keyframe_unselected");
		animationSliderTex = (Texture2D)Resources.Load ("Animator/VTP_slider_red");

		backgroundStyle = new GUIStyle ();
		backgroundStyle.normal.background = (Texture2D)Resources.Load ("Animator/background");
		backgroundStyle.border = new RectOffset (2, 2, 2, 2);
		backgroundStyle.margin = new RectOffset (0, 0, 0, 0);

		playTex = (Texture2D)Resources.Load ("Animator/play");
		toStartTex = (Texture2D)Resources.Load ("Animator/to_start");
		toEndTex = (Texture2D)Resources.Load ("Animator/to_end");


	}


	void snapToKeyFrame() {

		for (int i = 0; i < keyframes.Count; i++) {

			if (Mathf.Abs (keyframes [i] - animationSlider) < 8f / (position.width - 16 - 40)) {
				animationSlider = keyframes [i];
				selectedKeyFrame = i;
			}

		}

	}

	void ProcessInputs() {

		Event e = Event.current;

		if (e.type == EventType.MouseUp) {

			isDraggingKeyFrame = -1;
			isDraggingAnimationSlider = false;
			sortKeyFrames();

			if( !changedKeyFrame )
				snapToKeyFrame ();
			currentGameObject.GetComponent<VertexColorAnimator> ().scrobble (animationSlider);
			changedKeyFrame = false;

		}

		if (e.type == EventType.MouseDown) {

			for (int i = 0; i < keyframes.Count; i++) {
				
				Rect keyframeController = new Rect (20 + keyframes [i] * (position.width - 16 - 40), 50f, 16f, 32f);
				if( keyframeController.Contains( e.mousePosition ) ) {
					isDraggingKeyFrame = i;
				}

			}

			if (isDraggingKeyFrame == -1) {
				selectedKeyFrame = -1;
			}
			else {
				selectedKeyFrame = isDraggingKeyFrame;
				changedKeyFrame = true;

			}


			Rect sliderController = new Rect (20 + animationSlider * (position.width-8-40), 25f, 16f, 24f);
			if( sliderController.Contains( e.mousePosition ) ) {
				isDraggingAnimationSlider = true;
			}

		}

		if (e.type == EventType.MouseDrag && isDraggingKeyFrame != -1 && !isDraggingAnimationSlider) {
			selectedKeyFrame = -1;

			float relativePosition = (e.mousePosition.x - 20 - 8) / (position.width - 16 - 40);

			if (relativePosition < 0) {
				relativePosition = 0;
			} else if (relativePosition > 1) {
				relativePosition = 1;
			}

			keyframes [isDraggingKeyFrame] = relativePosition;
			currentGameObject.GetComponent<VertexColorAnimator> ().scrobble (animationSlider);
		}

		if (e.type == EventType.MouseDrag && isDraggingAnimationSlider) {
			float relativePosition = (e.mousePosition.x - 20 - 8) / (position.width - 16 - 40);

			if (relativePosition < 0) {
				relativePosition = 0;
			} else if (relativePosition > 1) {
				relativePosition = 1;
			}

			animationSlider = relativePosition;

			//Debug.Log (animationSlider);
		}

		if( gameObjectState == "ready" && keyframes != null && currentGameObject.GetComponent<VertexColorAnimator> ().animationKeyframes != null )
			currentGameObject.GetComponent<VertexColorAnimator> ().animationKeyframes = keyframes;


	}

	private void sortKeyFrames() {

		if (keyframes == null)
			return;

		for (int n = keyframes.Count; n > 1; n--){
			
			for (int i = 0; i < n-1; i++) {
				if (keyframes[i] > keyframes[i+1]){

					float tempFloat = keyframes [i];
					keyframes [i] = keyframes [i + 1];
					keyframes [i + 1] = tempFloat;

					MeshHolder tempMeshHolder = currentGameObject.GetComponent<VertexColorAnimator> ().animationMeshes [i];
					currentGameObject.GetComponent<VertexColorAnimator> ().animationMeshes [i] = currentGameObject.GetComponent<VertexColorAnimator> ().animationMeshes [i + 1];
					currentGameObject.GetComponent<VertexColorAnimator> ().animationMeshes [i + 1] = tempMeshHolder;

				} // ende if
			} // ende innere for-Schleife

		}
	}





}