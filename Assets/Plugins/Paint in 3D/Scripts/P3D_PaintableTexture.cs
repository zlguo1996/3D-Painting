using UnityEngine;
using System.Collections;

[System.Serializable]
public class P3D_PaintableTexture
{
	[Tooltip("If your paintable has more than one texture then you can specify a group to select just one")]
	public P3D_Group Group;

	[Tooltip("The material index we want to paint")]
	public int MaterialIndex;

	[Tooltip("The texture we want to paint")]
	public string TextureName = "_MainTex";

	[Tooltip("The UV set used when painting this texture")]
	public P3D_CoordType Coord = P3D_CoordType.UV1;

	[Tooltip("Should the material and texture get duplicated on awake? (useful for prefab clones)")]
	public bool DuplicateOnAwake;

	[Tooltip("Should the texture get created on awake? (useful for saving scene file size)")]
	public bool CreateOnAwake;

	[Tooltip("The width of the created texture")]
	public int CreateWidth = 512;

	[Tooltip("The height of the created texture")]
	public int CreateHeight = 512;

	[Tooltip("The pixel format of the created texture")]
	public P3D_Format CreateFormat;

	[Tooltip("The color of the created texture")]
	public Color CreateColor = Color.white;

	[Tooltip("Should the created etxture have mip maps?")]
	public bool CreateMipMaps = true;
	
	[Tooltip("Some shaders (e.g. Standard Shader) require you to enable keywords when adding new textures, you can specify that keyword here")]
	public string CreateKeyword;

	[SerializeField]
	private P3D_Painter painter;
	
	public P3D_Painter Painter
	{
		get
		{
			return painter;
		}
	}

	public void Paint(P3D_Brush brush, Vector2 uv)
	{
		if (painter != null)
		{
			painter.Paint(brush, uv);
		}
	}
	
	public void UpdateTexture(GameObject gameObject)
	{
		if (painter == null)
		{
			painter = new P3D_Painter();
		}

		painter.SetCanvas(gameObject, TextureName, MaterialIndex);
	}
	
	public void Awake(GameObject gameObject)
	{
		if (DuplicateOnAwake == true)
		{
			// Get cloned material
			var material = P3D_Helper.CloneMaterial(gameObject, MaterialIndex);

			if (material != null)
			{
				// Get texture
				var texture = material.GetTexture(TextureName);

				if (texture != null)
				{
					// Clone material
					texture = P3D_Helper.Clone(texture);

					// Update material
					material.SetTexture(TextureName, texture);
				}
			}
		}

		if (CreateOnAwake == true && CreateWidth > 0 && CreateHeight > 0)
		{
			var material = P3D_Helper.GetMaterial(gameObject, MaterialIndex);

			if (material != null)
			{
				var texture    = material.GetTexture(TextureName);
				var format     = P3D_Helper.GetTextureFormat(CreateFormat);
				var newTexture = P3D_Helper.CreateTexture(CreateWidth, CreateHeight, format, CreateMipMaps);

				if (texture != null)
				{
					Debug.LogWarning("There is already a texture in this texture slot, maybe set it to null to save memory?", gameObject);
				}
				
				texture = newTexture;

				P3D_Helper.ClearTexture(newTexture, CreateColor, true);

				material.SetTexture(TextureName, texture);

				// Enable a keyword?
				if (string.IsNullOrEmpty(CreateKeyword) == false)
				{
					material.EnableKeyword(CreateKeyword);
				}
            }
        }

		UpdateTexture(gameObject);
	}
}