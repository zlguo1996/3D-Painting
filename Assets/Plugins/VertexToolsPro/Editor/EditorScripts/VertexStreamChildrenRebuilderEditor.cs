using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditor.SceneManagement;

[CustomEditor(typeof(VertexStreamChildrenRebuilder))]
public class VertexStreamChildrenRebuilderEditor : Editor {

	//private bool isDone = false;


	public override void OnInspectorGUI(){

		//if (isDone)
		//	return;

		VertexStreamChildrenRebuilder vcs = (VertexStreamChildrenRebuilder)target;

		VertexColorStream[] vcss = vcs.gameObject.GetComponentsInChildren<VertexColorStream> ();

		foreach (VertexColorStream _vcs in vcss) {

			_vcs.rebuild ();

		}

		AssetDatabase.Refresh ();

		//isDone = true;


	}

		
}
