using UnityEngine;

public partial class P3D_Brush
{
	private static class AdditiveBlend
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
						var add = color;

						if (shape != null) add *= shape.GetPixelBilinear(shapeCoord.x, shapeCoord.y);

						if (detail != null) add *= SampleRepeat(detail, detailX * x, detailY * y);
								
						canvas.SetPixel(x, y, Blend(old, add));
					}
				}
			}
		}

		private static Color Blend(Color old, Color add)
		{
			old.r += add.r;
			old.g += add.g;
			old.b += add.b;
			old.a += add.a;
		
			return old;
		}
	}
}