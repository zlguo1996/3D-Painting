using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_PaintNearby))]
public class P3D_PaintNearby_Editor : P3D_Editor<P3D_PaintNearby>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.MaxDistance <= 0.0f));
			DrawDefault("MaxDistance");
		EndError();
		DrawDefault("LayerMask");
		DrawDefault("GroupMask");
		BeginError(Any(t => t.Interval < 0.0f));
			DrawDefault("Interval");
		EndError();
		DrawDefault("Paint");
		DrawDefault("Brush");
	}
}
#endif

// This script allows you to paint the scene using raycasts
// NOTE: This requires the paint targets have the P3D_Paintable component
public class P3D_PaintNearby : MonoBehaviour
{
	public enum PaintType
	{
		Nearest,
		All
	}

	[Tooltip("The maximum distance from the current GameObject that can be painted")]
	public float MaxDistance = 1.0f;

	[Tooltip("The GameObject layers you want to be able to paint")]
	public LayerMask LayerMask = -1;

	[Tooltip("The paintable texture groups you want to be able to paint")]
	public P3D_GroupMask GroupMask = -1;

	[Tooltip("The amount of seconds between each paint event")]
	public float Interval = 1.0f;

	[Tooltip("Which surfaces it should hit")]
	public PaintType Paint;

	[Tooltip("The settings for the brush we will paint with")]
	public P3D_Brush Brush;

	private float cooldown;

	protected virtual void Update()
	{
		cooldown -= Time.deltaTime;

        if (cooldown <= 0.0f)
		{
			cooldown = Interval;

			var position = transform.position;
			
			switch (Paint)
			{
				case PaintType.Nearest: P3D_Paintable.ScenePaintPerpedicularNearest(Brush, position, MaxDistance, LayerMask, GroupMask); break;
				case PaintType.All:     P3D_Paintable.ScenePaintPerpedicularAll    (Brush, position, MaxDistance, LayerMask, GroupMask); break;
			}
        }
    }

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, MaxDistance);
	}
#endif
}
