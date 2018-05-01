using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditor.SceneManagement;

[CustomEditor(typeof(terrainStream))]
public class terrainStreamEditor : Editor {

	//private bool isDone = false;


	public override void OnInspectorGUI(){

		//if (isDone)
		//	return;

		terrainStream tS = (terrainStream)target;

		if (GUILayout.Button ("Update stream to Terrain")) {
			tS.streamToTerrainTrees ();
		}

		//isDone = true;


	}

		
}
