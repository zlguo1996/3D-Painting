using UnityEngine;
using System.Collections.Generic;

// This class converts a mesh into a tree, and provides various methods for finding UVs
//[System.Serializable]
public class P3D_Tree
{
	// The current mesh we're painting using
	[SerializeField]
	private Mesh mesh;

	// The current mesh we're painting using
	[SerializeField]
	private int vertexCount;

	// The current sub mesh we're painting using
	[SerializeField]
	private int subMeshIndex;

	// All the nodes of the tree, contain information about which triangles are inside
	[SerializeField]
	private List<P3D_Node> nodes = new List<P3D_Node>();

	// All the triangles extracted from the mesh
	[SerializeField]
	private List<P3D_Triangle> triangles = new List<P3D_Triangle>();

	// After performaning a search, this stores the triangles that are potentially valid
	private static List<P3D_Triangle> potentials = new List<P3D_Triangle>();

	// After performing a search, this stores the triangles and weights that were found
	private static List<P3D_Result> results = new List<P3D_Result>();

	private static P3D_Tree tempInstance;

	public static P3D_Tree TempInstance
	{
		get
		{
			if (tempInstance == null)
			{
				tempInstance = new P3D_Tree();
			}

			return tempInstance;
		}
	}

	public bool IsReady
	{
		get
		{
			return nodes.Count > 0;
		}
	}

	// This allows you to reset this class
	public void Clear()
	{
		mesh         = null;
		vertexCount  = 0;
		subMeshIndex = 0;

		for (var i = triangles.Count - 1; i >= 0; i--)
		{
			P3D_Triangle.Despawn(triangles[i]);
		}

		triangles.Clear();

		for (var i = nodes.Count - 1; i >= 0; i--)
		{
			P3D_Node.Despawn(nodes[i]);
		}

		nodes.Clear();
	}

	public void ClearResults()
	{
		for (var i = results.Count - 1; i >= 0; i--)
		{
			P3D_Result.Despawn(results[i]);
		}

		results.Clear();

		potentials.Clear();
	}

	// This allows you to change which mesh is currently used when doing painting
	// NOTE: If you're using MeshCollider raycasting then you don't need to call this, as you can pass the hit UV coordinates directly to the Paint method
	// NOTE: The material index is passed here because it alters the submesh used by the mesh
	public void SetMesh(Mesh newMesh, int newSubMeshIndex = 0, bool forceUpdate = false)
	{
		if (newMesh != null)
		{
			// Doesn't need updating?
			if (forceUpdate == false && newMesh == mesh && newSubMeshIndex == subMeshIndex && newMesh.vertexCount == vertexCount)
			{
				return;
			}

			Clear();

			mesh         = newMesh;
			subMeshIndex = newSubMeshIndex;
			vertexCount  = newMesh.vertexCount;

			ExtractTriangles();

			ConstructNodes();
		}
		else
		{
			Clear();
		}
	}

	// This will automatically finds the mesh from a MeshFilter and sets it
	// NOTE: This method will not work if the passed GameObject is a SkinnedMeshRenderer
	public void SetMesh(GameObject gameObject, int subMeshIndex = 0, bool forceUpdate = false)
	{
		var bakedMesh = default(Mesh);
		var newMesh   = P3D_Helper.GetMesh(gameObject, ref bakedMesh);

		if (bakedMesh != null)
		{
			P3D_Helper.Destroy(bakedMesh);

			throw new System.Exception("P3D_Tree cannot manage baked meshes, call SetMesh with the Mesh directly to use animated meshes");
		}

		SetMesh(newMesh, subMeshIndex, forceUpdate);
	}

