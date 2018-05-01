using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class VertexColorStream_AVS : MonoBehaviour {

	public bool isStaticGameObject = false;

	[HideInInspector]
	public Color[] _vertexColors;

	[HideInInspector]
	public Vector3[] _meshVertices;

	[HideInInspector]
	public Vector2[] _uv;

	[HideInInspector]
	public Vector2[] _uv2;

	[HideInInspector]
	public Vector2[] _uv3;

	[HideInInspector]
	public Vector2[] _uv4;



	// Use this for initialization
	void Start () {

		Upload ();

	}
		
	public void Upload()
	{

		MeshRenderer mr = GetComponent<MeshRenderer>();
		MeshFilter mf = GetComponent<MeshFilter>();

		if (mr != null && mf != null)
		{
			Mesh m = new Mesh();
			m.hideFlags = HideFlags.HideAndDontSave;
			m.vertices = _meshVertices;
			m.colors = _vertexColors;


			m.uv = _uv;
			m.uv2 = _uv2;
			m.uv3 = _uv3;
			m.uv4 = _uv4;
				


			mr.additionalVertexStreams = m;
			m.UploadMeshData (true);


			#if UNITY_EDITOR
			stream = m;
			#endif
		}
	}

	#if UNITY_EDITOR
	private Mesh stream;
	void Update() {

		if (!Application.isPlaying)
		{
			if (isStaticGameObject) {

				gameObject.isStatic = true;
				GameObjectUtility.SetStaticEditorFlags (gameObject, ~(StaticEditorFlags.BatchingStatic));

			} else {
				gameObject.isStatic = false;
			}



			MeshRenderer r = GetComponent<MeshRenderer>();
			r.additionalVertexStreams = stream;
		}
	}
	#endif


}
