using UnityEngine;

public partial class P3D_Brush
{
	private static class AlphaErase
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
						var opa = opacity;

						if (shape != null) opa *= shape.GetPixelBilinear(shapeCoord.x, shapeCoord.y).a;

						if (detail != null) opa *= SampleRepeat(detail, detailX * x, detailY * y).a;
								
						canvas.SetPixel(x, y, Blend(old, opa));
					}
				}
			}
		}

		private static Color Blend(Color old, float sub)
		{
			old.a -= sub;
			
			return old;
		}
	}
}