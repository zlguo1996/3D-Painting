using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class P3D_Main
{
	[SerializeField]
	private bool showMesh;

	private void DrawMesh()
	{
		if (locked == true)
		{
			EditorGUILayout.Separator();

			BeginGroup(ref showMesh, "Mesh"); if (showMesh == true)
			{
				DrawSubmeshToolbar();
				
				if (Button("Refresh") == true)
				{
					tree.Clear();
				}
			}
			EndGroup();
		}
	}

	private void DrawSubmeshToolbar()
	{
		var submeshNames = new string[lockedMesh.subMeshCount];
		var index        = Mathf.Min(currentMaterialIndex, lockedMesh.subMeshCount - 1);

		for (var i = submeshNames.Length - 1; i >= 0; i--)
		{
			submeshNames[i] = "Submesh " + i;
		}

		EditorGUI.BeginDisabledGroup(true);
		{
			GUI.Toolbar(P3D_Helper.Reserve(20.0f, true), index, submeshNames);
		}
		EditorGUI.EndDisabledGroup();
	}
}
