using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class P3D_UndoState
{
	public Texture2D Texture;
	
	public int Width;
	
	public int Height;
	
	public Color32[] Pixels;
	
	public P3D_UndoState(Texture2D newTexture)
	{
		if (newTexture != null)
		{
			Texture = newTexture;
			Width   = newTexture.width;
			Height  = newTexture.height;
			Pixels  = newTexture.GetPixels32();
		}
	}
	
	public void Perform()
	{
		if (Texture != null)
		{
			if (Texture.width != Width || Texture.height != Height)
			{
				Texture.Resize(Width, Height);
			}
			
			Texture.SetPixels32(Pixels);
			Texture.Apply();
		}
	}
}