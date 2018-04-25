using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class P3D_Main
{
	[SerializeField]
	private P3D_CoordType currentCoord = P3D_CoordType.UV1;
	
	[SerializeField]
	private float resolution = 1.0f;

	[SerializeField]
	private bool passThrough;

	[SerializeField]
	private float scatterAngle;

	[SerializeField]
	private float scatterPosition;

	[SerializeField]
	private float scatterScale;

	[SerializeField]
	private bool showPaint;

	private static string[] coordNames = new string[] { "UV 1", "UV 2" };

	private void DrawPaint()
	{
		EditorGUILayout.Separator();

		BeginGroup(ref showPaint, "Paint"); if (showPaint == true)
		{
			BeginLabelWidth(Mathf.Min(85.0f, position.width * 0.5f));
			{
				currentCoord = (P3D_CoordType)GUI.Toolbar(P3D_Helper.Reserve(20.0f, true), (int)currentCoord, coordNames);
				
				resolution = EditorGUILayout.Slider("Resolution", resolution, 0.0f, 5.0f);

				scatterPosition = EditorGUILayout.FloatField("Scatter Position", scatterPosition);

				scatterAngle = EditorGUILayout.Slider("Scatter Angle", scatterAngle, 0.0f, Mathf.PI * 2.0f);

				scatterScale = EditorGUILayout.Slider("Scatter Scale", scatterScale, 0.0f, 1.0f);

				passThrough = EditorGUILayout.Toggle("Pass Through", passThrough);
			}
			EndLabelWidth();
		}
		EndGroup();
	}
}
