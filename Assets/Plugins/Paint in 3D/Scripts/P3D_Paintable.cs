using UnityEngine;
using System.Collections.Generic;

public class P3D_Paintable : MonoBehaviour
{
	// All active and enabled paintables in the scene
	public static List<P3D_Paintable> AllPaintables = new List<P3D_Paintable>();
	
	[Tooltip("The submesh in the attached renderer we want to paint to")]
	public int SubMeshIndex;
	
	[Tooltip("The amount of seconds it takes for the mesh data to be updated (useful for animated meshes). -1 = No updates")]
	public float UpdateInterval = -1.0f;

	[Tooltip("The amount of seconds it takes for texture modifications to get applied")]
	public float ApplyInterval = 0.01f;
	
	[Tooltip("All the textures this paintable is associated with")]
	public List<P3D_PaintableTexture> Textures;

	//[SerializeField]
	private P3D_Tree tree;

	private Mesh bakedMesh;

	private float updateCooldown;

	private float applyCooldown;

	public bool IsReady
	{
		get
		{
			if (tree != null && tree.IsReady == true)
			{
				return true;
			}
			
			return false;
		}
	}
	
	// This will paint the nearest surface to position within maxDistance in world space
	public static void ScenePaintNearest(P3D_Brush brush, Vector3 position, float maxDistance, int layerMask = -1, int groupMask = -1)
	{
		var nearestPaintable = default(P3D_Paintable);
		var nearestResult    = default(P3D_Result);
		
		for (var i = AllPaintables.Count - 1; i >= 0; i--)
		{
			var paintable = AllPaintables[i];

			if (P3D_Helper.IndexInMask(paintable.gameObject.layer, layerMask) == true)
			{
				var tree = paintable.GetTree();
				
				if (tree != null)
				{
					var transform    = paintable.transform;
					var uniformScale = P3D_Helper.GetUniformScale(transform);

					if (uniformScale != 0.0f)
					{
						var point  = transform.InverseTransformPoint(position);
						var result = tree.FindNearest(point, maxDistance);

						if (result != null)
						{
							nearestPaintable = paintable;
							nearestResult    = result;

							maxDistance *= result.Distance01;
						}
					}
				}
			}
		}

		// Paint something?
		if (nearestPaintable != null)
		{
			nearestPaintable.Paint(brush, nearestResult, groupMask);
		}
	}

	// This will paint the nearest surface between the start and end positions in world space, unless something is blocking it
	public static void ScenePaintBetweenNearestRaycast(P3D_Brush brush, Vector3 startPosition, Vector3 endPosition, int layerMask = -1, int groupMask = -1)
	{
		var maxDistance      = Vector3.Distance(startPosition, endPosition); if (maxDistance == 0.0f) return;
		var nearestPaintable = default(P3D_Paintable);
		var nearestHit       = default(RaycastHit);
		var nearestResult    = default(P3D_Result);
		
		// Raycast scene to see if we hit a paintable	
		if (Physics.Raycast(startPosition, endPosition - startPosition, out nearestHit, maxDistance, layerMask) == true)
		{
			nearestPaintable = nearestHit.collider.GetComponent<P3D_Paintable>();
			maxDistance      = nearestHit.distance;
		}

		// See if any paintables are closer (this happens if they have no collider, e.g. skinned meshes)
		for (var i = AllPaintables.Count - 1; i >= 0; i--)
		{
			var paintable = AllPaintables[i];

			if (P3D_Helper.IndexInMask(paintable.gameObject.layer, layerMask) == true)
			{
				var tree = paintable.GetTree();
				
				if (tree != null)
				{
					var transform = paintable.transform;
					var start     = transform.InverseTransformPoint(startPosition);
					var end       = transform.InverseTransformPoint(endPosition);
					var direction = (end - start).normalized;
					var result    = tree.FindBetweenNearest(start, start + direction * maxDistance);

					if (result != null)
					{
						nearestPaintable = paintable;
						nearestResult    = result;

						maxDistance *= result.Distance01;
					}
				}
			}
		}

		// Paint something?
		if (nearestPaintable != null)
		{
			if (nearestResult != null)
			{
				nearestPaintable.Paint(brush, nearestResult, groupMask);
			}
			else
			{
				nearestPaintable.Paint(brush, nearestHit, groupMask);
			}
		}
    }

	public static void ScenePaintBetweenNearest(P3D_Brush brush, Vector3 startPosition, Vector3 endPosition, int layerMask = -1, int groupMask = -1)
	{
		var maxDistance      = Vector3.Distance(startPosition, endPosition); if (maxDistance == 0.0f) return;
		var nearestPaintable = default(P3D_Paintable);
		var nearestResult    = default(P3D_Result);
		
		// See if any paintables are closer (this happens if they have no collider, e.g. skinned meshes)
		for (var i = AllPaintables.Count - 1; i >= 0; i--)
		{
			var paintable = AllPaintables[i];

			if (P3D_Helper.IndexInMask(paintable.gameObject.layer, layerMask) == true)
			{
				var tree = paintable.GetTree();
				
				if (tree != null)
				{
					var transform = paintable.transform;
					var start     = transform.InverseTransformPoint(startPosition);
					var end       = transform.InverseTransformPoint(endPosition);
					var direction = (end - start).normalized;
					var result    = tree.FindBetweenNearest(start, start + direction * maxDistance);

					if (result != null)
					{
						nearestPaintable = paintable;
						nearestResult    = result;

						maxDistance *= result.Distance01;
					}
				}
			}
		}

		// Paint something?
		if (nearestPaintable != null && nearestResult != null)
		{
			nearestPaintable.Paint(brush, nearestResult, groupMask);
		}
    }

