using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(P3D_Paintable))]
public class P3D_Paintable_Editor : P3D_Editor<P3D_Paintable>
{
	protected override void OnInspector()
	{
		if (Any(InvalidRenderer))
		{
			EditorGUILayout.HelpBox("There is no renderer attached to this GameObject", MessageType.Error);
		}
		
		BeginError(Any(SubMeshOob));
		{
			DrawDefault("SubMeshIndex");
		}
		EndError();

		DrawDefault("UpdateInterval");

		BeginError(Any(t => t.ApplyInterval < 0.0f));
		{
			DrawDefault("ApplyInterval");
		}
		EndError();

		Separator();
		
		if (Targets.Length == 1)
		{
			var texturesProp = serializedObject.FindProperty("Textures");
			var textures     = Target.Textures;
			
			if (textures == null)
			{
				textures = Target.Textures = new List<P3D_PaintableTexture>();
			}

			for (var i = 0; i < texturesProp.arraySize; i++)
			{
				var textureProp = texturesProp.GetArrayElementAtIndex(i);
				var texture     = Target.Textures[i];

				if (texture == null)
				{
					texture = Target.Textures[i] = new P3D_PaintableTexture();
				}

				GUILayout.BeginVertical(EditorStyles.helpBox);
				{
					var duplicate1 = textures.Exists(t => t != null && t != texture && t.Group == texture.Group);
					var duplicate2 = textures.Exists(t => t != null && t != texture && t.MaterialIndex == texture.MaterialIndex && t.TextureName == texture.TextureName);

					BeginError(duplicate1 == true);
					{
						DrawDefault(textureProp, "Group");
					}
					EndError();
					
					BeginError(MaterialOob(Target, texture) == true || duplicate2 == true);
					{
						DrawDefault(textureProp, "MaterialIndex");
					}
					EndError();

					BeginError(NameOob(Target, texture) == true || duplicate2 == true);
					{
						DrawDefault(textureProp, "TextureName");
					}
					EndError();

					DrawDefault(textureProp, "Coord");
					DrawDefault(textureProp, "DuplicateOnAwake");
					DrawDefault(textureProp, "CreateOnAwake");

					if (texture.CreateOnAwake == true)
					{
						BeginIndent();
						{
							BeginError(texture.CreateWidth <= 0);
							{
								DrawDefault(textureProp, "CreateWidth");
							}
							EndError();

							BeginError(texture.CreateWidth <= 0);
							{
								DrawDefault(textureProp, "CreateHeight");
							}
							EndError();

							DrawDefault(textureProp, "CreateFormat");
							DrawDefault(textureProp, "CreateColor");
							DrawDefault(textureProp, "CreateMipMaps");
							DrawDefault(textureProp, "CreateKeyword");
						}
						EndIndent();
					}

					Separator();

					if (Button("Delete") == true)
					{
						texturesProp.DeleteArrayElementAtIndex(i--);
					}
				}
				GUILayout.EndVertical();
			}

			if (Button("Add Texture") == true)
			{
				Target.AddTexture();
			}
		}
	}

	private bool InvalidRenderer(P3D_Paintable paintable)
	{
		if (paintable.GetComponent<MeshRenderer>() != null && paintable.GetComponent<MeshFilter>() != null)
		{
			return false;
		}

		if (paintable.GetComponent<SkinnedMeshRenderer>() != null)
		{
			return false;
		}

		return true;
	}
	
	private bool NameOob(P3D_Paintable paintable, P3D_PaintableTexture texture)
	{
		var renderer = paintable.GetComponent<Renderer>();

		if (renderer != null)
		{
			if (texture.MaterialIndex >= 0)
			{
				var materials = renderer.sharedMaterials;

				if (texture.MaterialIndex < materials.Length)
				{
					var material = materials[texture.MaterialIndex];
					var names    = P3D_Helper.GetTexEnvNames(material);

					if (names.Contains(texture.TextureName) == true)
					{
						return false;
					}
				}
            }
		}

		return true;
	}
	
	private bool SubMeshOob(P3D_Paintable paintable)
	{
		var meshFilter = paintable.GetComponent<MeshFilter>();
		
		if (meshFilter != null)
		{
			if (paintable.SubMeshIndex >= 0 && paintable.SubMeshIndex < meshFilter.sharedMesh.subMeshCount)
			{
				return false;
			}
		}

		var skinnedMeshRenderer = paintable.GetComponent<SkinnedMeshRenderer>();
		
		if (skinnedMeshRenderer != null)
		{
			if (paintable.SubMeshIndex >= 0 && paintable.SubMeshIndex < skinnedMeshRenderer.sharedMesh.subMeshCount)
			{
				return false;
			}
		}

		return true;
	}

	private bool MaterialOob(P3D_Paintable paintable, P3D_PaintableTexture texture)
	{
		var renderer = paintable.GetComponent<Renderer>();

		if (renderer != null)
		{
			if (texture.MaterialIndex >= 0 && texture.MaterialIndex < renderer.sharedMaterials.Length)
			{
				return false;
			}
		}

		return true;
	}
}
