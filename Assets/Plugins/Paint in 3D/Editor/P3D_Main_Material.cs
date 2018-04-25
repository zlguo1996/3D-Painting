using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class P3D_Main
{
	[SerializeField]
	private bool showMaterial = true;

	private void DrawMaterial()
	{
		if (materials != null)
		{
			EditorGUILayout.Separator();

			BeginGroup(ref showMaterial, "Material"); if (showMaterial == true)
			{
				if (materials.Length > 0)
				{
					var rect    = P3D_Helper.Reserve(20.0f, true);
					var rect1   = rect; rect1.xMax -= 24;
					var rect2   = rect; rect2.xMin = rect1.xMax + 2;

					DrawMaterialToolbar(rect1);

					if (currentMaterial == null)
					{
						DrawCreateMaterial();
					}
					else
					{
						if (currentMaterial.hideFlags != HideFlags.None)
						{
							DrawDuplicateMaterial("This material's hideFlags indicate it shouldn't be modified, duplicate it to prevent any issues");
						}
						else
						{
							DrawDuplicateMaterial();

							if (P3D_Helper.IsAsset(currentMaterial) == false)
							{
								DrawSaveMaterial();
							}
						}
					}

					DrawAddLayer(rect2);
				}
				else
				{
					DrawAddMaterial();
				}
			}
			EndGroup();
		}
	}

	private void DrawMaterialToolbar(Rect rect)
	{
		var materialNames = new string[materials.Length];

		for (var i = materials.Length - 1; i >= 0; i--)
		{
			materialNames[i] = i.ToString();
		}

		currentMaterialIndex = GUI.SelectionGrid(rect, currentMaterialIndex, materialNames, materialNames.Length);
		currentMaterial      = P3D_Helper.GetIndexOrDefault(materials, currentMaterialIndex);

		EditorGUI.BeginDisabledGroup(true);
		{
			EditorGUILayout.ObjectField("", currentMaterial, typeof(Material), false);
		}
		EditorGUI.EndDisabledGroup();

		UpdateState();
	}

	private void DrawCreateMaterial()
	{
		BeginError();
		{
			EditorGUILayout.HelpBox("There is no material in this material slot", MessageType.Error);

			BeginColor(Color.green);
			{
				if (Button("Create") == true)
				{
					currentMaterial = new Material(Shader.Find("Diffuse"));
					currentMaterial.name = "New Material";

					materials[currentMaterialIndex] = currentMaterial;

					lockedRenderer.sharedMaterials = materials;
				}
			}
			EndColor();
		}
		EndError();
	}
	
	private void DrawAddLayer(Rect rect)
	{
		if (GUI.Button(rect, "+") == true)
		{
			var menu = new GenericMenu();

			menu.AddItem(new GUIContent("Unlit"), false, () => { AddLayer("Unlit/Transparent"); });
			menu.AddItem(new GUIContent("Unlit Cutout"), false, () => { AddLayer("Unlit/Transparent Cutout"); });

			menu.AddItem(new GUIContent("Diffuse"), false, () => { AddLayer("Transparent/Diffuse"); });
			menu.AddItem(new GUIContent("Diffuse Cutout"), false, () => { AddLayer("Transparent/Cutout/Diffuse"); });

			menu.AddItem(new GUIContent("Specular"), false, () => { AddLayer("Transparent/Specular"); });
			menu.AddItem(new GUIContent("Specular Cutout"), false, () => { AddLayer("Transparent/Cutout/Specular"); });

			menu.AddItem(new GUIContent("Bumped"), false, () => { AddLayer("Transparent/Bumped Diffuse"); });
			menu.AddItem(new GUIContent("Bumped Cutout"), false, () => { AddLayer("Transparent/Cutout/Bumped Diffuse"); });

#if UNITY_5
			menu.AddItem(new GUIContent("Standard"), false, () => { AddLayer("Standard"); });
#endif

			menu.DropDown(rect);
		}
	}

	private void AddLayer(string shaderName)
	{
		P3D_Helper.AddMaterial(lockedRenderer, Shader.Find(shaderName), currentMaterialIndex + 1);

		P3D_Helper.SetDirty(this);

		UpdateState();
	}

	private void DrawDuplicateMaterial(string errorMessage = null)
	{
		var showError = string.IsNullOrEmpty(errorMessage) == false;

		BeginError(showError);
		{
			if (showError == true)
			{
				EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
			}

			BeginColor(showError == true ? Color.green : GUI.color);
			{
				if (Button("Duplicate") == true)
				{
					materials[currentMaterialIndex] = currentMaterial = P3D_Helper.Clone(currentMaterial);

					lockedRenderer.sharedMaterials = materials;

					P3D_Helper.SetDirty(this);
				}
			}
			EndColor();
		}
		EndError();
	}

	private void DrawSaveMaterial()
	{
		if (Button("Save") == true)
		{
			var path = P3D_Helper.SaveDialog("Save Material", "Assets", currentMaterial.name, "mat");

			if (string.IsNullOrEmpty(path) == false)
			{
				var textures = P3D_Helper.CopyTextures(currentMaterial);

				AssetDatabase.CreateAsset(currentMaterial, path);

				P3D_Helper.PasteTextures(currentMaterial, textures);
			}
		}
	}

	private void DrawAddMaterial()
	{
		BeginError();
		{
			EditorGUILayout.HelpBox("This renderer contains no material slots", MessageType.Error);

			BeginColor(Color.green);
			{
				if (Button("Add Material") == true)
				{
					currentMaterialIndex = 0;
					currentMaterial      = new Material(Shader.Find("Diffuse"));
					materials            = new Material[] { currentMaterial };

					currentMaterial.name = "New Material";

					lockedRenderer.sharedMaterials = materials;

					P3D_Helper.SetDirty(this);
				}
			}
			EndColor();
		}
		EndError();
	}
}