	// This will paint all surfaces between the start and end positions in world space
	public static void ScenePaintBetweenAll(P3D_Brush brush, Vector3 startPosition, Vector3 endPosition, int layerMask = -1, int groupMask = -1)
	{
		for (var i = AllPaintables.Count - 1; i >= 0; i--)
		{
			var paintable = AllPaintables[i];

			if (P3D_Helper.IndexInMask(paintable.gameObject.layer, layerMask) == true)
			{
				paintable.PaintBetweenAll(brush, startPosition, endPosition, groupMask);
			}
		}
	}

	// This will paint the nearest surface that is perpendicular to position within maxDistance in world space
	public static void ScenePaintPerpedicularNearest(P3D_Brush brush, Vector3 position, float maxDistance, int layerMask = -1, int groupMask = -1)
	{
		var nearestPaintable = default(P3D_Paintable);
		var nearestResult    = default(P3D_Result);
		
		// See if any paintables are closer (this happens if they have no collider, e.g. skinned meshes)
		for (var i = AllPaintables.Count - 1; i >= 0; i--)
		{
			var paintable = AllPaintables[i];

			if (P3D_Helper.IndexInMask(paintable.gameObject.layer, layerMask) == true)
			{
				var tree = paintable.GetTree();
				
				if (tree != null)
				{
					var transform    = paintable.transform;
					var uniformScale = P3D_Helper.GetUniformScale(transform);

					if (uniformScale != 0.0f)
					{
						var point  = transform.InverseTransformPoint(position);
						var result = tree.FindPerpendicularNearest(point, maxDistance);

						if (result != null)
						{
							nearestPaintable = paintable;
							nearestResult    = result;

							maxDistance *= result.Distance01;
						}
					}
				}
			}
		}

		// Paint something?
		if (nearestPaintable != null)
		{
			nearestPaintable.Paint(brush, nearestResult, groupMask);
		}
    }

	// This will paint the nearest surface that is perpendicular to the position within maxDistance in world space
	public static void ScenePaintPerpedicularAll(P3D_Brush brush, Vector3 position, float maxDistance, int layerMask = -1, int groupMask = -1)
	{
		for (var i = AllPaintables.Count - 1; i >= 0; i--)
		{
			var paintable = AllPaintables[i];

			if (P3D_Helper.IndexInMask(paintable.gameObject.layer, layerMask) == true)
			{
				paintable.PaintPerpendicularAll(brush, position, maxDistance, groupMask);
			}
		}
    }

	// This will paint all surfaces perpendicular to the position within maxDistance in world space
	public void PaintPerpendicularNearest(P3D_Brush brush, Vector3 position, float maxDistance, int groupMask = -1)
	{
		if (CheckTree() == true)
		{
			var uniformScale = P3D_Helper.GetUniformScale(transform);

			if (uniformScale != 0.0f)
			{
				var point   = transform.InverseTransformPoint(position);
				var results = tree.FindPerpendicularNearest(point, maxDistance / uniformScale);

				Paint(brush, results, groupMask);
			}
		}
	}

	// This will paint the nearest surface perpendicular to the position within maxDistance in world space
	public void PaintPerpendicularAll(P3D_Brush brush, Vector3 position, float maxDistance, int groupMask = -1)
	{
		if (CheckTree() == true)
		{
			var uniformScale = P3D_Helper.GetUniformScale(transform);

			if (uniformScale != 0.0f)
			{
				var point   = transform.InverseTransformPoint(position);
				var results = tree.FindPerpendicularAll(point, maxDistance / uniformScale);
				
				Paint(brush, results, groupMask);
			}
		}
	}

	// This will paint the nearest surface to the position within maxDistance in world space
	public void PaintNearest(P3D_Brush brush, Vector3 position, float maxDistance, int groupMask = -1)
	{
		if (CheckTree() == true)
		{
			var uniformScale = P3D_Helper.GetUniformScale(transform);

			if (uniformScale != 0.0f)
			{
				var point   = transform.InverseTransformPoint(position);
				var results = tree.FindNearest(point, maxDistance / uniformScale);

				Paint(brush, results, groupMask);
			}
		}
	}