	// This finds the nearest triangles to the input position within maxDistance
	public P3D_Result FindNearest(Vector3 point, float maxDistance)
	{
		ClearResults();

		if (IsReady == true && maxDistance > 0.0f)
		{
			var bestDistanceSqr = maxDistance * maxDistance;
			var bestTriangle    = default(P3D_Triangle);
			var bestWeights     = default(Vector3);

			BeginSearchDistance(point, bestDistanceSqr);

			// Go through all potential triangles
			for (var i = potentials.Count - 1; i >= 0; i--)
			{
				var triangle    = potentials[i];
				var weights     = default(Vector3);
				var distanceSqr = P3D_Helper.ClosestBarycentric(point, triangle, out weights);

				if (distanceSqr <= bestDistanceSqr)
				{
					bestDistanceSqr = distanceSqr;
					bestTriangle    = triangle;
					bestWeights     = weights;
				}
			}

			// Add the best triangle to the results
			if (bestTriangle != null)
			{
				return GetResult(bestTriangle, bestWeights, Mathf.Sqrt(bestDistanceSqr) / maxDistance);
			}
		}

		return null;
	}

	// This finds the nearest triangles between the input positions
	public P3D_Result FindBetweenNearest(Vector3 startPoint, Vector3 endPoint)
	{
		ClearResults();

		if (IsReady == true)
		{
			var bestDistance01 = float.PositiveInfinity;
			var bestTriangle   = default(P3D_Triangle);
			var bestWeights    = default(Vector3);

			BeginSearchBetween(startPoint, endPoint);

			// Go through all potential triangles
			for (var i = potentials.Count - 1; i >= 0; i--)
			{
				var triangle   = potentials[i];
				var weights    = default(Vector3);
				var distance01 = default(float);

				// See if this triangle is between the start and end points
				if (P3D_Helper.IntersectBarycentric(startPoint, endPoint, triangle, out weights, out distance01) == true)
				{
					if (distance01 < bestDistance01)
					{
						bestDistance01 = distance01;
						bestTriangle   = triangle;
						bestWeights    = weights;
                    }
				}
			}

			// Add the best triangle to the results
			if (bestTriangle != null)
			{
				return GetResult(bestTriangle, bestWeights, bestDistance01);
			}
		}

		return null;
	}

	// This finds all triangles between the input positions
	public List<P3D_Result> FindBetweenAll(Vector3 startPoint, Vector3 endPoint)
	{
		ClearResults();

		if (IsReady == true)
		{
			BeginSearchBetween(startPoint, endPoint);

			// Go through all potential triangles
			for (var i = potentials.Count - 1; i >= 0; i--)
			{
				var triangle   = potentials[i];
				var weights    = default(Vector3);
				var distance01 = default(float);

				// See if this triangle is between the start and end points
				if (P3D_Helper.IntersectBarycentric(startPoint, endPoint, triangle, out weights, out distance01) == true)
				{
					AddToResults(triangle, weights, distance01);
				}
			}
		}

		return results;
	}

	// This finds the nearest triangle perpendicular to the input position
	public P3D_Result FindPerpendicularNearest(Vector3 point, float maxDistance)
	{
		ClearResults();

		if (IsReady == true && maxDistance > 0.0f)
		{
			var bestDistanceSqr = maxDistance * maxDistance;
			var bestTriangle    = default(P3D_Triangle);
			var bestWeights     = default(Vector3);

			BeginSearchDistance(point, bestDistanceSqr);

			// Go through all potential triangles
			for (var i = potentials.Count - 1; i >= 0; i--)
			{
				var triangle    = potentials[i];
				var weights     = default(Vector3);
				var distanceSqr = default(float);

				// See if a perpendicular triangle point can be found
				if (P3D_Helper.ClosestBarycentric(point, triangle, ref weights, ref distanceSqr) == true)
				{
					if (distanceSqr <= bestDistanceSqr)
					{
						bestDistanceSqr = distanceSqr;
						bestTriangle    = triangle;
						bestWeights     = weights;
					}
				}
			}

			// Add the best triangle to the results
			if (bestTriangle != null)
			{
				return GetResult(bestTriangle, bestWeights, Mathf.Sqrt(bestDistanceSqr) / maxDistance);
			}
		}

		return null;
	}

