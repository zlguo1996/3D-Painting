using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_ClickToThrow))]
public class P3D_ClickToThrow_Editor : P3D_Editor<P3D_ClickToThrow>
{
	protected override void OnInspector()
	{
		DrawDefault("Requires");
		BeginError(Any(t => t.Speed < 0.0f));
			DrawDefault("Speed");
		EndError();
		BeginError(Any(t => t.Prefab == null));
			DrawDefault("Prefab");
		EndError();
	}
}
#endif

// This script allows you to paint the scene using raycasts
// NOTE: This requires the paint targets have the P3D_Paintable component
public class P3D_ClickToThrow : MonoBehaviour
{
	[Tooltip("The key that must be held down to mouse look")]
	public KeyCode Requires = KeyCode.Mouse0;
	
	[Tooltip("The layer mask used when raycasting into the scene")]
	public float Speed = 10.0f;

	[Tooltip("The prefab we want to throw")]
	public GameObject Prefab;

	private Camera mainCamera;

	// Called every frame
	protected virtual void Update()
	{
		if (mainCamera == null) mainCamera = Camera.main;

		if (Prefab != null && mainCamera != null)
		{
			// The required key is down?
			if (Input.GetKeyDown(Requires) == true)
			{
				// Find the ray for this screen position
				var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

				// Spawn prefab
				var clone          = (GameObject)Instantiate(Prefab, ray.origin, Quaternion.LookRotation(ray.direction));
				var cloneRigidbody = clone.GetComponent<Rigidbody>();

				if (cloneRigidbody != null)
				{
					cloneRigidbody.velocity = clone.transform.forward * Speed;
                }
            }
		}
	}
}