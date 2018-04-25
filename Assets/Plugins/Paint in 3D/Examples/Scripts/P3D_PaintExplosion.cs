using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_PaintExplosion))]
public class P3D_PaintExplosion_Editor : P3D_Editor<P3D_PaintExplosion>
{
	protected override void OnInspector()
	{
		DrawDefault("RaycastMask");
		DrawDefault("GroupMask");
		BeginError(Any(t => t.Radius <= 0.0f));
			DrawDefault("Radius");
		EndError();
		BeginError(Any(t => t.Count <= 0));
			DrawDefault("Count");
		EndError();
		DrawDefault("MaxDelay");
		DrawDefault("Brush");
	}
}
#endif

// This script allows you to paint the scene using raycasts
// NOTE: This requires the paint targets have the P3D_Paintable component
public class P3D_PaintExplosion : MonoBehaviour
{
	[Tooltip("The layer mask used when raycasting into the scene")]
	public LayerMask RaycastMask = -1;

	[Tooltip("The paintable texture groups you want to be able to paint")]
	public P3D_GroupMask GroupMask = -1;

	[Tooltip("The explosion radius")]
	public float Radius = 5.0f;

	[Tooltip("The amount of raycasts to send out")]
	public int Count = 20;

	[Tooltip("The maximum amount of time it can take for a paint splat to hit")]
	public float MaxDelay = 0.25f;

	[Tooltip("The settings for the brush we will paint with")]
	public P3D_Brush Brush;

	protected virtual void Start()
	{
		// Fire off all raycasts
		for (var i = 0; i < Count; i++)
		{
			ExplodeOnce();
        }
    }

	public void ExplodeOnce()
	{
		// Randomly raycast from the current GameObject's position
		var direction = Random.onUnitSphere;
		var hit       = default(RaycastHit);

		if (Physics.Raycast(transform.position, direction, out hit, Radius, RaycastMask) == true)
		{
			// See if the object the raycast hit is paintable
			var paintable = hit.collider.GetComponent<P3D_Paintable>();

			if (paintable != null)
			{
				// Delay the painting via coroutine
				var distance01 = hit.distance / Radius;

				StartCoroutine(DelayedPaint(paintable, hit.textureCoord, distance01));
			}
		}
	}

	private IEnumerator DelayedPaint(P3D_Paintable paintable, Vector2 uv, float distance01)
	{
		// Delay it based on how far the ray travelled to hit
		yield return new WaitForSeconds(distance01 * MaxDelay);
		
		// Make sure the paintable still exists
		if (paintable != null)
		{
			// Make a temp brush clone so we can alter some settings without changing our actual brush
			var tempBrush = Brush.GetTempClone();

			// Set the opacity to be based on distance
			tempBrush.Opacity *= 1.0f - distance01;

			// Paint using our modified brush
			paintable.Paint(tempBrush, uv, GroupMask);
		}
	}

#if UNITY_EDITOR
	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, Radius);
	}
#endif
}