	// This finds all triangles perpendicular to the input position
	public List<P3D_Result> FindPerpendicularAll(Vector3 point, float maxDistance)
	{
		ClearResults();

		if (IsReady == true && maxDistance > 0.0f)
		{
			var maxDistanceSqr = maxDistance * maxDistance;

			BeginSearchDistance(point, maxDistanceSqr);

			// Go through all potential triangles
			for (var i = potentials.Count - 1; i >= 0; i--)
			{
				var triangle    = potentials[i];
				var weights     = default(Vector3);
				var distanceSqr = default(float);

				// See if a perpendicular triangle point can be found
				if (P3D_Helper.ClosestBarycentric(point, triangle, ref weights, ref distanceSqr) == true)
				{
					if (distanceSqr <= maxDistanceSqr)
					{
						AddToResults(triangle, weights, Mathf.Sqrt(distanceSqr) / maxDistance);
					}
				}
			}
		}

		return results;
	}

	// This finds all nodes within range of the point, and adds thir triangles to the potentials list
	private void BeginSearchDistance(Vector3 point, float maxDistanceSqr)
	{
		SearchDistance(nodes[0], point, maxDistanceSqr);
	}

	private void SearchDistance(P3D_Node node, Vector3 point, float maxDistanceSqr)
	{
		// Is the node bound in range?
		if (node.Bound.SqrDistance(point) < maxDistanceSqr)
		{
			if (node.Split == true)
			{
				if (node.PositiveIndex != 0) SearchDistance(nodes[node.PositiveIndex], point, maxDistanceSqr);
				if (node.NegativeIndex != 0) SearchDistance(nodes[node.NegativeIndex], point, maxDistanceSqr);
			}
			else
			{
				AddToPotentials(node);
			}
		}
	}

	// This finds all nodes between the start and end points, and adds thir triangles to the potentials list
	private void BeginSearchBetween(Vector3 startPoint, Vector3 endPoint)
	{
		var vec      = endPoint - startPoint;
		var ray      = new Ray(startPoint, vec);
		var distance = vec.magnitude;

		SearchBetween(nodes[0], ray, distance);
	}

	private void SearchBetween(P3D_Node node, Ray ray, float maxDistance)
	{
		// Does ray hit the node bound?
		var distance = default(float);

		if (node.Bound.IntersectRay(ray, out distance) == true && distance <= maxDistance)
		{
			if (node.Split == true)
			{
				if (node.PositiveIndex != 0) SearchBetween(nodes[node.PositiveIndex], ray, maxDistance);
				if (node.NegativeIndex != 0) SearchBetween(nodes[node.NegativeIndex], ray, maxDistance);
			}
			else
			{
				AddToPotentials(node);
			}
		}
	}

	// This adds all triangles in this node to the potential triangles list
	private void AddToPotentials(P3D_Node node)
	{
		for (var i = node.TriangleIndex; i < node.TriangleIndex + node.TriangleCount; i++)
		{
			potentials.Add(triangles[i]);
		}
	}

	// This adds a triangle result to the results list
	private void AddToResults(P3D_Triangle triangle, Vector3 weights, float distance01)
	{
		var result = P3D_Result.Spawn();

		result.Triangle   = triangle;
		result.Weights    = weights;
		result.Distance01 = distance01;

		results.Add(result);
	}

	private P3D_Result GetResult(P3D_Triangle triangle, Vector3 weights, float distance01)
	{
		ClearResults();

		AddToResults(triangle, weights, distance01);

		return results[0];
	}

