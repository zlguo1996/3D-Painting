using UnityEngine;
using System.Collections.Generic;

// This class handles showing the current brush preview (i.e. the unshaded view).
[ExecuteInEditMode]
public class P3D_BrushPreview : MonoBehaviour
{
	private static List<P3D_BrushPreview> AllPreviews = new List<P3D_BrushPreview>();
	
	private MeshRenderer meshRenderer;
	
	private MeshFilter meshFilter;
	
	private Material material;
	
	private int age;
	
	private Material[] materials = new Material[1];
	
	// Call this if you want to show the brush preview yourself
	public static void Show(Mesh mesh, int submeshIndex, Transform transform, float opacity, P3D_Matrix paintMatrix, Vector2 canvasResolution, Texture2D shape, Vector2 tiling, Vector2 offset)
	{
		for (var i = AllPreviews.Count - 1; i >= 0; i--)
		{
			var preview = AllPreviews[i];
			
			if (preview != null && preview.age > 0)
			{
				preview.UpdateShow(mesh, submeshIndex, transform, opacity, paintMatrix, canvasResolution, shape, tiling, offset); return;
			}
		}
		
		var newGameObject = new GameObject("P3D_BrushPreview");             newGameObject.hideFlags = HideFlags.HideAndDontSave;
		var newPreview    = newGameObject.AddComponent<P3D_BrushPreview>(); newPreview.hideFlags    = HideFlags.HideAndDontSave;
		
		newPreview.UpdateShow(mesh, submeshIndex, transform, opacity, paintMatrix, canvasResolution, shape, tiling, offset);
	}
	
	public static void Mark()
	{
		for (var i = AllPreviews.Count - 1; i >= 0; i--)
		{
			var preview = AllPreviews[i];
			
			if (preview != null)
			{
				preview.age = 5;
			}
		}
	}
	
	public static void Sweep()
	{
		for (var i = AllPreviews.Count - 1; i >= 0; i--)
		{
			var preview = AllPreviews[i];
			
			if (preview != null && preview.age > 1)
			{
				AllPreviews.RemoveAt(i);
				
				P3D_Helper.Destroy(preview.gameObject);
			}
		}
	}
	
	protected virtual void OnEnable()
	{
		AllPreviews.Add(this);
	}
	
	// If this preview hasn't been updated in a while, destroy it
	protected virtual void Update()
	{
		if (age >= 2)
		{
			P3D_Helper.Destroy(gameObject);
		}
		else
		{
			age += 1;
		}
	}

	protected virtual void OnDisable()
	{
		AllPreviews.Remove(this);
	}

	protected virtual void OnDestroy()
	{
		P3D_Helper.Destroy(material);
	}
	
	private void UpdateShow(Mesh mesh, int submeshIndex, Transform target, float opacity, P3D_Matrix paintMatrix, Vector2 canvasResolution, Texture2D shape, Vector2 tiling, Vector2 offset)
	{
		if (target != null)
		{
			if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
			if (meshFilter   == null) meshFilter   = gameObject.AddComponent<MeshFilter>();
			if (material     == null) material     = new Material(Shader.Find("Hidden/P3D_BrushPreview"));
			
			transform.position   = target.position;
			transform.rotation   = target.rotation;
			transform.localScale = target.lossyScale;
			
			material.hideFlags = HideFlags.HideAndDontSave;
			
			material.SetMatrix("_WorldMatrix", target.localToWorldMatrix);
			material.SetMatrix("_PaintMatrix", paintMatrix.Matrix4x4);
			material.SetVector("_CanvasResolution", canvasResolution);
			material.SetVector("_Tiling", tiling);
			material.SetVector("_Offset", offset);
			material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, opacity));
			material.SetTexture("_Shape", shape);
			
			if (materials.Length != submeshIndex + 1)
			{
				materials = new Material[submeshIndex + 1];
			}
			
			for (var i = 0; i < submeshIndex; i++)
			{
				materials[i] = P3D_Helper.ClearMaterial;
			}
			
			materials[submeshIndex] = material;
			
			meshRenderer.sharedMaterials = materials;
			
			meshFilter.sharedMesh = mesh;
			
			age = 0;
		}
	}
}