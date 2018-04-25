using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class P3D_ClearWindow : P3D_EditorWindow
{
	private static Color newColor = Color.white;

	private Texture2D texture;
	
	public static void Pop(Rect rect, Texture2D newTexture)
	{
		var window = EditorWindow.CreateInstance<P3D_ClearWindow>();
		var size   = new Vector2(300.0f, 16.0f * 3 + 3);
		
		window.SetTitle("Clear");

		window.texture = newTexture;
		
		window.position = rect;
		window.minSize  = size;
		window.maxSize  = size;

		window.ShowAuxWindow();
	}

	protected override void OnInspector()
	{
		if (texture != null)
		{
			newColor = EditorGUILayout.ColorField("New Color", newColor);

			EditorGUILayout.Separator();
			
			if (Button("Clear") == true)
			{
				P3D_Main.Instance.Clear(newColor);

				Close();
			}
		}
		else
		{
			Close();
		}
	}
}
