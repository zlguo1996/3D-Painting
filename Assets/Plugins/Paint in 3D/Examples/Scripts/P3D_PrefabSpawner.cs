using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_PrefabSpawner))]
public class P3D_PrefabSpawner_Editor : P3D_Editor<P3D_PrefabSpawner>
{
	protected override void OnInspector()
	{
		DrawDefault("BrushMask");
		DrawDefault("Prefab");
	}
}
#endif

// This script allows you to spawn prefabs using the UI or similar
public class P3D_PrefabSpawner : MonoBehaviour
{
	// This layer mask used when raycasting into the scene
	public LayerMask BrushMask = -1;
	
	public GameObject Prefab;
	
	// This method will clone the prefab, and duplicate its material and main texture
	public void SpawnPrefabClone()
	{
		// Does the prefab exist?
		if (Prefab != null)
		{
			var pos = Random.insideUnitCircle * 2.0f;
			var rot = Quaternion.identity;
			
			// Make a clone of it
			// NOTE: This prefab contains the P3D_Paintable component, which automatically duplicates the material and texture used for painting
			Instantiate(Prefab, pos, rot);
		}
	}
}