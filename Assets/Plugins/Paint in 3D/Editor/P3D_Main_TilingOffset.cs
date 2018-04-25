using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class P3D_Main
{
	[SerializeField]
	private Vector2 customTiling = Vector2.one;
	
	[SerializeField]
	private Vector2 customOffset;
	
	[SerializeField]
	private bool overrideTilingOffset;
	
	[SerializeField]
	private bool showTilingOffset;
	
	public Vector2 CurrentTiling
	{
		get
		{
			if (overrideTilingOffset == false && currentMaterial != null)
			{
				return currentMaterial.GetTextureScale(currentTexEnvName);
			}
			
			return customTiling;
		}
	}
	
	public Vector2 CurrentOffset
	{
		get
		{
			if (overrideTilingOffset == false && currentMaterial != null)
			{
				return currentMaterial.GetTextureOffset(currentTexEnvName);
			}
			
			return customOffset;
		}
	}
	
	private void DrawTilingOffset()
	{
		if (currentTexture != null)
		{
			EditorGUILayout.Separator();

			BeginGroup(ref showTilingOffset, "Tiling & Offset"); if (showTilingOffset == true)
			{
				BeginLabelWidth(Mathf.Min(85.0f, position.width * 0.5f));
				{
					DrawOverrideTilingOffset();
				}
				EndLabelWidth();
			}
			EndGroup();
		}
	}
	
	private void DrawOverrideTilingOffset()
	{
		overrideTilingOffset = EditorGUILayout.Toggle("Override", overrideTilingOffset);
		
		EditorGUI.BeginDisabledGroup(overrideTilingOffset == false);
		{
			var rect   = P3D_Helper.Reserve(48.0f); rect.yMax = rect.yMin + 13.0f;
			var left   = rect;   left.xMax =   left.xMin + 24.0f;
			var middle = rect; middle.xMin = middle.xMin + 25.0f;
			var right  = P3D_Helper.SplitHorizontal(ref middle, 1);
			
			EditorGUI.LabelField(new Rect(left.xMin, left.yMin + 16.0f, left.width, left.height + 3.0f), "x", P3D_Helper.SmallLeftText);
			EditorGUI.LabelField(new Rect(left.xMin, left.yMin + 32.0f, left.width, left.height + 3.0f), "y", P3D_Helper.SmallLeftText);
			
			EditorGUI.LabelField(new Rect(middle.xMin, middle.yMin, middle.width, middle.height + 3.0f), "Tiling", P3D_Helper.SmallTopText);
			EditorGUI.LabelField(new Rect(right.xMin, right.yMin, right.width, right.height + 3.0f), "Offset", P3D_Helper.SmallTopText);
			
			customTiling.x = EditorGUI.FloatField(new Rect(middle.xMin, middle.yMin + 16.0f, middle.width, middle.height), customTiling.x, P3D_Helper.SmallEntryText);
			customTiling.y = EditorGUI.FloatField(new Rect(middle.xMin, middle.yMin + 32.0f, middle.width, middle.height), customTiling.y, P3D_Helper.SmallEntryText);
			
			customOffset.x = EditorGUI.FloatField(new Rect(right.xMin, right.yMin + 16.0f, right.width, right.height), customOffset.x, P3D_Helper.SmallEntryText);
			customOffset.y = EditorGUI.FloatField(new Rect(right.xMin, right.yMin + 32.0f, right.width, right.height), customOffset.y, P3D_Helper.SmallEntryText);
		}
		EditorGUI.EndDisabledGroup();
	}
}