using UnityEngine;

public partial class P3D_Brush
{
	private static class AlphaBlendRGB
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
			if (old.a > 0.0f && add.a > 0.0f)
			{
				var add_a = add.a;
				var add_i = 1.0f - add_a;
				var old_a = old.a;
				var old_n = add_a + old_a * add_i;
			
				old.r = (add.r * add_a + old.r * old_a * add_i) / old_n;
				old.g = (add.g * add_a + old.g * old_a * add_i) / old_n;
				old.b = (add.b * add_a + old.b * old_a * add_i) / old_n;
			}
		
			return old;
		}
	}
}