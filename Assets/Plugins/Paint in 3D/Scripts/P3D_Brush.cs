using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(P3D_Brush))]
public class P3D_Brush_Drawer : P3D_PropertyDrawer<P3D_Brush>
{
	private static bool show;

	protected override void OnInspector()
	{
		var labelRect = Reserve();

		if (isLive == true)
		{
			show = EditorGUI.Foldout(labelRect, show, label);
		}

		if (show == true)
		{
			BeginIndent();
			{
				//DrawDefault(property, "Name");
				DrawDefault(property, "Opacity");
				DrawDefault(property, "Angle");
				DrawDefault(property, "Offset");
				DrawDefault(property, "Size");
				DrawDefault(property, "Blend");
				DrawDefault(property, "Shape");
				DrawDefault(property, "Color");

				if (Target.Blend == P3D_BlendMode.NormalBlend)
				{
					if (Target.Shape == null || Target.Shape.format == TextureFormat.Alpha8)
					{
						DrawDefault(property, "Direction");
					}
				}
				
				DrawDefault(property, "Detail");
				DrawDefault(property, "DetailScale");
			}
			EndIndent();
		}
	}
}
#endif

// This stores all the data for a single  This can be passed to P3D painter to construct a paint operation
[System.Serializable]
public partial class P3D_Brush
{
	// This gets called before a brush is applied to a texture
	public static System.Action<Texture2D, P3D_Rect> OnPrePaint;

	// This gets called after a brush is applied to a texture
	public static System.Action<Texture2D, P3D_Rect> OnPostPaint;

	[Tooltip("The name of this brush (mainly used for saving/loading)")]
	public string Name = "Default";

	[Tooltip("The opacity of the brush (how solid it is)")]
	[Range(0.0f, 1.0f)]
	public float Opacity = 1.0f;

	[Tooltip("The angle of the brush in radians")]
	[Range(-Mathf.PI, Mathf.PI)]
	public float Angle;

	[Tooltip("The amount of pixels the brush gets moved from the pain location")]
	public Vector2 Offset;

	[Tooltip("The size of the brush in pixels")]
	public Vector2 Size = new Vector2(10.0f, 10.0f);

	[Tooltip("The blend mode of the brush")]
	public P3D_BlendMode Blend = P3D_BlendMode.AlphaBlend;

	[Tooltip("The shape of the brush")]
	public Texture2D Shape;

	[Tooltip("The color of the brush")]
	public Color Color = Color.white;
	
	[Tooltip("The normal direction of the brush (used for NormalBlend)")]
	public Vector2 Direction;
	
	[Tooltip("The detail texture when painting")]
	public Texture2D Detail;

	[Tooltip("The scale of the detail texture, allowing you to tile it")]
	public Vector2 DetailScale = new Vector2(0.5f, 0.5f);
	
	// Cached variables used during painting
	private static Texture2D  canvas;
	private static int        canvasW;
	private static int        canvasH;
	private static P3D_Rect   rect;
	private static P3D_Matrix matrix;
	private static P3D_Matrix inverse;
	private static float      opacity;
	private static Color      color;
	private static Vector2    direction;
	private static Texture2D  shape;
	private static Texture2D  detail;
	private static Vector2    detailScale;
	
	private static P3D_Brush tempInstance;

	public static P3D_Brush TempInstance
	{
		get
		{
			if (tempInstance == null)
			{
				tempInstance = new P3D_Brush();
			}

			return tempInstance;
		}
	}

	public P3D_Brush GetTempClone()
	{
		CopyTo(TempInstance);
		
		return tempInstance;
	}

	public void CopyTo(P3D_Brush other)
	{
		if (other != null)
		{
			other.Name        = Name;
			other.Opacity     = Opacity;
			other.Angle       = Angle;
			other.Offset      = Offset;
			other.Size        = Size;
			other.Blend       = Blend;
			other.Color       = Color;
			other.Direction   = Direction;
			other.Shape       = Shape;
			other.Detail      = Detail;
			other.DetailScale = DetailScale;
		}
	}

	public void Paint(Texture2D newCanvas, P3D_Matrix newMatrix)
	{
		canvas  = newCanvas;
		canvasW = newCanvas.width;
		canvasH = newCanvas.height;
		matrix  = newMatrix;

		if (CalculateRect(ref rect) == true)
		{
			inverse     = newMatrix.Inverse;
			opacity     = Opacity;
			color       = Color;
			direction   = Direction;
			shape       = Shape;
			detail      = Detail;
			detailScale = DetailScale;

			if (OnPrePaint != null) OnPrePaint(canvas, rect);
			
			switch (Blend)
			{
				case P3D_BlendMode.AlphaBlend: AlphaBlend.Paint(); break;
				case P3D_BlendMode.AlphaBlendRgb: AlphaBlendRGB.Paint(); break;
				case P3D_BlendMode.AlphaErase: AlphaErase.Paint(); break;
				case P3D_BlendMode.AdditiveBlend: AdditiveBlend.Paint(); break;
				case P3D_BlendMode.SubtractiveBlend: SubtractiveBlend.Paint(); break;
				case P3D_BlendMode.NormalBlend: NormalBlend.Paint(); break;
				case P3D_BlendMode.Replace: Replace.Paint(); break;
			}

			if (OnPostPaint != null) OnPostPaint(canvas, rect);
		}
	}
	
	private bool CalculateRect(ref P3D_Rect rect)
	{
		// Grab transformed corners
		var a = matrix.MultiplyPoint(0.0f, 0.0f);
		var b = matrix.MultiplyPoint(1.0f, 0.0f);
		var c = matrix.MultiplyPoint(0.0f, 1.0f);
		var d = matrix.MultiplyPoint(1.0f, 1.0f);

		// Find min/max x/y
		var xMin = Mathf.Min(Mathf.Min(a.x, b.x), Mathf.Min(c.x, d.x));
		var xMax = Mathf.Max(Mathf.Max(a.x, b.x), Mathf.Max(c.x, d.x));
		var yMin = Mathf.Min(Mathf.Min(a.y, b.y), Mathf.Min(c.y, d.y));
		var yMax = Mathf.Max(Mathf.Max(a.y, b.y), Mathf.Max(c.y, d.y));
		
		// Has volume?
		if (xMin < xMax && yMin < yMax)
		{
			// Make sure rect doesn't go outside canvas
			rect.XMin = Mathf.Clamp(Mathf.FloorToInt(xMin), 0, canvasW );
			rect.XMax = Mathf.Clamp(Mathf. CeilToInt(xMax), 0, canvasW );
			rect.YMin = Mathf.Clamp(Mathf.FloorToInt(yMin), 0, canvasH);
			rect.YMax = Mathf.Clamp(Mathf. CeilToInt(yMax), 0, canvasH);
			
			return true;
		}

		return false;
	}

	private static bool IsInsideShape(P3D_Matrix inverseMatrix, int x, int y, ref Vector2 shapeCoord)
	{
		shapeCoord = inverseMatrix.MultiplyPoint(x, y);

		if (shapeCoord.x >= 0.0f && shapeCoord.x < 1.0f)
		{
			if (shapeCoord.y >= 0.0f && shapeCoord.y < 1.0f)
			{
				return true;
			}
		}

		return false;
	}

	private static Color SampleRepeat(Texture2D texture, float u, float v)
	{
		return texture.GetPixelBilinear(u % 1.0f, v % 1.0f);
	}
}
