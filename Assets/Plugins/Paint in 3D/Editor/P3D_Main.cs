using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// This is the main PaintIn3D editor window, and it provides a simple interface to the P3D_Painter class, among other features
public partial class P3D_Main : P3D_EditorWindow
{
	public static P3D_Main Instance;

	private static Mesh highlightMesh;

	private static Transform highlightTransform;
	
	[SerializeField]
	private Material[] materials;

	[SerializeField]
	private int currentMaterialIndex;

	[SerializeField]
	private Material currentMaterial;

	[SerializeField]
	private string[] texEnvNames;

	[SerializeField]
	private int currentTexEnvIndex;

	[SerializeField]
	private string currentTexEnvName;

	[SerializeField]
	private Texture2D currentTexture;

	[SerializeField]
	private bool oldMouseDown;

	[SerializeField]
	private Vector3 oldRayStart;

	[SerializeField]
	private Vector3 oldRayEnd;

	[SerializeField]
	private Vector2 oldMousePosition;

	[SerializeField]
	private int maxUndoLevels = 10;

	//[SerializeField]
	private List<P3D_UndoState> undoStates = new List<P3D_UndoState>();

	[SerializeField]
	private int undoIndex;

	[MenuItem("Window/Paint in 3D")]
	public static void OpenWindow()
	{
		EditorWindow.GetWindow(typeof(P3D_Main));
	}

	protected override void OnEnable()
	{
		Instance = this;

		base.OnEnable();

		SetTitle("Paint in 3D");

		LoadPresets();
	}

	protected override void OnDisable()
	{
		SavePresets();
	}

	protected virtual void OnDestroy()
	{
		Unlock();

		P3D_BrushPreview.Mark();
		P3D_BrushPreview.Sweep();
		P3D_TexturePreview.Mark();
		P3D_TexturePreview.Sweep();
	}

	protected override void OnInspector()
	{
		UpdateLock();
		UpdateState();

		DrawUndoRedo();

		EditorGUILayout.Separator();

		DrawLock();

		EditorGUILayout.Separator();

		if (locked == true)
		{
			DrawMesh();
			
			DrawMaterial();
			
			DrawTexture();
			
			DrawTilingOffset();
			
			DrawPaint();

			DrawBrush();

			DrawPresets();

			DrawPreview();
			
			DrawTools();

			if (lockedRenderer != null)
			{
				EditorUtility.SetSelectedWireframeHidden(lockedRenderer, showWireframe == false);
			}
		}

		EditorGUILayout.Separator();

		Repaint();
	}
	
	private static Vector2 lastMousePosition;

	// Draws the brush and texture previews in the main scene view
	protected override void OnScene(SceneView sceneView, Camera camera, Vector2 mousePosition)
	{
		if (sceneView == (SceneView)SceneView.sceneViews[0])
		{
			P3D_BrushPreview.Sweep();
			P3D_BrushPreview.Mark();

			P3D_TexturePreview.Sweep();
			P3D_TexturePreview.Mark();

			if (lastMousePosition != mousePosition)
			{
				sceneView.Repaint();
			}

			lastMousePosition = mousePosition;
		}

		UpdateLock();

		if (locked == true)
		{
			if (mousePosition.x >= 0.0f && mousePosition.x < sceneView.position.width && mousePosition.y >= 0.0f && mousePosition.y < sceneView.position.height)
			{
				if (Event.current.button == 0)
				{
					if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
					{
						Paint(camera, mousePosition);
					}
				}

				ShowBrushPreview(camera, mousePosition);
			}

			if (Event.current.type == EventType.MouseUp)
			{
				FinishPaint();
			}

			ShowTexturePreview();
		}
	}

	private void DrawUndoRedo()
	{
		var rect1 = P3D_Helper.Reserve();
		var rect2 = P3D_Helper.SplitHorizontal(ref rect1, 2);

		EditorGUI.BeginDisabledGroup(undoIndex <= 0);
		{
			if (GUI.Button(rect1, "Undo") == true)
			{
				PerformUndo();
			}
		}
		EditorGUI.EndDisabledGroup();

		EditorGUI.BeginDisabledGroup(undoIndex >= undoStates.Count - 1);
		{
			if (GUI.Button(rect2, "Redo") == true)
			{
				PerformRedo();
			}
		}
		EditorGUI.EndDisabledGroup();
	}

	private void ClearUndo()
	{
		undoIndex = 0;

		undoStates.Clear();
	}

	private void StartRecordUndo()
	{
		if (undoStates.Count > 0)
		{
			var currentUndoState = undoStates[undoIndex];

			if (currentUndoState.Texture == currentTexture)
			{
				return;
			}
		}

		RecordUndo(false);
	}