	// This will extract all triangles from the current mesh
	private void ExtractTriangles()
	{
		if (subMeshIndex >= 0 && mesh.subMeshCount >= 0)
		{
			var submeshIndex = Mathf.Min(subMeshIndex, mesh.subMeshCount - 1);
			var indices      = mesh.GetTriangles(submeshIndex);
			var allPositions = mesh.vertices;
			var allCoords1   = mesh.uv;
			var allCoords2   = mesh.uv2;

			if (indices.Length > 0)
			{
				var triangleCount = indices.Length / 3;

				for (var i = triangleCount - 1; i >= 0; i--)
				{
					var triangle = P3D_Triangle.Spawn();
					var a        = indices[i * 3 + 0];
					var b        = indices[i * 3 + 1];
					var c        = indices[i * 3 + 2];

					triangle.PointA = allPositions[a];
					triangle.PointB = allPositions[b];
					triangle.PointC = allPositions[c];

					triangle.Coord1A = allCoords1[a];
					triangle.Coord1B = allCoords1[b];
					triangle.Coord1C = allCoords1[c];

					if (allCoords2.Length > 0)
					{
						triangle.Coord2A = allCoords2[a];
						triangle.Coord2B = allCoords2[b];
						triangle.Coord2C = allCoords2[c];
					}

					triangles.Add(triangle);
				}
			}
		}
	}

	// Sort the triangles list into a node tree
	private void ConstructNodes()
	{
		var rootNode = P3D_Node.Spawn();

		nodes.Add(rootNode);

		Pack(rootNode, 0, triangles.Count);
	}

	// This will pack all remaining triangles into the current node, or create more
	private void Pack(P3D_Node node, int min, int max)
	{
		var count = max - min;

		node.TriangleIndex = min;
		node.TriangleCount = count;
		node.Split = count >= 5;

		node.CalculateBound(triangles);

		// Split this node?
		if (node.Split == true)
		{
			var mid = (min + max) / 2;

			SortTriangles(min, max);

			// Split node along pivot
			node.PositiveIndex = nodes.Count; var positiveNode = P3D_Node.Spawn(); nodes.Add(positiveNode); Pack(positiveNode, min, mid);
			node.NegativeIndex = nodes.Count; var negativeNode = P3D_Node.Spawn(); nodes.Add(negativeNode); Pack(negativeNode, mid, max);
		}
	}

	// This sorts the triangles between min/max according to the axis and pivot calculated based on the bounds
	private void SortTriangles(int minIndex, int maxIndex)
	{
		// Use this array to store the triangles we will stort
		potentials.Clear();

		// Find axis
		var min = triangles[minIndex].Min;
		var max = triangles[minIndex].Max;
		var mid = Vector3.zero;

		for (var i = minIndex; i < maxIndex; i++)
		{
			var triangle = triangles[i];

			min = Vector3.Min(min, triangle.Min);
			max = Vector3.Max(max, triangle.Max);
			mid += triangle.PointA + triangle.PointB + triangle.PointC;

			// Add to sorting list
			potentials.Add(triangle);
		}

		var size = max - min;

		// X
		if (size.x > size.y && size.x > size.z)
		{
			var pivot = P3D_Helper.Divide(mid.x, triangles.Count * 3.0f);

			for (var i = potentials.Count - 1; i >= 0; i--)
			{
				var triangle = potentials[i];

				SortTriangle(triangle, ref minIndex, ref maxIndex, triangle.MidX >= pivot);
			}
		}
		// Y
		else if (size.y > size.x && size.y > size.z)
		{
			var pivot = P3D_Helper.Divide(mid.y, triangles.Count * 3.0f);

			for (var i = potentials.Count - 1; i >= 0; i--)
			{
				var triangle = potentials[i];

				SortTriangle(triangle, ref minIndex, ref maxIndex, triangle.MidY >= pivot);
			}
		}
		// Z
		else
		{
			var pivot = P3D_Helper.Divide(mid.z, triangles.Count * 3.0f);

			for (var i = potentials.Count - 1; i >= 0; i--)
			{
				var triangle = potentials[i];

				SortTriangle(triangle, ref minIndex, ref maxIndex, triangle.MidZ >= pivot);
			}
		}
	}

	private void SortTriangle(P3D_Triangle triangle, ref int minIndex, ref int maxIndex, bool abovePivot)
	{
		if (abovePivot == true)
		{
			triangles[maxIndex - 1] = triangle; maxIndex -= 1;
		}
		else
		{
			triangles[minIndex] = triangle; minIndex += 1;
		}
	}
}
