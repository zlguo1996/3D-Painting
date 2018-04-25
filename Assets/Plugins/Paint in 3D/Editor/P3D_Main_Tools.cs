using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class P3D_Main
{
	[SerializeField]
	private bool showResize;

	[SerializeField]
	private bool showTools;
	
	public void Resize(int newWidth, int newHeight)
	{
		if (currentTexture != null)
		{
			if (currentTexture.width != newWidth || currentTexture.height != newHeight)
			{
				StartRecordUndo();
				{
					var oldColors   = currentTexture.GetPixels();
					var oldWidth    = currentTexture.width;
					var oldHeight   = currentTexture.height;
					var widthRecip  = P3D_Helper.Reciprocal(newWidth);
					var heightRecip = P3D_Helper.Reciprocal(newHeight);

					currentTexture.Resize(newWidth, newHeight);

					for (var y = newHeight - 1; y >= 0; y--)
					{
						for (var x = newWidth - 1; x >= 0; x--)
						{
							var u = x * widthRecip;
							var v = y * heightRecip;
							var z = u * oldWidth;
							var w = v * oldHeight;
							var c = GetPixelBilinear(oldColors, oldWidth, oldHeight, z, w);

							currentTexture.SetPixel(x, y, c);
						}
					}

					currentTexture.Apply();
				}
				RecordUndo();
			}
		}
	}

	public void Clear(Color color, bool recordUndo = true)
	{
		if (currentTexture != null)
		{
			if (recordUndo == true) StartRecordUndo();
			{
				P3D_Helper.ClearTexture(currentTexture, color, false);
			}
			if (recordUndo == true) RecordUndo();

			currentTexture.Apply();
		}
	}
	
	private void DrawTools()
	{
		if (currentTexture != null)
		{
			EditorGUILayout.Separator();

			BeginGroup(ref showTools, "Tools"); if (showTools == true)
			{
				maxUndoLevels = EditorGUILayout.IntField("Max Undo Levels", maxUndoLevels);

				EditorGUILayout.Separator();

				DrawPad();

				DrawResize();

				DrawClear();
			}
			EndGroup();
		}
	}

	private void DrawPad()
	{
		if (Button("Apply Edge Padding") == true)
		{
			PadEdges();
		}
	}

	private void DrawResize()
	{
		var rect = P3D_Helper.Reserve(16.0f, true);
		
		if (GUI.Button(rect, "Resize") == true)
		{
			P3D_ResizeWindow.Pop(GetPopupRect(rect), currentTexture);
		}
	}

	private void DrawClear()
	{
		var rect = P3D_Helper.Reserve(16.0f, true);
		
		if (GUI.Button(rect, "Clear") == true)
		{
			P3D_ClearWindow.Pop(GetPopupRect(rect), currentTexture);
		}
	}

	private Color GetPixelBilinear(Color[] pixels, int width, int height, float u, float v)
	{
		var x = Mathf.FloorToInt(u);
		var y = Mathf.FloorToInt(v);
		var z = u % 1.0f;
		var w = v % 1.0f;
		var a = GetPixel(pixels, width, height, x    , y    );
		var b = GetPixel(pixels, width, height, x + 1, y    );
		var c = GetPixel(pixels, width, height, x    , y + 1);
		var d = GetPixel(pixels, width, height, x + 1, y + 1);

		var ab = Color.Lerp(a, b, z);
		var cd = Color.Lerp(c, d, z);

		return Color.Lerp(ab, cd, w);
	}

	private Color GetPixel(Color[] pixels, int width, int height, int x, int y)
	{
		if (x >= width ) x = width  - 1;
		if (y >= height) y = height - 1;

		return pixels[y * width + x];
	}

	private Color GetPixel2(Color[] pixels, int width, int height, int x, int y)
	{
		if (x < 0) x = 0; else if (x >= width ) x = width  - 1;
		if (y < 0) y = 0; else if (y >= height) y = height - 1;

		return pixels[y * width + x];
	}

	private void PadEdges(int distance = 8)
	{
		if (currentTexture != null)
		{
#if UNITY_EDITOR
			P3D_Helper.MakeTextureReadable(currentTexture);
#endif
			StartRecordUndo();
			{
				var width  = currentTexture.width;
				var height = currentTexture.height;
				var total  = width * height;
				var pixels = currentTexture.GetPixels();

				for (var i = 0; i < distance; i++)
				{
					for (var y = height - 1; y >= 0; y--)
					{
						var offset = width * y;

						for (var x = width - 1; x >= 0; x--)
						{
							var index = offset + x;
							var pixel = pixels[index];

							if (pixel.a == 0.0f)
							{
								var pixel1 = GetPixel2(pixels, width, height, x - 1, y    );
								var pixel2 = GetPixel2(pixels, width, height, x + 1, y    );
								var pixel3 = GetPixel2(pixels, width, height, x    , y - 1);
								var pixel4 = GetPixel2(pixels, width, height, x    , y + 1);
								var sum    = 0;

								pixel.r = pixel.g = pixel.b = 0.0f;

								if (pixel1.a != 0.0f)
								{
									pixel.r = pixel1.r; pixel.g = pixel1.g; pixel.b = pixel1.b; sum += 1;
								}

								if (pixel2.a != 0.0f)
								{
									pixel.r += pixel2.r; pixel.g += pixel2.g; pixel.b += pixel2.b; sum += 1;
								}

								if (pixel3.a != 0.0f)
								{
									pixel.r += pixel3.r; pixel.g += pixel3.g; pixel.b += pixel3.b; sum += 1;
								}

								if (pixel4.a != 0.0f)
								{
									pixel.r += pixel4.r; pixel.g += pixel4.g; pixel.b += pixel4.b; sum += 1;
								}

								if (sum > 0)
								{
									pixel.r /= sum;
									pixel.g /= sum;
									pixel.b /= sum;
									pixel.a  = -1.0f;

									pixels[index] = pixel;
								}
							}
						}
					}
				}

				for (var i = 0; i < total; i++)
				{
					var pixel = pixels[i];

					if (pixel.a < 0.0f)
					{
						pixel.a = 0.0f;

						pixels[i] = pixel;
					}
				}

				currentTexture.SetPixels(pixels);
			}
			RecordUndo();

			currentTexture.Apply();
		}
	}
}
