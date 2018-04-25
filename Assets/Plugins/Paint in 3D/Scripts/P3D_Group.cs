using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(P3D_Group))]
public class P3D_Group_Drawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var sObj = property.serializedObject;
		var sPro = property.FindPropertyRelative("index");

		if (sPro.intValue < 0 || sPro.intValue > 31)
		{
			sPro.intValue = Mathf.Clamp(sPro.intValue, 0, 31);

			sObj.ApplyModifiedProperties();
		}
		
		var right  = position; right.xMin += EditorGUIUtility.labelWidth;
		var handle = "Group " + sPro.intValue;

		EditorGUI.LabelField(position, property.displayName);
		
		if (GUI.Button(right, handle, EditorStyles.popup) == true)
		{
			var menu = new GenericMenu();
			
			for (var i = 0; i < 32; i++)
			{
				var index   = i;
				var content = new GUIContent("Group " + i);
				var on      = sPro.intValue == index;

				menu.AddItem(content, on, () => { sPro.intValue = index; sObj.ApplyModifiedProperties(); });
			}
			
			menu.DropDown(right);
		}
	}
}
#endif

[System.Serializable]
public struct P3D_Group
{
	[SerializeField]
	private int index;
	
	public P3D_Group(int newIndex)
	{
		if (newIndex <= 0)
		{
			index = 0;
		}
		else if (newIndex >= 31)
		{
			index = 31;
		}
		else
		{
			index = newIndex;
		}
	}

	public static implicit operator int(P3D_Group group)
	{
		return group.index;
	}

	public static implicit operator P3D_Group(int index)
	{
		return new P3D_Group(index);
	}
}