	// This will paint the first surface between the start and end positions in world space
	public void PaintBetweenNearest(P3D_Brush brush, Vector3 startPosition, Vector3 endPosition, int groupMask = -1)
	{
		if (CheckTree() == true)
		{
			var start   = transform.InverseTransformPoint(startPosition);
			var end     = transform.InverseTransformPoint(endPosition);
			var results = tree.FindBetweenNearest(start, end);

			Paint(brush, results, groupMask);
		}
	}
	
	// This will paint all surfaces between the start and end positions in world space
	public void PaintBetweenAll(P3D_Brush brush, Vector3 startPosition, Vector3 endPosition, int groupMask = -1)
	{
		if (CheckTree() == true)
		{
			var start   = transform.InverseTransformPoint(startPosition);
			var end     = transform.InverseTransformPoint(endPosition);
			var results = tree.FindBetweenAll(start, end);

			Paint(brush, results, groupMask);
		}
	}

	// This will paint the current paintable at all the results with the specified coord
	public void Paint(P3D_Brush brush, List<P3D_Result> results, int groupMask = -1)
	{
		if (results != null)
		{
			for (var i = 0; i < results.Count; i++)
			{
				Paint(brush, results[i], groupMask);
			}
		}
	}

	// This will paint the current paintable at the result with the specified coord
	public void Paint(P3D_Brush brush, P3D_Result result, int groupMask = -1)
	{
		if (result != null && Textures != null)
		{
			for (var i = Textures.Count - 1; i >= 0; i--)
			{
				var texture = Textures[i];

				if (texture != null && P3D_Helper.IndexInMask(texture.Group, groupMask))
				{
					texture.Paint(brush, result.GetUV(texture.Coord));
				}
			}
		}
	}
	
	// This will paint the current paintable at the raycast hit with the specified coord
	public void Paint(P3D_Brush brush, RaycastHit hit, int groupMask = -1)
	{
		if (Textures != null)
		{
			for (var i = Textures.Count - 1; i >= 0; i--)
			{
				var texture = Textures[i];

				if (texture != null && P3D_Helper.IndexInMask(texture.Group, groupMask))
				{
					texture.Paint(brush, P3D_Helper.GetUV(hit, texture.Coord));
				}
			}
		}
	}

	// This will paint the current paintable at the specified UV
	public void Paint(P3D_Brush brush, Vector2 uv, int groupMask = -1)
	{
		if (Textures != null)
		{
			for (var i = Textures.Count - 1; i >= 0; i--)
			{
				var texture = Textures[i];

				if (texture != null && P3D_Helper.IndexInMask(texture.Group, groupMask))
				{
					texture.Paint(brush, uv);
				}
			}
		}
	}

	// This will get the tree associated with this paintable, and update it if it needs to be
	public P3D_Tree GetTree()
	{
		if (tree != null)
		{
			if (UpdateInterval >= 0.0f && updateCooldown < 0.0f)
			{
				updateCooldown = UpdateInterval;

				UpdateTree();
			}
		}

		return tree;
	}

	[ContextMenu("Add Texture")]
	public void AddTexture()
	{
		var texture = new P3D_PaintableTexture();

		if (Textures == null)
		{
			Textures = new List<P3D_PaintableTexture>();
		}

		Textures.Add(texture);
	}

	[ContextMenu("Update Tree")]
	public void UpdateTree()
	{
		var forceUpdate = false;
		var mesh        = P3D_Helper.GetMesh(gameObject, ref bakedMesh);

		if (bakedMesh != null)
		{
			forceUpdate = true;
		}

		if (tree == null)
		{
			tree = new P3D_Tree();
		}

		tree.SetMesh(mesh, SubMeshIndex, forceUpdate);
	}
	
	protected virtual void Awake()
	{
		if (Textures != null)
		{
			for (var i = Textures.Count - 1; i >= 0; i--)
			{
				var texture = Textures[i];

				if (texture != null)
				{
					texture.Awake(gameObject);
				}
			}
		}

		UpdateTree();
	}

	protected virtual void OnEnable()
	{
		AllPaintables.Add(this);
	}

#if UNITY_EDITOR
	protected virtual void OnValidate()
	{
		if (Textures == null)
		{
			Textures = new List<P3D_PaintableTexture>();
		}

		if (Textures.Count == 0)
		{
			Textures.Add(new P3D_PaintableTexture());
		}
	}
#endif

	protected virtual void Update()
	{
		applyCooldown -= Time.deltaTime;

		if (applyCooldown <= 0.0f && Textures != null)
		{
			applyCooldown = ApplyInterval;

			for (var i = Textures.Count - 1; i >= 0; i--)
			{
				var texture = Textures[i];

				if (texture != null && texture.Painter != null)
				{
					texture.Painter.Apply();
				}
			}
		}

		updateCooldown -= Time.deltaTime;
	}

	protected virtual void OnDisable()
	{
		AllPaintables.Remove(this);
	}

	private bool CheckTree()
	{
		if (tree != null)
		{
			if (UpdateInterval >= 0.0f && updateCooldown < 0.0f)
			{
				updateCooldown = UpdateInterval;

				UpdateTree();
			}

			return true;
		}

		return false;
	}
}
