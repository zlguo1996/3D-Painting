using UnityEngine;

public partial class P3D_Brush
{
	private static class NormalBlend
	{
		public static void Paint()
		{
			var shapeCoord = default(Vector2);
			var detailX    = P3D_Helper.Reciprocal(canvasW * detailScale.x);
			var detailY    = P3D_Helper.Reciprocal(canvasH * detailScale.y);

			color.a *= opacity;

			// Normal shape?
			if (shape != null && shape.format != TextureFormat.Alpha8)
			{
				for (var x = rect.XMin; x < rect.XMax; x++)
				{
					for (var y = rect.YMin; y < rect.YMax; y++)
					{
						if (IsInsideShape(inverse, x, y, ref shapeCoord) == true)
						{
							var vec = ColorToNormalXY(canvas.GetPixel(x, y));
							var add = ColorToNormalXY(shape.GetPixelBilinear(shapeCoord.x, shapeCoord.y));
							
							if (detail != null)
							{
								var det = ColorToNormalXY(SampleRepeat(detail, detailX * x, detailY * y));
								
								add = CombineNormalsXY(add, det);
							}

							vec = CombineNormalsXY(vec, add, opacity);
							vec = ComputeZ(vec);
							vec = Vector3.Normalize(vec);
							
							canvas.SetPixel(x, y, NormalToColor(vec));
						}
					}
				}
			}
			// Alpha shape?
			else
			{
				for (var x = rect.XMin; x < rect.XMax; x++)
				{
					for (var y = rect.YMin; y < rect.YMax; y++)
					{
						if (IsInsideShape(inverse, x, y, ref shapeCoord) == true)
						{
							var vec = ColorToNormalXY(canvas.GetPixel(x, y));
							var add = direction;
							var opa = opacity;

							if (shape != null) opa *= shape.GetPixelBilinear(shapeCoord.x, shapeCoord.y).a;
							
							if (detail != null)
							{
								var det = ColorToNormalXY(SampleRepeat(detail, detailX * x, detailY * y));
								
								add = CombineNormalsXY(add, det);
							}
							
							vec = CombineNormalsXY(vec, add, opa);
							vec = ComputeZ(vec);
							vec = Vector3.Normalize(vec);
							
							canvas.SetPixel(x, y, NormalToColor(vec));
						}
					}
				}
			}
		}

		private static Vector3 ColorToNormalXY(Color c)
		{
			var v = default(Vector3);

			v.x = c.r * 2.0f - 1.0f;
			v.y = c.g * 2.0f - 1.0f;

			return v;
		}

		private static Color NormalToColor(Vector3 n)
		{
			var c = default(Color);

			c.r = n.x * 0.5f + 0.5f;
			c.g = n.y * 0.5f + 0.5f;
			c.b = n.z * 0.5f + 0.5f;
			c.a = c.r;

			return c;
		}

		private static Vector3 ComputeZ(Vector3 a)
		{
			a.z = Mathf.Sqrt(1.0f - a.x * a.x + a.y * a.y);

			return a;
		}

		private static Vector3 CombineNormalsXY(Vector3 a, Vector3 b)
		{
			a.x += b.x;
			a.y += b.y;

			return a;
		}

		private static Vector3 CombineNormalsXY(Vector3 a, Vector3 b, float c)
		{
			a.x += b.x * c;
			a.y += b.y * c;

			return a;
		}

		private static Vector3 CombineNormalsXY(Vector3 a, Vector2 b, float c)
		{
			a.x += b.x * c;
			a.y += b.y * c;

			return a;
		}
	}
}