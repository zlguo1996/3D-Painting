using UnityEngine;
using System.Collections.Generic;

// This stores compiled data of a single triangle from a mesh
[System.Serializable]
public class P3D_Triangle
{
	private static List<P3D_Triangle> pool = new List<P3D_Triangle>();

	public static P3D_Triangle Spawn()
	{
		if (pool.Count > 0)
		{
			var index    = pool.Count - 1;
			var triangle = pool[index];

			pool.RemoveAt(index);

			return triangle;
		}

		return new P3D_Triangle();
	}

	public static P3D_Triangle Despawn(P3D_Triangle triangle)
	{
		pool.Add(triangle);

		return null;
	}

	public Vector3 PointA;
	public Vector3 PointB;
	public Vector3 PointC;

	public Vector2 Coord1A;
	public Vector2 Coord1B;
	public Vector2 Coord1C;

	public Vector2 Coord2A;
	public Vector2 Coord2B;
	public Vector2 Coord2C;

	public Vector3 Edge1
	{
		get
		{
			return PointB - PointA;
		}
	}

	public Vector3 Edge2
	{
		get
		{
			return PointC - PointA;
		}
	}

	public Vector3 Min
	{
		get
		{
			return Vector3.Min(PointA, Vector3.Min(PointB, PointC));
		}
	}

	public Vector3 Max
	{
		get
		{
			return Vector3.Max(PointA, Vector3.Max(PointB, PointC));
		}
	}

	public float MidX
	{
		get
		{
			return (PointA.x + PointB.x + PointC.x) / 3.0f;
		}
	}

	public float MidY
	{
		get
		{
			return (PointA.y + PointB.y + PointC.y) / 3.0f;
		}
	}

	public float MidZ
	{
		get
		{
			return (PointA.z + PointB.z + PointC.z) / 3.0f;
		}
	}
}
