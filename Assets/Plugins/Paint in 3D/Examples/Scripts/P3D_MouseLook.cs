using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_MouseLook))]
public class P3D_MouseLook_Editor : P3D_Editor<P3D_MouseLook>
{
	protected override void OnInspector()
	{
		DrawDefault("Requires");
		BeginError(Any(t => t.Sensitivity <= 0.0f));
			DrawDefault("Sensitivity");
		EndError();
		BeginError(Any(t => t.Acceleration <= 0.0f));
			DrawDefault("Acceleration");
		EndError();
		DrawDefault("TargetPitch");
		DrawDefault("TargetYaw");
	}
}
#endif

// This component handles mouselook when attached to the camera
[ExecuteInEditMode]
public class P3D_MouseLook : MonoBehaviour
{
	[Tooltip("The key that must be held down to mouse look")]
	public KeyCode Requires = KeyCode.Mouse0;

	[Tooltip("How sensitive the mouse inputs are")]
	public float Sensitivity = 2.0f;

	[Tooltip("How quickly the camera accelerates toward the target rotation")]
	public float Acceleration = 10.0f;

	[Tooltip("The target camera pitch (up down)")]
	public float TargetPitch;

	[Tooltip("The target camera yaw (left right)")]
	public float TargetYaw;

	private float currentPitch;

	private float currentYaw;

	protected virtual void Awake()
	{
		currentPitch = TargetPitch;
		currentYaw   = TargetYaw;
	}

	protected virtual void Update()
	{
		TargetPitch = Mathf.Clamp(TargetPitch, -89.9f, 89.9f);

		// Change the target pitch & yaw values if the required key is pressed
		if (Requires == KeyCode.None || Input.GetKey(Requires) == true)
		{
			TargetPitch -= Input.GetAxisRaw("Mouse Y") * Sensitivity;
			TargetYaw   += Input.GetAxisRaw("Mouse X") * Sensitivity;
		}

		// If it's in edit mode there is no Time.deltaTime, so instantly snap to the target rotation
#if UNITY_EDITOR
		if (Application.isPlaying == false)
		{
			currentPitch = TargetPitch;
			currentYaw   = TargetYaw;
		}
#endif

		// Move current rotation toward the target position
		currentPitch = P3D_Helper.Dampen(currentPitch, TargetPitch, Acceleration, Time.deltaTime);
		currentYaw   = P3D_Helper.Dampen(currentYaw  , TargetYaw  , Acceleration, Time.deltaTime);
		
		transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);
	}
}