using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_PaintOnCollision))]
public class P3D_PaintOnCollision_Editor : P3D_Editor<P3D_PaintOnCollision>
{
	protected override void OnInspector()
	{
		DrawDefault("GroupMask");
		BeginError(Any(t => t.Threshold < 0.0f));
			DrawDefault("Threshold");
		EndError();
		DrawDefault("Brush");
	}
}
#endif

// This script allows you to paint the scene using raycasts
// NOTE: This requires the paint targets have the P3D_Paintable component
public class P3D_PaintOnCollision : MonoBehaviour
{
	[Tooltip("The paintable texture groups you want to be able to paint")]
	public P3D_GroupMask GroupMask = -1;

	[Tooltip("The relative speed required for a paint to occur")]
	public float Threshold = 1.0f;

	[Tooltip("The settings for the brush we will paint with")]
	public P3D_Brush Brush;

	protected virtual void OnCollisionEnter(Collision collision)
	{
		Paint(collision);
    }

	protected virtual void OnCollisionStay(Collision collision)
	{
		Paint(collision);
	}

	private void Paint(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > Threshold)
		{
			// Loop through all contacts
			var contactPoints = collision.contacts;

			for (var i = 0; i < contactPoints.Length; i++)
			{
				var contactPoint = contactPoints[i];
				
				// See if the object this collider hit is paintable
				var paintable = contactPoint.otherCollider.GetComponent<P3D_Paintable>();
				
				if (paintable != null)
				{
					paintable.PaintNearest(Brush, contactPoint.point, 0.1f, GroupMask);
				}
			}
		}
    }
}
