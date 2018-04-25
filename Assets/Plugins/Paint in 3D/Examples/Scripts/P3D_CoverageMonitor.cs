using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_CoverageMonitor))]
public class P3D_CoverageMonitor_Editor : P3D_Editor<P3D_CoverageMonitor>
{
	protected override void OnInspector()
	{
		DrawDefault("CutOff");
		DrawDefault("TargetGameObject");

		if (Any(t => t.TargetGameObject != null))
		{
			BeginIndent();
				DrawDefault("MaterialIndex");
				DrawDefault("TextureName");
			EndIndent();
		}

		DrawDefault("TargetTexture");
		DrawDefault("TotalCoverage");
	}
}
#endif

// This script will calculate how many solid pixels you have painted onto the target texture
public class P3D_CoverageMonitor : MonoBehaviour
{
	[Tooltip("How sensitive the coverage monitor is")]
	public float CutOff = 0.1f;

	[Tooltip("The texture we're monitoring for coverage")]
	public Texture2D TargetTexture;

	[Tooltip("The GameObject that has the target texture")]
	public GameObject TargetGameObject;

	[Tooltip("The material index we want to paint")]
	public int MaterialIndex;

	[Tooltip("The texture we want to paint")]
	public string TextureName = "_MainTex";

	[Tooltip("The total amount of pixels that have been painted solid")]
	public int TotalCoverage;
	
	private int prePaintCoverage;

	protected virtual void OnEnable()
	{
		P3D_Brush.OnPrePaint  += PrePaint;
		P3D_Brush.OnPostPaint += PostPaint;
	}

	protected virtual void Update()
	{
		// Find the target texture?
		if (TargetTexture == null)
		{
			var material = P3D_Helper.GetMaterial(TargetGameObject, MaterialIndex);

			if (material != null)
			{
				TargetTexture = material.GetTexture(TextureName) as Texture2D;
			}
		}
	}

	protected virtual void OnGUI()
	{
		P3D_Gui.DrawText("Coverage = " + TotalCoverage + " pixels", TextAnchor.MiddleRight);
	}

	protected virtual void OnDisable()
	{
		P3D_Brush.OnPrePaint  -= PrePaint;
		P3D_Brush.OnPostPaint -= PostPaint;
	}

	private void PrePaint(Texture2D canvas, P3D_Rect rect)
	{
		if (canvas == TargetTexture)
		{
			prePaintCoverage = CalculateCoverage(canvas, rect);
		}
	}

	private void PostPaint(Texture2D canvas, P3D_Rect rect)
	{
		if (canvas == TargetTexture)
		{
			var postPaintCoverage = CalculateCoverage(canvas, rect);

			TotalCoverage += postPaintCoverage - prePaintCoverage;
		}
	}

	private int CalculateCoverage(Texture2D canvas, P3D_Rect rect)
	{
		var coverage = 0;

		for (var x = rect.XMin; x < rect.XMax; x++)
		{
			for (var y = rect.YMin; y < rect.YMax; y++)
			{
				var pixel = canvas.GetPixel(x, y);

				if (pixel.a > CutOff)
				{
					coverage += 1;
				}
			}
		}

		return coverage;
	}
}
