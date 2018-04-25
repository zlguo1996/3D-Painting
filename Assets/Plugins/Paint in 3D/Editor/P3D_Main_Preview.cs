using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class P3D_Main
{
	[SerializeField]
	private float previewTextureOpacity = 0.0f;

	[SerializeField]
	private float previewBrushOpacity = 1.0f;

	[SerializeField]
	private float previewSize = 128.0f;

	[SerializeField]
	private bool showWireframe = true;

	[SerializeField]
	private bool showPreview;

	private void DrawPreview()
	{
		EditorGUILayout.Separator();

		BeginGroup(ref showPreview, "Preview"); if (showPreview == true)
		{
			BeginLabelWidth(Mathf.Min(85.0f, position.width * 0.5f));
			{
				previewTextureOpacity = EditorGUILayout.Slider("Texture", previewTextureOpacity, 0.0f, 1.0f);

				previewBrushOpacity = EditorGUILayout.Slider("Brush", previewBrushOpacity, 0.0f, 1.0f);

				previewSize = EditorGUILayout.Slider("Size", previewSize, 64.0f, 512.0f);

				showWireframe = EditorGUILayout.Toggle("Wireframe", showWireframe);

				DrawCurrentTexture();
			}
			EndLabelWidth();
		}
		EndGroup();
	}

	private void DrawCurrentTexture()
	{
		if (currentTexture != null)
		{
			var rect1  = P3D_Helper.Reserve(previewSize, true);
			var rect2  = rect1;
			var aspect = currentTexture.width / (float)currentTexture.height;
			var ratio  = rect1.width / rect1.height;

			GUI.Box(rect1, "", "box");

			rect2.xMin += 1;
			rect2.yMin += 1;
			rect2.xMax -= 1;
			rect2.yMax -= 1;

			if (ratio > aspect)
			{
				rect2.width *= aspect / ratio;
			}
			else
			{
				rect2.height *= ratio / aspect;
			}

			rect2.center = rect1.center;

			GUI.DrawTexture(rect2, currentTexture, ScaleMode.StretchToFill);

			rect1.yMax -= 5.0f;

			EditorGUI.DropShadowLabel(rect1, "(" + currentTexture.width + " x " + currentTexture.height + ")");
		}
	}

	private void ShowTexturePreview()
	{
		if (currentTexture != null && previewTextureOpacity > 0.0f)
		{
			var meshFilter = lockedGameObject.GetComponent<MeshFilter>();

			if (meshFilter != null)
			{
				var mesh = meshFilter.sharedMesh;

				if (mesh != null)
				{
					P3D_TexturePreview.Show(mesh, currentMaterialIndex, lockedGameObject.transform, previewTextureOpacity, currentTexture, CurrentTiling, CurrentOffset);
				}
			}
		}
	}

	private void ShowBrushPreview(Camera camera, Vector2 mousePosition)
	{
		if (lockedMesh != null && currentTexture != null && previewBrushOpacity > 0.0f)// && currentTool != ToolType.Fill)
		{
			var ray           = HandleUtility.GUIPointToWorldRay(mousePosition);
			var startPosition = ray.origin + ray.direction * camera.nearClipPlane;
			var endPosition   = ray.origin + ray.direction * camera.farClipPlane;
			var start         = lockedGameObject.transform.InverseTransformPoint(startPosition);
			var end           = lockedGameObject.transform.InverseTransformPoint(endPosition);

			tree.SetMesh(lockedMesh, currentMaterialIndex);

			if (passThrough == true)// && currentTool != ToolType.Fill)
			{
				var results = tree.FindBetweenAll(start, end);

				for (var i = results.Count - 1; i >= 0; i--)
				{
					ShowBrushPreview(results[i]);
				}
			}
			else
			{
				var result = tree.FindBetweenNearest(start, end);

				if (result != null)
				{
					ShowBrushPreview(result);
				}
			}
		}
	}

	private void ShowBrushPreview(P3D_Result result)
	{
		var width      = currentTexture.width;
		var height     = currentTexture.height;
		var uv         = result.GetUV(currentCoord);
		var xy         = P3D_Helper.CalculatePixelFromCoord(uv, CurrentTiling, CurrentOffset, width, height);
		var matrix     = P3D_Helper.CreateMatrix(xy + new Vector2(0.01f, 0.01f), currentBrush.Size * 0.999f, currentBrush.Angle).Inverse;
		var resolution = new Vector2(width, height);
		
		P3D_BrushPreview.Show(lockedMesh, currentMaterialIndex, lockedGameObject.transform, previewBrushOpacity, matrix, resolution, currentBrush.Shape, CurrentTiling, CurrentOffset);
	}
}
