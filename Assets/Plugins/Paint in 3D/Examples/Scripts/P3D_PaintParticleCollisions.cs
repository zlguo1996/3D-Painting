using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_PaintParticleCollisions))]
public class P3D_PaintParticleCollisions_Editor : P3D_Editor<P3D_PaintParticleCollisions>
{
	protected override void OnInspector()
	{
		DrawDefault("LayerMask");
		DrawDefault("GroupMask");
		DrawDefault("Brush");
	}
}
#endif

// This component will paint every time a particle hits something
[RequireComponent(typeof(ParticleSystem))]
public class P3D_PaintParticleCollisions : MonoBehaviour
{
	[Tooltip("The GameObject layers you want to be able to paint")]
	public LayerMask LayerMask = -1;

	[Tooltip("The paintable texture groups you want to be able to paint")]
	public P3D_GroupMask GroupMask = -1;

	[Tooltip("The brush settings for the painting")]
	public P3D_Brush Brush;
	
	private ParticleSystem cachedParticleSystem;

	// Not supported in earlier versions
#if UNITY_5
	// The current collision events array
	private static ParticleCollisionEvent[] collisionEvents;

	protected virtual void OnEnable()
	{
		if (cachedParticleSystem == null)
		{
			cachedParticleSystem = GetComponent<ParticleSystem>();
		}
	}

	protected virtual void OnParticleCollision(GameObject paintTarget)
	{
		var paintable = paintTarget.GetComponent<P3D_Paintable>();
			
		if (paintable != null)
		{
			// Get the collision events array
			var count = cachedParticleSystem.GetSafeCollisionEventSize();

			if (collisionEvents == null || collisionEvents.Length < count)
			{
				collisionEvents = new ParticleCollisionEvent[count];
			}

			count = cachedParticleSystem.GetCollisionEvents(paintTarget, collisionEvents);
				
			// Paint the surface next to the collision intersection point
			for (var i = 0; i < count; i++)
			{
				paintable.PaintNearest(Brush, collisionEvents[i].intersection, 0.01f);
			}
		}
	}
#endif
}
