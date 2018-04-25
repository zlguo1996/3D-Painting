using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_ClickToPaintSubstep))]
public class P3D_ClickToPaintSubstep_Editor : P3D_Editor<P3D_ClickToPaintSubstep>
{
	protected override void OnInspector()
	{
		DrawDefault("Requires");
		DrawDefault("LayerMask");
		DrawDefault("GroupMask");
		DrawDefault("StepSize");
		DrawDefault("Paint");
		DrawDefault("Brush");
	}
}
#endif

// This script allows you to paint the scene using raycasts
// NOTE: This requires the paint targets have the P3D_Paintable component
public class P3D_ClickToPaintSubstep : P3D_ClickToPaint
{
	[Tooltip("The maximum amount of pixels between ")]
	public float StepSize = 1.0f;
	
	private Vector2 oldMousePosition;

	// Called every frame
	protected override void Update()
	{
		var mainCamera = Camera.main;
		
		if (mainCamera != null && StepSize > 0.0f)
		{
			// The required key is down?
			if (Input.GetKeyDown(Requires) == true)
			{
				oldMousePosition = Input.mousePosition;
            }

			// The required key is set?
			if (Input.GetKey(Requires) == true)
			{
				// Find the ray for this screen position
				var newMousePosition = (Vector2)Input.mousePosition;
				var stepCount        = Vector2.Distance(oldMousePosition, newMousePosition) / StepSize + 1;

				for (var i = 0; i < stepCount; i++)
				{
					var subMousePosition = Vector2.Lerp(oldMousePosition, newMousePosition, i / stepCount);
					var ray              = mainCamera.ScreenPointToRay(subMousePosition);
					var start            = ray.GetPoint(0.0f);
					var end              = ray.GetPoint(mainCamera.farClipPlane - mainCamera.nearClipPlane);

					// This will both use Physics.Raycast and search P3D_Paintables
					switch (Paint)
					{
						case PaintType.Nearest:        P3D_Paintable.ScenePaintBetweenNearest       (Brush, start, end, LayerMask, GroupMask); break;
						case PaintType.All:            P3D_Paintable.ScenePaintBetweenAll           (Brush, start, end, LayerMask, GroupMask); break;
						case PaintType.NearestRaycast: P3D_Paintable.ScenePaintBetweenNearestRaycast(Brush, start, end, LayerMask, GroupMask); break;
					}
				}

				oldMousePosition = newMousePosition;
			}
		}
	}
}
