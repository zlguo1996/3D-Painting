using UnityEngine;
using System.Collections;

public class VTP : MonoBehaviour {



	public static Color getSingleVertexColorAtHit( Transform transform, RaycastHit hit ) {

		Vector3[] currentVertices = transform.GetComponent<MeshFilter> ().sharedMesh.vertices;
		int[] currentTriangles = transform.GetComponent<MeshFilter> ().sharedMesh.triangles;
		Color[] currentColors = transform.GetComponent<MeshFilter> ().sharedMesh.colors;
		int triangle = hit.triangleIndex;

		float shortestDistance = Mathf.Infinity;
		int colorIndex = 0;
		for (int i = 0 ; i < 3 ; i++) {


			Vector3 vertPos = transform.TransformPoint(currentVertices[  currentTriangles[triangle*3 + i] ]);
			float sqrMag = Vector3.Distance(vertPos, hit.point);

			if (sqrMag < shortestDistance) {
				colorIndex = currentTriangles[triangle*3 + i];
				shortestDistance = sqrMag;
			}
		}


		return currentColors [colorIndex];

	}

	public static Color getFaceVerticesColorAtHit( Transform transform, RaycastHit hit ) {

		int[] currentTriangles = transform.GetComponent<MeshFilter> ().sharedMesh.triangles;
		Color[] currentColors = transform.GetComponent<MeshFilter> ().sharedMesh.colors;
		int triangle = hit.triangleIndex;


		int colorTriangleIndex = currentTriangles[triangle*3];
		Color averageColor = currentColors [colorTriangleIndex];
		averageColor += currentColors [colorTriangleIndex+1];
		averageColor += currentColors [colorTriangleIndex+2];
		averageColor /= 3f;


		return averageColor;

	}


	public static void paintSingleVertexOnHit( Transform transform, RaycastHit hit, Color color, float strength) {

		Vector3[] currentVertices = transform.GetComponent<MeshFilter> ().sharedMesh.vertices;
		int[] currentTriangles = transform.GetComponent<MeshFilter> ().sharedMesh.triangles;
		Color[] currentColors = transform.GetComponent<MeshFilter> ().sharedMesh.colors;
		int triangle = hit.triangleIndex;

		float shortestDistance = Mathf.Infinity;
		int paintIndex = 0;
		for (int i = 0 ; i < 3 ; i=i+3) {


			Vector3 vertPos = transform.TransformPoint(currentVertices[  currentTriangles[triangle*3 + i] ]);
			float sqrMag = Vector3.Distance(vertPos, hit.point);

			if (sqrMag < shortestDistance) {
				paintIndex = currentTriangles[triangle*3 + i];
				shortestDistance = sqrMag;
			}
		}

		Color newColor = VertexColorLerp (currentColors [paintIndex], color, strength);

		currentColors [paintIndex] = newColor;
		transform.GetComponent<MeshFilter> ().sharedMesh.colors = currentColors;


	}

	public static void paintFaceVerticesOnHit( Transform transform, RaycastHit hit, Color color, float strength) {

		int[] currentTriangles = transform.GetComponent<MeshFilter> ().sharedMesh.triangles;
		Color[] currentColors = transform.GetComponent<MeshFilter> ().sharedMesh.colors;
		int triangle = hit.triangleIndex;

		int paintIndex = 0;
		for (int i = 0 ; i < 3 ; i++) {

			paintIndex = currentTriangles[triangle*3 + i];
			Color newColor = VertexColorLerp (currentColors [paintIndex], color, strength);
			currentColors [paintIndex] = newColor;
		
		}


		transform.GetComponent<MeshFilter> ().sharedMesh.colors = currentColors;


	}

