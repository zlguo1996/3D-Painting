using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_ClickToPaintChange))]
public class P3D_ClickToPaintChange_Editor : P3D_Editor<P3D_ClickToPaintChange>
{
	protected override void OnInspector()
	{
		BeginError(Any(t => t.Target == null));
			DrawDefault("Target");
		EndError();
		DrawDefault("Color");
	}
}
#endif

// This script allows you to paint the scene using raycasts
// NOTE: This requires the paint targets have the P3D_Paintable component
public class P3D_ClickToPaintChange : MonoBehaviour
{
	[Tooltip("The ClickToPaint component you want to change")]
	public P3D_ClickToPaint Target;

	[Tooltip("The color you want to paint the target")]
	public Color Color = Color.white;
	
	private Vector2 oldMousePosition;

	public void ChangeColor()
	{
		if (Target != null)
		{
			var brush = Target.Brush;

			if (brush != null)
			{
				brush.Color = Color;
			}
		}
	}
}
