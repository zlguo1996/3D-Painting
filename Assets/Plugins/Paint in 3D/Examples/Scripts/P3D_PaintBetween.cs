using UnityEngine;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_PaintBetween))]
public class P3D_PaintBetween_Editor : P3D_Editor<P3D_PaintBetween>
{
	protected override void OnInspector()
	{
		DrawDefault("LayerMask");
		DrawDefault("GroupMask");
		BeginError(Any(t => t.Start == null));
			DrawDefault("Start");
		EndError();
		BeginError(Any(t => t.End == null));
			DrawDefault("End");
		EndError();
		DrawDefault("Paint");
		DrawDefault("Brush");
	}
}
#endif

// This script allows you to paint onto the target object. Any pixels between the Start and End points will be painted red. It also allows you to control the rotation of the target object.
public class P3D_PaintBetween : MonoBehaviour
{
	public enum PaintType
	{
		NearestRaycast,
		All,
		Nearest
	}

	[Tooltip("The GameObject layers you want to be able to paint")]
	public LayerMask LayerMask = -1;

	[Tooltip("The paintable texture groups you want to be able to paint")]
	public P3D_GroupMask GroupMask = -1;

	[Tooltip("This transform marks the start point of the painting ray")]
	public Transform Start;

	[Tooltip("This transform marks the end point of the painting ray")]
	public Transform End;

	[Tooltip("Which surfaces it should hit")]
	public PaintType Paint;

	[Tooltip("The settings for the brush we will paint with")]
	public P3D_Brush Brush;

	// This will paint the target object every frame
	protected virtual void Update()
	{
		if (Start != null && End != null)
		{
			// Paint between the start and end positions
			switch (Paint)
			{
				case PaintType.Nearest:        P3D_Paintable.ScenePaintBetweenNearest       (Brush, Start.position, End.position, LayerMask, GroupMask); break;
				case PaintType.All:            P3D_Paintable.ScenePaintBetweenAll           (Brush, Start.position, End.position, LayerMask, GroupMask); break;
				case PaintType.NearestRaycast: P3D_Paintable.ScenePaintBetweenNearestRaycast(Brush, Start.position, End.position, LayerMask, GroupMask); break;
			}
		}
	}
}
