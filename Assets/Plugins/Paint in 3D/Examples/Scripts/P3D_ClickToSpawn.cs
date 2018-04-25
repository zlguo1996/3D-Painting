using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_ClickToSpawn))]
public class P3D_ClickToSpawn_Editor : P3D_Editor<P3D_ClickToSpawn>
{
	protected override void OnInspector()
	{
		DrawDefault("Requires");
		DrawDefault("RaycastMask");
		BeginError(Any(t => t.Prefab == null));
			DrawDefault("Prefab");
		EndError();
		DrawDefault("Offset");
	}
}
#endif

// This script allows you to spawn a prefab at the point under the mouse/finger
public class P3D_ClickToSpawn : MonoBehaviour
{
	[Tooltip("The key that must be held down to spawn")]
	public KeyCode Requires = KeyCode.Mouse0;

	[Tooltip("The layer mask used when raycasting into the scene")]
	public LayerMask RaycastMask = -1;

	[Tooltip("The prefab we want to spawn")]
	public GameObject Prefab;
	
	[Tooltip("How far from the hit point the position should be offset")]
	public float Offset = 0.1f;
	
	// Called every frame
	protected virtual void Update()
	{
		var mainCamera = Camera.main;
		
		if (Prefab != null && mainCamera != null)
		{
			// The required key is down?
			if (Input.GetKeyDown(Requires) == true)
			{
				// Find the ray for this screen position
				var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
				var hit = default(RaycastHit);

				// Raycast into the 3D scene
				if (Physics.Raycast(ray, out hit, float.PositiveInfinity, RaycastMask) == true)
				{
					Instantiate(Prefab, hit.point + hit.normal * Offset, transform.rotation);
				}
			}
		}
	}
}