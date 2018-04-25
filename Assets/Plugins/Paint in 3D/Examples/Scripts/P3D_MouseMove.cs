using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_MouseMove))]
public class P3D_MouseMove_Editor : P3D_Editor<P3D_MouseMove>
{
	protected override void OnInspector()
	{
		DrawDefault("Requires");
		BeginError(Any(t => t.Speed < 0.0f));
			DrawDefault("Speed");
		EndError();
		BeginError(Any(t => t.Acceleration <= 0.0f));
			DrawDefault("Acceleration");
		EndError();
	}
}
#endif

// This component handles mouselook when attached to the camera
[ExecuteInEditMode]
public class P3D_MouseMove : MonoBehaviour
{
	[Tooltip("The key that must be held down to mouse look")]
	public KeyCode Requires = KeyCode.None;

	[Tooltip("The speed the camera moves when pressing a direction")]
	public float Speed = 1.0f;

	[Tooltip("How fast the camera accelerates to its target speed")]
	public float Acceleration = 5.0f;

	private Vector3 targetPosition;

	protected virtual void Start()
	{
		targetPosition = transform.position;
	}

	protected virtual void Update()
	{
		// If the required key is pressed, adjust the target position
		if (Requires == KeyCode.None || Input.GetKey(Requires) == true)
		{
			targetPosition += transform.forward * Input.GetAxisRaw("Vertical") * Speed * Time.deltaTime;

			targetPosition += transform.right * Input.GetAxisRaw("Horizontal") * Speed * Time.deltaTime;
		}

		// Move current position toward the target position
		transform.position = P3D_Helper.Dampen3(transform.position, targetPosition, Acceleration, Time.deltaTime, 0.1f);
	}
}