	public static void deformVerticesOnHit( RaycastHit hit, bool up, float strength, float radius, bool linearFalloff, bool recalculateNormals, bool recalculateCollider, bool recalculateFlow) {

		Transform hittedTransform = hit.transform;

		Vector3[] currentVertices = hittedTransform.GetComponent<MeshFilter> ().sharedMesh.vertices;
		Vector3[] currentNormals = hittedTransform.GetComponent<MeshFilter> ().sharedMesh.normals;
		int[] currentTriangles = hittedTransform.GetComponent<MeshFilter> ().sharedMesh.triangles;

		int direction = 1;
		if (!up)
			direction = -1;

		for (int i = 0 ; i < currentVertices.Length ; i++) {


			Vector3 vertPos = hit.transform.TransformPoint(currentVertices[ i ]);
			float sqrMag = Vector3.Distance(vertPos, hit.point);


			if (sqrMag < radius) {
				float falloff = 0;
				if (linearFalloff) {
					falloff = (radius - sqrMag) / radius;
				} else {
					falloff = Mathf.Pow ((radius - sqrMag) / radius, 2);
				}

				currentVertices[i] += direction * 0.1f * strength * currentNormals[i] * falloff;
			}
		}




		hittedTransform.GetComponent<MeshFilter> ().sharedMesh.vertices = currentVertices;

		if (recalculateNormals)
			hittedTransform.GetComponent<MeshFilter> ().sharedMesh.RecalculateNormals ();

		if (recalculateCollider)
			hittedTransform.GetComponent<MeshCollider> ().sharedMesh = hittedTransform.GetComponent<MeshFilter> ().sharedMesh;

		if (recalculateFlow) {
			Vector4[] tmpTangents = calculateMeshTangents(currentTriangles, currentVertices, hittedTransform.GetComponent<MeshCollider> ().sharedMesh.uv, currentNormals);
			hittedTransform.GetComponent<MeshCollider> ().sharedMesh.tangents = tmpTangents;
			recalculateMeshForFlow (hittedTransform, currentVertices, currentNormals, tmpTangents);

		}


	}


	public static void deformSingleVertexOnHit( Transform transform, RaycastHit hit, bool up, float strength, bool recalculateNormals, bool recalculateCollider, bool recalculateFlow) {

		Vector3[] currentVertices = transform.GetComponent<MeshFilter> ().sharedMesh.vertices;
		int[] currentTriangles = transform.GetComponent<MeshFilter> ().sharedMesh.triangles;
		Vector3[] currentNormals = transform.GetComponent<MeshFilter> ().sharedMesh.normals;
		int triangle = hit.triangleIndex;

		float shortestDistance = Mathf.Infinity;
		int deformIndex = 0;
		for (int i = 0 ; i < 3 ; i++) {


			Vector3 vertPos = transform.TransformPoint(currentVertices[  currentTriangles[triangle*3 + i] ]);
			float sqrMag = Vector3.Distance(vertPos, hit.point);

			if (sqrMag < shortestDistance) {
				deformIndex = currentTriangles[triangle*3 + i];
				shortestDistance = sqrMag;
			}
		}
		int direction = 1;
		if (!up)
			direction = -1;


		currentVertices[deformIndex] += direction * 0.1f * strength * currentNormals[deformIndex];

		transform.GetComponent<MeshFilter> ().sharedMesh.vertices = currentVertices;

		if (recalculateNormals)
			transform.GetComponent<MeshFilter> ().sharedMesh.RecalculateNormals ();

		if (recalculateCollider)
			transform.GetComponent<MeshCollider> ().sharedMesh = transform.GetComponent<MeshFilter> ().sharedMesh;

		if (recalculateFlow) {
			Vector4[] tmpTangents = calculateMeshTangents(currentTriangles, currentVertices, transform.GetComponent<MeshCollider> ().sharedMesh.uv, currentNormals);
			transform.GetComponent<MeshCollider> ().sharedMesh.tangents = tmpTangents;
			recalculateMeshForFlow (transform, currentVertices, currentNormals, tmpTangents);

		}


	}


