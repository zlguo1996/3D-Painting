#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

public abstract class P3D_PropertyDrawer<T> : PropertyDrawer
{
	protected T Target;

	protected bool isLive;

	protected SerializedProperty property;

	protected Rect position;

	protected GUIContent label;

	protected float totalHeight;

	public override float GetPropertyHeight(SerializedProperty newProperty, GUIContent newLabel)
	{
		isLive      = false;
		Target      = (T)fieldInfo.GetValue(newProperty.serializedObject.targetObject);
		totalHeight = 0.0f;
		property    = newProperty;
		label       = newLabel;

		OnInspector();

		return totalHeight;
	}

	public override void OnGUI(Rect newPosition, SerializedProperty newProperty, GUIContent newLabel)
	{
		isLive      = true;
		totalHeight = 0.0f;
		position    = newPosition;
		property    = newProperty;
		label       = newLabel;
		
		OnInspector();
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
		var rect = Reserve();

		if (isLive == true)
		{
			return GUI.Button(rect, text);
		}

		return false;
	}

	protected Rect Reserve(float height = 16.0f)
	{
		var rect = position;
		
		rect.y     += totalHeight;
		rect.height = height;

		totalHeight += height + 4.0f;

		return rect;
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
	
	protected void DrawDefault(SerializedProperty property, string propertyPathRelative)
	{
		var rect = Reserve();

		if (isLive == true)
		{
			EditorGUI.PropertyField(rect, property.FindPropertyRelative(propertyPathRelative), true);
		}
	}

	protected virtual void OnInspector()
	{
	}
}
#endif