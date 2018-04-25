using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class P3D_Main
{
	public List<P3D_Brush> PresetBrushes = new List<P3D_Brush>();
	
	private static string EntryDelimiter = "\n";
	
	private static string ValueDelimiter = "=";

	[SerializeField]
	private bool showPresets;
	
	private void DrawPresets()
	{
		EditorGUILayout.Separator();

		BeginGroup(ref showPresets, "Presets"); if (showPresets == true)
		{
			if (PresetBrushes.Count == 0)
			{
				EditorGUILayout.HelpBox("Save a brush to make it a preset.", MessageType.Info);
			}
		
			for (var i = 0; i < PresetBrushes.Count; i++)
			{
				var presetBrush = PresetBrushes[i];
				var rect        = P3D_Helper.Reserve();
				var rect1       = new Rect(rect.xMax - 20.0f, rect.y, 20.0f, rect.height);
				var rect2       = new Rect(rect.xMax - 62.0f, rect.y, 40.0f, rect.height);
			
				EditorGUI.LabelField(rect, presetBrush.Name);
			
				if (GUI.Button(rect1, "X") == true)
				{
					if (EditorUtility.DisplayDialog("Delete brush preset?", "Are you sure you want to delete this brush preset?", "Yes", "No") == true)
					{
						PresetBrushes.RemoveAt(i);
					}
				}
			
				if (GUI.Button(rect2, "load") == true)
				{
					currentBrush.Name         = presetBrush.Name;
					currentBrush.Blend        = presetBrush.Blend;
					currentBrush.Color        = presetBrush.Color;
					currentBrush.Opacity      = presetBrush.Opacity;
					currentBrush.Direction    = presetBrush.Direction;
					currentBrush.Shape        = presetBrush.Shape;
					currentBrush.Size         = presetBrush.Size;
					currentBrush.Detail       = presetBrush.Detail;
					currentBrush.DetailScale = presetBrush.DetailScale;
				}
			}
		}
		EndGroup();
	}
	
	private void LoadPresets()
	{
		PresetBrushes.Clear();
		
		var text        = EditorPrefs.GetString("P3D_PresetBrushes", "");
		var tokens      = text.Split(new string[] { EntryDelimiter }, System.StringSplitOptions.RemoveEmptyEntries);
		var presetBrush = default(P3D_Brush);
		
		foreach (var token in tokens)
		{
			var bits = token.Split(new string[] { ValueDelimiter }, System.StringSplitOptions.RemoveEmptyEntries);
			
			if (bits.Length == 2)
			{
				switch (bits[0])
				{
					case "Name":
					{
						presetBrush = new P3D_Brush();
						
						presetBrush.Name = bits[1];
						
						PresetBrushes.Add(presetBrush);
					}
					break;
					
					case "Blend":        presetBrush.Blend         = (P3D_BlendMode)System.Enum.Parse(typeof(P3D_BlendMode), bits[1]); break;
					case "Opacity":      presetBrush.Opacity       = float.Parse(bits[1]); break;
					case "ColorR":       presetBrush.Color.r       = float.Parse(bits[1]); break;
					case "ColorG":       presetBrush.Color.g       = float.Parse(bits[1]); break;
					case "ColorB":       presetBrush.Color.b       = float.Parse(bits[1]); break;
					case "ColorA":       presetBrush.Color.a       = float.Parse(bits[1]); break;
					case "DirectionX":   presetBrush.Direction.x   = float.Parse(bits[1]); break;
					case "DirectionY":   presetBrush.Direction.y   = float.Parse(bits[1]); break;
					case "Shape":        presetBrush.Shape         = Deserialize(bits[1]); break;
					case "SizeX":        presetBrush.Size.x        = float.Parse(bits[1]); break;
					case "SizeY":        presetBrush.Size.y        = float.Parse(bits[1]); break;
					case "Detail":       presetBrush.Detail        = Deserialize(bits[1]); break;
					case "DetailScaleX": presetBrush.DetailScale.x = float.Parse(bits[1]); break;
					case "DetailScaleY": presetBrush.DetailScale.y = float.Parse(bits[1]); break;
				}
			}
		}
	}
	
	private void SavePresets()
	{
		var text = "";
		
		foreach (var presetBrush in PresetBrushes)
		{
			text += "Name"         + ValueDelimiter + presetBrush.Name              + EntryDelimiter;
			text += "Blend"        + ValueDelimiter + presetBrush.Blend             + EntryDelimiter;
			text += "Opacity"      + ValueDelimiter + presetBrush.Opacity           + EntryDelimiter;
			text += "ColorR"       + ValueDelimiter + presetBrush.Color.r           + EntryDelimiter;
			text += "ColorG"       + ValueDelimiter + presetBrush.Color.g           + EntryDelimiter;
			text += "ColorB"       + ValueDelimiter + presetBrush.Color.b           + EntryDelimiter;
			text += "ColorA"       + ValueDelimiter + presetBrush.Color.a           + EntryDelimiter;
			text += "DirectionX"   + ValueDelimiter + presetBrush.Direction.x       + EntryDelimiter;
			text += "DirectionY"   + ValueDelimiter + presetBrush.Direction.y       + EntryDelimiter;
			text += "Shape"        + ValueDelimiter + Serialize(presetBrush.Shape)  + EntryDelimiter;
			text += "SizeX"        + ValueDelimiter + presetBrush.Size.x            + EntryDelimiter;
			text += "SizeY"        + ValueDelimiter + presetBrush.Size.y            + EntryDelimiter;
			text += "Detail"       + ValueDelimiter + Serialize(presetBrush.Detail) + EntryDelimiter;
			text += "DetailScaleX" + ValueDelimiter + presetBrush.DetailScale.x    + EntryDelimiter;
			text += "DetailScaleY" + ValueDelimiter + presetBrush.DetailScale.y    + EntryDelimiter;
		}
		
		EditorPrefs.SetString("P3D_PresetBrushes", text);
	}
	
	private Texture2D Deserialize(string path)
	{
		if (string.IsNullOrEmpty(path) == false)
		{
			return AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
		}
		
		return null;
	}
	
	private string Serialize(Texture2D texture)
	{
		if (texture != null)
		{
			return AssetDatabase.GetAssetPath(texture);
		}
		
		return null;
	}
}