	public static void deformFaceVerticesOnHit( Transform transform, RaycastHit hit, bool up, float strength, bool recalculateNormals, bool recalculateCollider, bool recalculateFlow) {

		Vector3[] currentVertices = transform.GetComponent<MeshFilter> ().sharedMesh.vertices;
		int[] currentTriangles = transform.GetComponent<MeshFilter> ().sharedMesh.triangles;
		Vector3[] currentNormals = transform.GetComponent<MeshFilter> ().sharedMesh.normals;
		int triangle = hit.triangleIndex;

		int deformIndex = 0;

		int direction = 1;
		if (!up)
			direction = -1;

		for (int i = 0 ; i < 3 ; i++) {

			deformIndex = currentTriangles[triangle*3 + i];
			currentVertices[deformIndex] += direction * 0.1f * strength * currentNormals[deformIndex];

		}


		transform.GetComponent<MeshFilter> ().sharedMesh.vertices = currentVertices;

		if (recalculateNormals)
			transform.GetComponent<MeshFilter> ().sharedMesh.RecalculateNormals ();

		if (recalculateCollider)
			transform.GetComponent<MeshCollider> ().sharedMesh = transform.GetComponent<MeshFilter> ().sharedMesh;

		if (recalculateFlow) {
			Vector4[] tmpTangents = calculateMeshTangents(currentTriangles, currentVertices, transform.GetComponent<MeshCollider> ().sharedMesh.uv, currentNormals);
			transform.GetComponent<MeshCollider> ().sharedMesh.tangents = tmpTangents;
			recalculateMeshForFlow (transform, currentVertices, currentNormals, tmpTangents);

		}


	}



	private static void recalculateMeshForFlow(Transform transform, Vector3[] currentVertices, Vector3[] currentNormals, Vector4[] currentTangents) {


		Vector2[] currentUV4s = transform.GetComponent<MeshFilter> ().sharedMesh.uv4;

		for (int i = 0; i < currentVertices.Length; i++) {

			Vector3 binormal = transform.TransformDirection( Vector3.Cross (currentNormals [i], new Vector3(currentTangents[i].x, currentTangents[i].y, currentTangents[i].z) ).normalized * currentTangents[i].w );
			Vector3 tangent = transform.TransformDirection( currentTangents [i].normalized );

			float xFlow = 0.5f + 0.5f * tangent.y;
			float yFlow = 0.5f + 0.5f * binormal.y;

			currentUV4s [i] = new Vector2 (xFlow, yFlow);

		}

		transform.GetComponent<MeshFilter> ().sharedMesh.uv4 = currentUV4s;

	}


	/*
	 * http://answers.unity3d.com/questions/7789/calculating-tangents-vector4.html
	 * 
	 */

	private static Vector4[] calculateMeshTangents(int[] triangles, Vector3[] vertices, Vector2[] uv, Vector3[] normals)
	{



		//variable definitions
		int triangleCount = triangles.Length;
		int vertexCount = vertices.Length;

		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];

		Vector4[] tangents = new Vector4[vertexCount];

		for (long a = 0; a < triangleCount; a += 3)
		{
			long i1 = triangles[a + 0];
			long i2 = triangles[a + 1];
			long i3 = triangles[a + 2];

			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];

			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];

			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;

			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;

			float div = s1 * t2 - s2 * t1;
			float r = div == 0.0f ? 0.0f : 1.0f / div;

			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;

			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}


		for (long a = 0; a < vertexCount; ++a)
		{
			Vector3 n = normals[a];
			Vector3 t = tan1[a];

			//Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
			//tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
			Vector3.OrthoNormalize(ref n, ref t);
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;

			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}

		return tangents;
	}





	public static Color VertexColorLerp(Color colorA, Color colorB, float value) {

		if (value >= 1f) {
			return colorB;
		}
		if( value <= 0f ) {
			return colorA;
		}

		return new Color (colorA.r + (colorB.r - colorA.r) * value, 
			colorA.g + (colorB.g - colorA.g) * value, 
			colorA.b + (colorB.b - colorA.b) * value, 
			colorA.a + (colorB.a - colorA.a) * value);


	}



}
