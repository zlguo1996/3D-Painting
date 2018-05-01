using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class instanceStream : MonoBehaviour {

	//public Mesh streamMesh;
	public Vector3[] _vertices;
	public Color[] _colors;
	Mesh tmpMesh;

	// Use this for initialization
	void Start () {

		Apply ();
	}

	public void rebuildForTerrain() {
		GetComponent<MeshFilter>().sharedMesh.colors = _colors;

	}

	public void Apply() {

		if (tmpMesh == null) {
			tmpMesh = new Mesh ();
		}
		tmpMesh.vertices = _vertices;
		tmpMesh.colors = _colors;

		GetComponent<MeshRenderer> ().additionalVertexStreams = tmpMesh;

	}


	#if UNITY_EDITOR
	// Update is called once per frame
	private MeshRenderer _meshRenderer;
	void Update () {
		
		if (_meshRenderer == null)
			_meshRenderer = GetComponent<MeshRenderer> ();

		//Apply ();

		_meshRenderer.additionalVertexStreams = tmpMesh;
	}
	#endif

}
