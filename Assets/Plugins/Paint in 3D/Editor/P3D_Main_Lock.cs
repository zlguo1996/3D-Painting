using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class P3D_Main
{
	[SerializeField]
	private bool locked;
	
	[SerializeField]
	private GameObject lockedGameObject;
	
	[SerializeField]
	private Renderer lockedRenderer;
	
	[SerializeField]
	private Mesh lockedMesh;
	
	[SerializeField]
	private Mesh bakedMesh;
	
	[SerializeField]
	private Tool oldTool;
	
	private static List<GameObject> selectedGameObjects = new List<GameObject>();
	
	private void UpdateSelectedGameObjects()
	{
		selectedGameObjects.Clear();
		
		foreach (var gameObject in Selection.gameObjects)
		{
			AddGameObjects(gameObject);
		}
	}
	
	private void AddGameObjects(GameObject gameObject)
	{
		if (selectedGameObjects.Contains(gameObject) == false)
		{
			selectedGameObjects.Add(gameObject);
			
			var transform = gameObject.transform;
			
			for (var i = 0; i < transform.childCount; i++)
			{
				AddGameObjects(transform.GetChild(i).gameObject);
			}
		}
	}
	
	private void DrawLock()
	{
		UpdateSelectedGameObjects();
		
		if (locked == true)
		{
			BeginColor(Color.red);
			{
				if (Button("Unlock " + lockedGameObject.name) == true)
				{
					Unlock();
				}
			}
			EndColor();

			if (locked == true)
			{
				EditorGUI.BeginDisabledGroup(selectedGameObjects.Count == 1 && selectedGameObjects[0] == lockedGameObject);
				{
					if (Button("Select " + lockedGameObject.name) == true)
					{
						Selection.objects = new Object[] { lockedGameObject };
					}
				}
				EditorGUI.EndDisabledGroup();
			}
		}
		else
		{
			var lockable = false;
			
			for (var i = selectedGameObjects.Count - 1; i >= 0; i--)
			{
				var gameObject = selectedGameObjects[i];
				
				if (CanLock(gameObject) == true)
				{
					lockable = true;
					
					BeginColor(Color.green);
					{
						if (Button("Lock " + gameObject.name) == true)
						{
							Lock(gameObject);
						}
					}
					EndColor();
				}
			}
			
			if (lockable == false)
			{
				EditorGUILayout.HelpBox("Select a GameObject with a MeshFilter & MeshRenderer", MessageType.Info);
			}
		}
	}
	
	private void UpdateLock()
	{
		if (CanLock(lockedGameObject) == true)
		{
			Lock(lockedGameObject);
			
			// Only allow a selection change if it's something not in the scene
			//if (Selection.activeObject != lockedGameObject && P3D_Helper.IsAsset(Selection.activeObject) == true)
			//{
			//	Unlock();
			//}
			//else
			//{
				// Keep selection locked?
				//if (showWireframe == true)
				//{
				//	Selection.objects = new Object[] { lockedGameObject };
				//}

				// Disable drag selection
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
				
				// Disable gizmos
				Tools.current = Tool.None;
			//}
		}
		else
		{
			Unlock();
		}
	}
	
	private static bool CanLock(GameObject gameObject)
	{
		if (gameObject != null)
		{
			// Skinned mesh renderer?
			var skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
			
			if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
			{
				return true;
			}
			
			// Mesh renderer?
			var meshFilter   = gameObject.GetComponent<MeshFilter>();
			var meshRenderer = gameObject.GetComponent<MeshRenderer>();
			
			if (meshRenderer != null && meshFilter != null && meshFilter.sharedMesh != null)
			{
				return true;
			}
		}
		
		return false;
	}
	
	private void Lock(GameObject gameObject)
	{
		if (gameObject != null)
		{
			var newRenderer = gameObject.GetComponent<Renderer>();

			if (newRenderer != null)
			{
				var newMesh = P3D_Helper.GetMesh(gameObject, ref bakedMesh);

				if (newMesh != null)
				{
					if (locked == false)
					{
						locked  = true;
						oldTool = Tools.current;
					}

					lockedGameObject = gameObject;
					lockedRenderer   = newRenderer;
					lockedMesh       = newMesh;
					locked           = true;
				}
			}
		}
	}
	
	private void Unlock()
	{
		if (locked == true)
		{
			Tools.current = oldTool;
			
			if (lockedRenderer != null)
			{
				EditorUtility.SetSelectedWireframeHidden(lockedRenderer, false);
			}
			
			locked           = false;
			lockedGameObject = null;
			lockedRenderer   = null;
			lockedMesh       = null;
			
			FinishPaint();
			
			Repaint();
		}
	}
}