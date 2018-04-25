#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

public abstract class P3D_Editor<T> : Editor
	where T : MonoBehaviour
{
	protected T Target;

	protected T[] Targets;

	public override void OnInspectorGUI()
	{
		P3D_Helper.BaseRect    = P3D_Helper.Reserve(0.0f);
		P3D_Helper.BaseRectSet = true;

		EditorGUI.BeginChangeCheck();

		serializedObject.UpdateIfDirtyOrScript();

		Target  = (T)target;
		Targets = targets.Select(t => (T)t).ToArray();

		Separator();

		OnInspector();

		Separator();

		serializedObject.ApplyModifiedProperties();

		if (EditorGUI.EndChangeCheck() == true)
		{
			GUI.changed = true; Repaint();

			foreach (var t in Targets)
			{
				P3D_Helper.SetDirty(t);
			}
		}

		P3D_Helper.BaseRectSet = false;
	}

	public virtual void OnSceneGUI()
	{
		Target = (T)target;

		OnScene();

		if (GUI.changed == true)
		{
			P3D_Helper.SetDirty(target);
		}
	}

	protected void Each(System.Action<T> update)
	{
		foreach (var t in Targets)
		{
			update(t);
		}
	}

	protected bool Any(System.Func<T, bool> check)
	{
		foreach (var t in Targets)
		{
			if (check(t) == true)
			{
				return true;
			}
		}

		return false;
	}

	protected bool All(System.Func<T, bool> check)
	{
		foreach (var t in Targets)
		{
			if (check(t) == false)
			{
				return false;
			}
		}

		return true;
	}

	protected virtual void Separator()
	{
		EditorGUILayout.Separator();
	}

	protected void BeginIndent()
	{
		EditorGUI.indentLevel += 1;
	}

	protected void EndIndent()
	{
		EditorGUI.indentLevel -= 1;
	}

	protected bool Button(string text)
	{
		var rect = P3D_Helper.Reserve();

		return GUI.Button(rect, text);
	}

	protected bool HelpButton(string helpText, UnityEditor.MessageType type, string buttonText, float buttonWidth)
	{
		var clicked = false;

		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.HelpBox(helpText, type);

			var style = new GUIStyle(GUI.skin.button); style.wordWrap = true;

			clicked = GUILayout.Button(buttonText, style, GUILayout.ExpandHeight(true), GUILayout.Width(buttonWidth));
		}
		EditorGUILayout.EndHorizontal();

		return clicked;
	}

	protected void BeginMixed(bool mixed = true)
	{
		EditorGUI.showMixedValue = mixed;
	}

	protected void EndMixed()
	{
		EditorGUI.showMixedValue = false;
	}

	protected void BeginDisabled(bool disabled = true)
	{
		EditorGUI.BeginDisabledGroup(disabled);
	}

	protected void EndDisabled()
	{
		EditorGUI.EndDisabledGroup();
	}

	protected void BeginError(bool error = true)
	{
		EditorGUILayout.BeginVertical(error == true ? P3D_Helper.Error : P3D_Helper.NoError);
	}

	protected void EndError()
	{
		EditorGUILayout.EndVertical();
	}

	protected void DrawDefault(string propertyPath)
	{
		EditorGUILayout.BeginVertical(P3D_Helper.NoError);
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyPath), true);
		}
		EditorGUILayout.EndVertical();
	}

	protected void DrawDefault(SerializedProperty property, string propertyPathRelative)
	{
		EditorGUILayout.BeginVertical(P3D_Helper.NoError);
		{
			EditorGUILayout.PropertyField(property.FindPropertyRelative(propertyPathRelative), true);
		}
		EditorGUILayout.EndVertical();
	}

	protected virtual void OnInspector()
	{
	}

	protected virtual void OnScene()
	{
	}
}
#endif