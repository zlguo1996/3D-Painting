using UnityEngine;
using System.Collections.Generic;

// This class allows you to paint a single texture using a brush
[System.Serializable]
public class P3D_Painter
{
	// Has this canvas been modified since it was last applied?
	public bool Dirty;
	
	// The texture we're painting to
	public Texture2D Canvas;

	// The tiling value of the canvas
	public Vector2 Tiling = Vector2.one;
	
	// The offset value of the canvas
	public Vector2 Offset;
	
	public bool IsReady
	{
		get
		{
			return Canvas != null;
		}
	}

	// This allows you to change which texture is currently used when doing painting, via GameObject
	public void SetCanvas(GameObject gameObject, string textureName = "_MainTex", int newMaterialIndex = 0)
	{
		var material = P3D_Helper.GetMaterial(gameObject, newMaterialIndex);
		
		if (material != null)
		{
			SetCanvas(material.GetTexture(textureName), material.GetTextureScale(textureName), material.GetTextureOffset(textureName));
		}
		else
		{
			SetCanvas(null, Vector2.zero, Vector2.zero);
		}
	}
	
	// This allows you to change which texture is currently used when doing painting
	public void SetCanvas(Texture newTexture)
	{
		SetCanvas(newTexture, Vector2.one, Vector2.zero);
	}

	public void SetCanvas(Texture newTexture, Vector2 newTiling, Vector2 newOffset)
	{
		var newCanvas = newTexture as Texture2D;
		
		if (newCanvas != null && newTiling.x != 0.0f && newTiling.y != 0.0f)
		{
			if (P3D_Helper.IsWritableFormat(newCanvas.format) == false)
			{
				throw new System.Exception("Trying to paint a non-writable texture");
			}

			Canvas = newCanvas;
			Tiling = newTiling;
			Offset = newOffset;
#if UNITY_EDITOR
			P3D_Helper.MakeTextureReadable(Canvas);
#endif
		}
		else
		{
			Canvas = null;
		}
	}
	
	// This causes the current paint operation to get applied to all the specified results
	public bool Paint(P3D_Brush brush, List<P3D_Result> results, P3D_CoordType coord = P3D_CoordType.UV1)
	{
		var painted = false;

		if (results != null)
		{
			for (var i = 0; i < results.Count; i++)
			{
				painted |= Paint(brush, results[i], coord);
			}
		}

		return painted;
	}

	// This causes the current paint operation to get applied to the specified result
	public bool Paint(P3D_Brush brush, P3D_Result result, P3D_CoordType coord = P3D_CoordType.UV1)
	{
		if (result != null)
		{
			return Paint(brush, result.GetUV(coord));
		}

		return false;
	}
	
	// This causes the current paint operation to get applied to the specified u/v coordinate
	public bool Paint(P3D_Brush brush, Vector2 uv)
	{
		if (Canvas != null)
		{
			var xy = P3D_Helper.CalculatePixelFromCoord(uv, Tiling, Offset, Canvas.width, Canvas.height);

			return Paint(brush, xy.x, xy.y);
		}

		return false;
	}

	// This causes the current paint operation to get applied to the specified u/v coordinate
	public bool Paint(P3D_Brush brush, float x, float y)
	{
		if (brush != null)
		{
			var xy     = new Vector2(x, y);
			var matrix = P3D_Helper.CreateMatrix(xy + brush.Offset, brush.Size, brush.Angle);

			return Paint(brush, matrix);
		}

		return false;
	}

	// This causes the current paint operation to get applied to the specified matrix in pixel space
	public bool Paint(P3D_Brush brush, P3D_Matrix matrix)
	{
		if (Canvas != null && brush != null)
		{
			brush.Paint(Canvas, matrix);

			Dirty = true;

			return true;
		}

		return false;
	}
	
	// This applys all texture changes after you've finished painting
	public void Apply()
	{
		if (Canvas != null && Dirty == true)
		{
			Dirty = false;

			Canvas.Apply();
		}
	}
}
