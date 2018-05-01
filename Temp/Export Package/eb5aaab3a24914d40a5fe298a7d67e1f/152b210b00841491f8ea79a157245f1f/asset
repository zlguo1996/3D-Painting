using UnityEngine;
using UnityEditor;
using System.Collections;

public static class VPP_Utils {

	public static Mesh GetMesh(GameObject go) {

		Mesh curMesh = null;

		if (go) {

			MeshFilter curFilter = go.GetComponent<MeshFilter>();
			SkinnedMeshRenderer curSkinned = go.GetComponent<SkinnedMeshRenderer>();

			if( curFilter && !curSkinned ) {

				curMesh = curFilter.sharedMesh;

			}

			if( !curFilter && curSkinned ) {

				curMesh = curSkinned.sharedMesh;

			}

		}

		return curMesh;

	}

	public static void SetMesh(GameObject go, Mesh mesh) {
		

		if (go) {
			
			MeshFilter curFilter = go.GetComponent<MeshFilter>();
			SkinnedMeshRenderer curSkinned = go.GetComponent<SkinnedMeshRenderer>();
			
			if( curFilter && !curSkinned ) {
				
				curFilter.sharedMesh = mesh;
				
			}
			
			if( !curFilter && curSkinned ) {
				
				curSkinned.sharedMesh = mesh;
				
			}
			
		}
		

	}

	public static float linearFalloff(float distance, float brushRadius) {

		return Mathf.Pow( Mathf.Clamp01( 1-distance / brushRadius ), 2);

	}

	// Lerping method
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