	private void RecordUndo(bool removeDecoupled = true)
	{
		Repaint();

		if (currentTexture != null)
		{
			// Remove redo states?
			if (undoIndex < undoStates.Count - 1)
			{
				undoStates.RemoveRange(undoIndex + 1, undoStates.Count - undoIndex - 1);
			}

			// Add new state
			var newUndoState = new P3D_UndoState(currentTexture);

			undoStates.Add(newUndoState);

			// Too many?
			if (undoStates.Count > maxUndoLevels)
			{
				undoStates.RemoveAt(0);
			}

			// Remove decoupled states?
			if (removeDecoupled == true)
			{
				for (var i = undoStates.Count - 1; i >= 0; i--)
				{
					var thisTexture = GetUndoTexture(i);

					if (thisTexture != GetUndoTexture(i - 1) && thisTexture != GetUndoTexture(i + 1))
					{
						undoStates.RemoveAt(i);
					}
				}
			}

			undoIndex = undoStates.Count - 1;
		}
	}

	private void PerformUndo()
	{
		if (undoIndex > 0)
		{
			if (GetUndoTexture(undoIndex) != GetUndoTexture(undoIndex - 1))
			{
				undoIndex -= 2;
			}
			else
			{
				undoIndex -= 1;
			}

			undoStates[undoIndex].Perform();
		}
	}

	private void PerformRedo()
	{
		if (undoIndex < undoStates.Count - 1)
		{
			if (GetUndoTexture(undoIndex) != GetUndoTexture(undoIndex + 1))
			{
				undoIndex += 2;
			}
			else
			{
				undoIndex += 1;
			}

			undoStates[undoIndex].Perform();
		}
	}

	private Texture2D GetUndoTexture(int index)
	{
		if (index >= 0 && index < undoStates.Count)
		{
			return undoStates[index].Texture;
		}

		return null;
	}

	private void Paint(Camera camera, Vector2 mousePosition)
	{
		if (currentTexture != null)
		{
			StartRecordUndo();

			var ray      = HandleUtility.GUIPointToWorldRay(mousePosition);
			var rayStart = ray.origin + ray.direction * camera.nearClipPlane;
			var rayEnd   = ray.origin + ray.direction * camera.farClipPlane;

			BeginPaint();
			{
				if (oldMouseDown == true)// && currentTool != ToolType.)
				{
					// Find how many paint steps we should do between the old mouse position and the new mouse position
					var steps = Mathf.FloorToInt(Vector2.Distance(mousePosition, oldMousePosition) * resolution + 0.1f);
					
					// More than one step?
					if (steps > 0)
					{
						var stepsRecip = P3D_Helper.Reciprocal(steps);

						for (var i = 0; i <= steps; i++)
						{
							var subRayStart = Vector3.Lerp(oldRayStart, rayStart, i * stepsRecip);
							var subRayEnd   = Vector3.Lerp(oldRayEnd  , rayEnd  , i * stepsRecip);

							Paint(subRayStart, subRayEnd);
						}
					}
					else
					{
						Paint(rayStart, rayEnd);
					}
				}
				else
				{
					Paint(rayStart, rayEnd);
				}
			}
			EndPaint();

			oldMouseDown     = true;
			oldRayStart      = rayStart;
			oldRayEnd        = rayEnd;
			oldMousePosition = mousePosition;
		}
	}

	private void FinishPaint()
	{
		if (oldMouseDown == true)
		{
			oldMouseDown = false;

			RecordUndo();
		}
	}

	private void UpdateState()
	{
		if (lockedGameObject != null && lockedRenderer != null)
		{
			materials = lockedRenderer.sharedMaterials;

			if (currentMaterialIndex < 0)
			{
				currentMaterialIndex = 0;
			}

			currentMaterial = P3D_Helper.GetIndexOrDefault(materials, ref currentMaterialIndex);
			texEnvNames     = P3D_Helper.GetTexEnvNames(currentMaterial);

			if (currentTexEnvIndex < 0)
			{
				currentTexEnvIndex = 0;
			}

			currentTexEnvName = P3D_Helper.GetIndexOrDefault(texEnvNames, ref currentTexEnvIndex);

			if (string.IsNullOrEmpty(currentTexEnvName) == false)
			{
				currentTexture = currentMaterial.GetTexture(currentTexEnvName) as Texture2D;
			}
			else
			{
				currentTexture = null;
			}
		}
		else
		{
			lockedGameObject  = null;
			lockedRenderer    = null;
			materials         = null;
			currentMaterial   = null;
			texEnvNames       = null;
			currentTexEnvName = null;
			currentTexture    = null;
		}
	}

	private Rect GetPopupRect(Rect reservedRect)
	{
		var mouse = Event.current.mousePosition;
		
		mouse.x += position.x + scrollPosition.x;
		mouse.y += position.y - scrollPosition.y;

		return new Rect(mouse.x, mouse.y, 0.0f, 0.0f);
	}
}
