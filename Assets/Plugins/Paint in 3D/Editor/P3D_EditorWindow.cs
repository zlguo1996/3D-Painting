using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public abstract class P3D_EditorWindow : EditorWindow
{
	private static Stack<Color> guiColors = new Stack<Color>();

	private static Stack<float> labelWidths = new Stack<float>();

	[SerializeField]
	private Vector2 mousePosition;

	[SerializeField]
	protected Vector2 scrollPosition;

	//[SerializeField]
	//private List<float> groupHeights = new List<float>();

	public void SetTitle(string newTitle)
	{
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		title = newTitle;
#else
		titleContent = new GUIContent(newTitle);
#endif
	}

	protected virtual void OnEnable()
	{
		SceneView.onSceneGUIDelegate += OnSceneGUI;
	}

	protected virtual void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
	}

	protected virtual void OnGUI()
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		{
			P3D_Helper.BaseRect    = P3D_Helper.Reserve(0.0f);
			P3D_Helper.BaseRectSet = true;

			EditorGUI.BeginChangeCheck();
			{
				OnInspector();
			}
			if (EditorGUI.EndChangeCheck() == true)
			{
				SceneView.RepaintAll();
			}

			P3D_Helper.BaseRectSet = false;
		}
		GUILayout.EndScrollView();
	}

	protected virtual void OnSceneGUI(SceneView sceneView)
	{
		var camera = sceneView.camera;

		mousePosition = Event.current.mousePosition;

		if (camera != null)
		{
			Handles.BeginGUI();
			{
				OnScene(sceneView, camera, mousePosition);
			}
			Handles.EndGUI();

			//sceneView.Repaint();
		}
	}

	protected virtual void OnSelectionChange()
	{
		Repaint();
	}

	protected virtual void OnInspector()
	{
	}

	protected virtual void OnScene(SceneView sceneView, Camera camera, Vector2 mousePosition)
	{
	}

	protected bool Button(string text)
	{
		var rect = P3D_Helper.Reserve(16.0f, true);

		return GUI.Button(rect, text);
	}

	protected void BeginError(bool error = true)
	{
		EditorGUILayout.BeginVertical(error == true ? P3D_Helper.Error : P3D_Helper.NoError);
	}

	protected void EndError()
	{
		EditorGUILayout.EndVertical();
	}

	protected void BeginColor(Color color)
	{
		guiColors.Push(GUI.color);

		GUI.color = color;
	}

	protected void BeginIndent()
	{
		EditorGUI.indentLevel += 1;
	}

	protected void EndIndent()
	{
		EditorGUI.indentLevel -= 1;
	}

	protected void BeginLabelWidth(float width)
	{
		labelWidths.Push(EditorGUIUtility.labelWidth);

		EditorGUIUtility.labelWidth = width;
	}

	protected void EndLabelWidth()
	{
		EditorGUIUtility.labelWidth = labelWidths.Pop();
	}

	protected void EndColor()
	{
		GUI.color = guiColors.Pop();
	}

	protected void BeginGroup(ref bool show, string title)
	{
		show = EditorGUILayout.Foldout(show, title, P3D_Helper.BoldFoldout);

		BeginIndent();
	}

	protected void EndGroup()
	{
		EndIndent();
	}

	protected void Splitter(float height = 16.0f)
	{
		var rect = P3D_Helper.Reserve(height);

		rect.xMin += 10;
		rect.xMax -= 10;
		rect.yMin += height * 0.5f - 2;
		rect.yMax -= height * 0.5f - 1;

		GUI.Box(rect, "", "box");
	}

	protected bool GroupSplitter(string name, ref bool show, float height = 16.0f)
	{
		EditorGUILayout.Separator();

		var rect = P3D_Helper.Reserve(height);

		show = EditorGUI.Foldout(rect, show, name);

		rect.xMin += 20;
		rect.xMax -= 10;
		rect.yMin += height * 0.5f - 2;
		rect.yMax -= height * 0.5f - 1;

		rect.xMin += EditorStyles.label.CalcSize(new GUIContent(name)).x;

		GUI.Box(rect, "", "box");

		return show;
	}
}
