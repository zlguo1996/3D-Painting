using UnityEngine;
using System.Collections.Generic;

public static partial class P3D_Helper
{
	public const string ComponentMenuPrefix = "Paint in 3D/P3D ";
	
	private static Material clearMaterial;
	
	public static TextureFormat GetTextureFormat(P3D_Format format)
	{
		switch (format)
		{
			case P3D_Format.TruecolorRGBA: return TextureFormat.RGBA32;
			case P3D_Format.TruecolorRGB:  return TextureFormat.RGB24;
			case P3D_Format.TruecolorA:    return TextureFormat.Alpha8;
		}

		return default(TextureFormat);
	}

	public static bool IndexInMask(int index, LayerMask mask)
	{
		mask &= 1 << index;

		return mask != 0;
	}
	public static Texture2D CreateTexture(int width, int height, TextureFormat format, bool mipMaps)
	{
		if (width > 0 && height > 0)
		{
			return new Texture2D(width, height, format, mipMaps);
        }

		return null;
	}

	public static void ClearTexture(Texture2D texture2D, Color color, bool apply = true)
	{
		if (texture2D != null)
		{
#if UNITY_EDITOR
			P3D_Helper.MakeTextureReadable(texture2D);
#endif
			for (var y = texture2D.height - 1; y >= 0; y--)
			{
				for (var x = texture2D.width - 1; x >= 0; x--)
				{
					texture2D.SetPixel(x, y, color);
				}
			}

			if (apply == true)
			{
				texture2D.Apply();
            }
		}
	}

	// This returns a completely transparent material
	public static Material ClearMaterial
	{
		get
		{
			if (clearMaterial == null)
			{
				clearMaterial = new Material(Shader.Find("Transparent/Diffuse"));
				clearMaterial.color = Color.clear;
			}

			return clearMaterial;
		}
	}

	// This method allows you to easily find a Mesh attached to a GameObject
	public static Mesh GetMesh(GameObject gameObject, ref Mesh bakedMesh)
	{
		var mesh = default(Mesh);

		if (gameObject != null)
		{
			// Is this GO using a MeshFilter/MeshRenderer?
			var meshFilter = gameObject.GetComponent<MeshFilter>();

			if (meshFilter != null)
			{
				mesh = meshFilter.sharedMesh;
			}
			else
			{
				// Is this GO using a SkinnedMeshRenderer?
				var skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();

				if (skinnedMeshRenderer != null)
				{
					mesh = skinnedMeshRenderer.sharedMesh;

					if (mesh != null)
					{
						if (bakedMesh == null)
						{
							bakedMesh = new Mesh();

							bakedMesh.name = "Baked Mesh";
						}

						//var oldScale = gameObject.transform.localScale;

						skinnedMeshRenderer.BakeMesh(bakedMesh);

						return bakedMesh;
					}
				}
			}
		}

		DestroyMesh(ref bakedMesh);

		return mesh;
	}

	private static void DestroyMesh(ref Mesh mesh)
	{
		if (mesh != null)
		{
			Destroy(mesh);

			mesh = null;
		}
	}

	// This method allows you to easily find a Material attached to a GameObject
	public static Material GetMaterial(GameObject gameObject, int materialIndex = 0)
	{
		if (gameObject != null && materialIndex >= 0)
		{
			var renderer = gameObject.GetComponent<Renderer>();

			if (renderer != null)
			{
				var materials = renderer.sharedMaterials;

				if (materialIndex < materials.Length)
				{
					return materials[materialIndex];
				}
			}
		}

		return null;
	}

	// This method allows you to easily duplicate a Material attached to a GameObject
	public static Material CloneMaterial(GameObject gameObject, int materialIndex = 0)
	{
		if (gameObject != null && materialIndex >= 0)
		{
			var renderer = gameObject.GetComponent<Renderer>();

			if (renderer != null)
			{
				var materials = renderer.sharedMaterials;

				if (materialIndex < materials.Length)
				{
					// Get existing material
					var material = materials[materialIndex];

					// Clone it
					material = Clone(material);

					// Update array
					materials[materialIndex] = material;

					// Update materials
					renderer.sharedMaterials = materials;

					return material;
				}
			}
		}

		return null;
	}

	// This method allows you to add a material (layer) to a renderer at the specified material index, or -1 for the end (top)
	public static Material AddMaterial(Renderer renderer, Shader shader, int materialIndex = -1)
	{
		if (renderer != null)
		{
			var newMaterials = new List<Material>(renderer.sharedMaterials);
			var newMaterial  = new Material(shader);

			if (materialIndex <= 0)
			{
				materialIndex = newMaterials.Count;
			}

			newMaterials.Insert(materialIndex, newMaterial);

			renderer.sharedMaterials = newMaterials.ToArray();

			return newMaterial;
		}

		return null;
	}

	// This allows you to split a rect in half
	public static Rect SplitHorizontal(ref Rect rect, int separation)
	{
		var lRect = rect; lRect.xMax -= rect.width / 2 + separation;
		var rRect = rect; rRect.xMin += rect.width / 2 + separation;

		rect = lRect;
		return rRect;
	}

