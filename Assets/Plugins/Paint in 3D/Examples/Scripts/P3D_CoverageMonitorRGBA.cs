using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(P3D_CoverageMonitorRGBA))]
public class P3D_CoverageMonitorRGBA_Editor : P3D_Editor<P3D_CoverageMonitorRGBA>
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
		DrawDefault("TotalCoverageA");
		DrawDefault("TotalCoverageB");
		DrawDefault("TotalCoverageC");
		DrawDefault("TotalCoverageD");
	}
}
#endif

// This script will calculate how many solid pixels you have painted onto the target texture
public class P3D_CoverageMonitorRGBA : MonoBehaviour
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

	[Tooltip("The total amount of pixels that have been painted solid red")]
	public int TotalCoverageA;

	[Tooltip("The total amount of pixels that have been painted solid green")]
	public int TotalCoverageB;

	[Tooltip("The total amount of pixels that have been painted solid blue")]
	public int TotalCoverageC;

	[Tooltip("The total amount of pixels that have been painted solid alpha")]
	public int TotalCoverageD;
	
	private int prePaintCoverageA;
	private int prePaintCoverageB;
	private int prePaintCoverageC;
	private int prePaintCoverageD;

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
		var text = "";

		text += "Color A = " + TotalCoverageA + " pixels\n";
		text += "Color B = " + TotalCoverageB + " pixels\n";
		text += "Color C = " + TotalCoverageC + " pixels\n";
		text += "Color D = " + TotalCoverageD + " pixels";

		P3D_Gui.DrawText(text, TextAnchor.MiddleRight);
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
			CalculateCoverage(canvas, rect, ref prePaintCoverageA, ref prePaintCoverageB, ref prePaintCoverageC, ref prePaintCoverageD);
		}
	}

	private void PostPaint(Texture2D canvas, P3D_Rect rect)
	{
		if (canvas == TargetTexture)
		{
			var postPaintCoverageA = 0;
			var postPaintCoverageB = 0;
			var postPaintCoverageC = 0;
			var postPaintCoverageD = 0;

			CalculateCoverage(canvas, rect, ref postPaintCoverageA, ref postPaintCoverageB, ref postPaintCoverageC, ref postPaintCoverageD);

			TotalCoverageA += postPaintCoverageA - prePaintCoverageA;
			TotalCoverageB += postPaintCoverageB - prePaintCoverageB;
			TotalCoverageC += postPaintCoverageC - prePaintCoverageC;
			TotalCoverageD += postPaintCoverageD - prePaintCoverageD;
		}
	}

	private void CalculateCoverage(Texture2D canvas, P3D_Rect rect, ref int coverageA, ref int coverageB, ref int coverageC, ref int coverageD)
	{
		coverageA = 0;
		coverageB = 0;
		coverageC = 0;
		coverageD = 0;

		for (var x = rect.XMin; x < rect.XMax; x++)
		{
			for (var y = rect.YMin; y < rect.YMax; y++)
			{
				var pixel      = canvas.GetPixel(x, y);
				var highestIdx = -1;
				var highestVal = CutOff;
				
				for (var i = 0; i < 4; i++)
				{
					var val = pixel[i];

					if (val >= highestVal)
					{
						highestIdx = i;
						highestVal = val;
					}
				}
				
				switch (highestIdx)
				{
					case 0: coverageA += 1; break;
					case 1: coverageB += 1; break;
					case 2: coverageC += 1; break;
					case 3: coverageD += 1; break;
				}
			}
		}
	}
}
