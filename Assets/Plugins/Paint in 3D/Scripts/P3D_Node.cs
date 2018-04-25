using UnityEngine;
using System.Collections.Generic;

// This class handles a node in the triangle tree
[System.Serializable]
public class P3D_Node
{
	private static List<P3D_Node> pool = new List<P3D_Node>();

	public static P3D_Node Spawn()
	{
		if (pool.Count > 0)
		{
			var index = pool.Count - 1;
			var node  = pool[index];

			pool.RemoveAt(index);

			return node;
		}

		return new P3D_Node();
	}

	public static P3D_Node Despawn(P3D_Node node)
	{
		pool.Add(node);

		node.Bound         = new Bounds();
		node.Split         = false;
        node.PositiveIndex = 0;
		node.NegativeIndex = 0;
		node.TriangleIndex = 0;
		node.TriangleCount = 0;

		return null;
	}

	public Bounds Bound;

	public bool Split;

	public int PositiveIndex;

	public int NegativeIndex;

	public int TriangleIndex;

	public int TriangleCount;

	// This calculates the AABB for the current node
	public void CalculateBound(List<P3D_Triangle> triangles)
	{
		if (triangles.Count > 0 && TriangleCount > 0)
		{
			var min = triangles[TriangleIndex].Min;
			var max = triangles[TriangleIndex].Max;

			for (var i = TriangleIndex + TriangleCount - 1; i > TriangleIndex; i--)
			{
				var triangle = triangles[i];

				min = Vector3.Min(min, triangle.Min);
				max = Vector3.Max(max, triangle.Max);
			}

			Bound.SetMinMax(min, max);
		}
	}
}