	// This allows you to split a rect in half
	public static Rect SplitVertical(ref Rect rect, int separation)
	{
		var tRect = rect; tRect.yMax -= rect.height / 2 + separation;
		var bRect = rect; bRect.yMin += rect.height / 2 + separation;

		rect = tRect;
		return bRect;
	}

	public static bool Zero(float v)
	{
		return v == 0.0f;
	}

	public static float Divide(float a, float b)
	{
		return Zero(b) == false ? a / b : 0.0f;
	}

	public static float Reciprocal(float a)
	{
		return Zero(a) == false ? 1.0f / a : 0.0f;
	}

	public static float GetUniformScale(Transform transform)
	{
		var scale = transform.lossyScale;

		return (scale.x + scale.y + scale.z) / 3.0f;
	}

	public static Vector2 GetUV(RaycastHit hit, P3D_CoordType coord)
	{
		switch (coord)
		{
			case P3D_CoordType.UV1: return hit.textureCoord;
			case P3D_CoordType.UV2: return hit.textureCoord2;
		}

		return default(Vector2);
	}

	public static float DampenFactor(float dampening, float elapsed)
	{
		return 1.0f - Mathf.Pow((float)System.Math.E, -dampening * elapsed);
	}

	// This converts a 0..1 UV coordinate to a pixel coordination for the current texture
	public static Vector2 CalculatePixelFromCoord(Vector2 uv, Vector2 tiling, Vector2 offset, int width, int height)
	{
		uv.x = Mathf.Repeat(uv.x * tiling.x + offset.x, 1.0f);
		uv.y = Mathf.Repeat(uv.y * tiling.y + offset.y, 1.0f);
		
		uv.x = Mathf.Clamp(Mathf.RoundToInt(uv.x * width ), 0, width  - 1);
		uv.y = Mathf.Clamp(Mathf.RoundToInt(uv.y * height), 0, height - 1);

		return uv;
	}

	public static P3D_Matrix CreateMatrix(Vector2 position, Vector2 size, float angle)
	{
		var t = P3D_Matrix.Translation(position.x, position.y);
		var r = P3D_Matrix.Rotation(angle);
		var o = P3D_Matrix.Translation(size.x * -0.5f, size.y * -0.5f);
		var s = P3D_Matrix.Scaling(size.x, size.y);

		return t * r * o * s;
	}

	public static float Dampen(float current, float target, float dampening, float elapsed, float minStep = 0.0f)
	{
		var factor   = DampenFactor(dampening, elapsed);
		var maxDelta = Mathf.Abs(target - current) * factor + minStep * elapsed;

		return Mathf.MoveTowards(current, target, maxDelta);
	}

	public static Vector3 Dampen3(Vector3 current, Vector3 target, float dampening, float elapsed, float minStep = 0.0f)
	{
		var factor   = DampenFactor(dampening, elapsed);
		var maxDelta = (target - current).magnitude * factor + minStep * elapsed;

		return Vector3.MoveTowards(current, target, maxDelta);
	}

	// This allows you to destroy a UnityEngine.Object in edit or play mode
	public static T Destroy<T>(T o)
		where T : Object
	{
#if UNITY_EDITOR
		if (Application.isPlaying == false)
		{
			Object.DestroyImmediate(o, true); return null;
		}
#endif

		Object.Destroy(o);

		return null;
	}

	// This will intersect a triangle against a line segment, and return the barycentric weights of the hit point
	public static bool IntersectBarycentric(Vector3 start, Vector3 end, P3D_Triangle triangle, out Vector3 weights, out float distance01)
	{
		weights    = default(Vector3);
		distance01 = 0.0f;

		var e1  = triangle.Edge1;
		var e2  = triangle.Edge2;
		var d   = end - start;
		var p   = Vector3.Cross(d, e2);
		var det = Vector3.Dot(e1, p);

		if (Mathf.Abs(det) < float.Epsilon) return false;

		var detRecip = 1.0f / det;
		var t        = start - triangle.PointA;

		weights.x = Vector3.Dot(t, p) * detRecip;

		if (weights.x < -float.Epsilon || weights.x > 1.0f + float.Epsilon) return false;

		var q = Vector3.Cross(t, e1);

		weights.y = Vector3.Dot(d, q) * detRecip;

		var xy = weights.x + weights.y;

		if (weights.y < -float.Epsilon || xy > 1.0f + float.Epsilon) return false;

		weights    = new Vector3(1.0f - xy, weights.x, weights.y);
		distance01 = Vector3.Dot(e2, q) * detRecip;

		return distance01 >= 0.0f && distance01 <= 1.0f;
	}

