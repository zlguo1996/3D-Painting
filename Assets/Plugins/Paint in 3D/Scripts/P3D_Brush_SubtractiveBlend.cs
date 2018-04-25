using UnityEngine;

public partial class P3D_Brush
{
	private static class SubtractiveBlend
	{
		public static void Paint()
		{
			var shapeCoord = default(Vector2);
			var detailX    = P3D_Helper.Reciprocal(canvasW * detailScale.x);
			var detailY    = P3D_Helper.Reciprocal(canvasH * detailScale.y);

			color.a *= opacity;

			for (var x = rect.XMin; x < rect.XMax; x++)
			{
				for (var y = rect.YMin; y < rect.YMax; y++)
				{
					if (IsInsideShape(inverse, x, y, ref shapeCoord) == true)
					{
						var old = canvas.GetPixel(x, y);
						var sub = color;

						if (shape != null) sub *= shape.GetPixelBilinear(shapeCoord.x, shapeCoord.y);

						if (detail != null) sub *= SampleRepeat(detail, detailX * x, detailY * y);
								
						canvas.SetPixel(x, y, Blend(old, sub));
					}
				}
			}
		}

		private static Color Blend(Color old, Color sub)
		{
			old.r -= sub.r;
			old.g -= sub.g;
			old.b -= sub.b;
			old.a -= sub.a;
		
			return old;
		}
	}
}