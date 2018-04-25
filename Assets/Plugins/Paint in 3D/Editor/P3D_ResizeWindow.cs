using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class P3D_ResizeWindow : P3D_EditorWindow
{
	private static int newWidth = 256;

	private static int newHeight = 256;

	private Texture2D texture;
	
	public static void Pop(Rect rect, Texture2D newTexture)
	{
		var window = EditorWindow.CreateInstance<P3D_ResizeWindow>();
		var size   = new Vector2(300.0f, 16.0f * 7);
		
		window.SetTitle("Resize");

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
			EditorGUI.BeginDisabledGroup(true);
			{
				EditorGUILayout.IntField("Current Width", texture.width);
			}
			EditorGUI.EndDisabledGroup();
			
			newWidth = EditorGUILayout.IntField("New Width", newWidth);

			EditorGUILayout.Separator();

			EditorGUI.BeginDisabledGroup(true);
			{
				EditorGUILayout.IntField("Current Height", texture.height);
			}
			EditorGUI.EndDisabledGroup();
			
			newHeight = EditorGUILayout.IntField("New Height", newHeight);
			
			EditorGUILayout.Separator();
			
			EditorGUI.BeginDisabledGroup(newWidth == texture.width && newHeight == texture.height);
			{
				if (Button("Resize") == true)
				{
					P3D_Main.Instance.Resize(newWidth, newHeight);

					Close();
				}
			}
			EditorGUI.EndDisabledGroup();
		}
		else
		{
			Close();
		}
	}
}