	// This will compare a triangle against a point, and return the barycentric weights of the closest point
	public static float ClosestBarycentric(Vector3 point, P3D_Triangle triangle, out Vector3 weights)
	{
		// Project point to triangle space
		var a  = triangle.PointA;
		var b  = triangle.PointB;
		var c  = triangle.PointC;
		var r  = Quaternion.Inverse(Quaternion.LookRotation(-Vector3.Cross(a - b, a - c)));
		var ra = r * a;
		var rb = r * b;
		var rc = r * c;
		var rp = r * point;

		// Calculate weights to line segments?
		if (PointLeftOfLine(ra, rb, rp) == true)
		{
			var weight = ClosestBarycentric(rp, ra, rb); weights = new Vector3(1.0f - weight, weight, 0.0f);
		}
		else if (PointLeftOfLine(rb, rc, rp) == true)
		{
			var weight = ClosestBarycentric(rp, rb, rc); weights = new Vector3(0.0f, 1.0f - weight, weight);
		}
		else if (PointLeftOfLine(rc, ra, rp) == true)
		{
			var weight = ClosestBarycentric(rp, rc, ra); weights = new Vector3(weight, 0.0f, 1.0f - weight);
		}
		// Calculate weight to triangle?
		else
		{
			var v0    = rb - ra;
			var v1    = rc - ra;
			var v2    = rp - ra;
			var d00   = Vector2.Dot(v0, v0);
			var d01   = Vector2.Dot(v0, v1);
			var d11   = Vector2.Dot(v1, v1);
			var d20   = Vector2.Dot(v2, v0);
			var d21   = Vector2.Dot(v2, v1);
			var denom = P3D_Helper.Reciprocal(d00 * d11 - d01 * d01);

			weights.y = (d11 * d20 - d01 * d21) * denom;
			weights.z = (d00 * d21 - d01 * d20) * denom;
			weights.x = 1.0f - weights.y - weights.z;
		}

		// Return square distance from point to closest point
		var closest = weights.x * a + weights.y * b + weights.z * c;

		return (point - closest).sqrMagnitude;
    }

	public static bool ClosestBarycentric(Vector3 point, P3D_Triangle triangle, ref Vector3 weights, ref float distanceSqr)
	{
		// Project point to triangle space
		var a  = triangle.PointA;
		var b  = triangle.PointB;
		var c  = triangle.PointC;
		var r  = Quaternion.Inverse(Quaternion.LookRotation(-Vector3.Cross(a - b, a - c)));
		var ra = r * a;
		var rb = r * b;
		var rc = r * c;
		var rp = r * point;

		// Point is over triangle face?
		if (PointRightOfLine(ra, rb, rp) == true && PointRightOfLine(rb, rc, rp) == true && PointRightOfLine(rc, ra, rp) == true)
		{
			var v0    = rb - ra;
			var v1    = rc - ra;
			var v2    = rp - ra;
			var d00   = Vector2.Dot(v0, v0);
			var d01   = Vector2.Dot(v0, v1);
			var d11   = Vector2.Dot(v1, v1);
			var d20   = Vector2.Dot(v2, v0);
			var d21   = Vector2.Dot(v2, v1);
			var denom = P3D_Helper.Reciprocal(d00 * d11 - d01 * d01);

			weights.y = (d11 * d20 - d01 * d21) * denom;
			weights.z = (d00 * d21 - d01 * d20) * denom;
			weights.x = 1.0f - weights.y - weights.z;

			var closest = weights.x * a + weights.y * b + weights.z * c;

			distanceSqr = (point - closest).sqrMagnitude;

			return true;
		}

		return false;
	}

	public static float ClosestBarycentric(Vector2 point, Vector2 start, Vector2 end)
	{
		var v = end - start;
		var m = v.sqrMagnitude;

		if (m > 0.0f)
		{
			return Mathf.Clamp01(Vector2.Dot(point - start, v / m));
        }

		return 0.5f;
	}

	public static bool PointLeftOfLine(Vector2 a, Vector2 b, Vector2 p) // NOTE: CCW
	{
		return ((b.x - a.x) * (p.y - a.y) - (p.x - a.x) * (b.y - a.y)) >= 0.0f;
	}

	public static bool PointRightOfLine(Vector2 a, Vector2 b, Vector2 p) // NOTE: CCW
	{
		return ((b.x - a.x) * (p.y - a.y) - (p.x - a.x) * (b.y - a.y)) <= 0.0f;
	}

	// This allows you to clone any UnityEngine.Object
	public static T Clone<T>(T o, bool keepName = true)
		where T : Object
	{
		if (o != null)
		{
			var c = (T)Object.Instantiate(o);

			if (c != null && keepName == true) c.name = o.name;

			return c;
		}

		return null;
	}

	// This tells you if the TextureFormat is potentially writable if it's in a Texture2D
	public static bool IsWritableFormat(TextureFormat format)
	{
		switch (format)
		{
			case TextureFormat.RGBA32: return true;
			case TextureFormat.ARGB32: return true;
			case TextureFormat.BGRA32: return true;
			case TextureFormat.RGB24:  return true;
			case TextureFormat.Alpha8: return true;
		}

		return false;
	}
}
