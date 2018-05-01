using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VertexColorAnimator : MonoBehaviour {

	public List<MeshHolder> animationMeshes;
	public List<float> animationKeyframes;

	public float timeScale = 2f;
	public int mode = 0;
	float elapsedTime = 0f;


	public void initLists() {

		animationMeshes = new List<MeshHolder> ();
		animationKeyframes = new List<float> ();

	}

	public void addMesh(Mesh mesh, float atPosition) {


		MeshHolder newMeshHolder = new MeshHolder ();
		newMeshHolder.setAnimationData (mesh);

		animationMeshes.Add (newMeshHolder);
		animationKeyframes.Add (atPosition);


	}

	// Use this for initialization
	void Start () {
		elapsedTime = 0f;
	}

	public void replaceKeyframe( int frameIndex, Mesh mesh) {

		animationMeshes [frameIndex].setAnimationData (mesh);

	}

	public void deleteKeyframe( int frameIndex ) {

		animationMeshes.RemoveAt (frameIndex);
		animationKeyframes.RemoveAt (frameIndex);
	}

	public void scrobble( float scrobblePos ) {

		if (animationMeshes.Count == 0)
			return;

		Color[] currentColors = new Color[GetComponent<MeshFilter> ().sharedMesh.colors.Length];

		int currentAnimationIndex = 0;
		for (int i = 0; i < animationKeyframes.Count; i++) {

			if (scrobblePos >= animationKeyframes [i])
				currentAnimationIndex = i;

		}


		if (currentAnimationIndex >= animationKeyframes.Count - 1) {
			GetComponent<VertexColorStream> ().setColors (animationMeshes [currentAnimationIndex]._colors);
			return;
		}


		//Debug.Log ("Current Animation: " + currentAnimationIndex);

		float animationClipLength = animationKeyframes [currentAnimationIndex + 1] - animationKeyframes [currentAnimationIndex];
		float animationClipStart = animationKeyframes [currentAnimationIndex];
		float animationClipProgess = (scrobblePos - animationClipStart) / animationClipLength;


		for (int i = 0; i < currentColors.Length; i++) {

			currentColors [i] = Color.Lerp (animationMeshes [currentAnimationIndex]._colors[i], animationMeshes [currentAnimationIndex+1]._colors[i], animationClipProgess);

		}

		GetComponent<VertexColorStream> ().setColors (currentColors);

	}
	
	// Update is called once per frame
	void Update () {
	
		if (mode == 0) {
			elapsedTime += Time.fixedDeltaTime / timeScale;
		} else if (mode == 1) {
			elapsedTime += Time.fixedDeltaTime / timeScale;
			if (elapsedTime > 1f)
				elapsedTime = 0f;
		} else if (mode == 2) {

			if (Mathf.FloorToInt (Time.fixedTime / timeScale) % 2 == 0) {
				elapsedTime += Time.fixedDeltaTime / timeScale;
			} else {
				elapsedTime -= Time.fixedDeltaTime / timeScale;
			}
				

		}

		Color[] currentColors = new Color[GetComponent<MeshFilter> ().sharedMesh.colors.Length];

		int currentAnimationIndex = 0;
		for (int i = 0; i < animationKeyframes.Count; i++) {

			if (elapsedTime >= animationKeyframes [i])
				currentAnimationIndex = i;

		}


		if (currentAnimationIndex < animationKeyframes.Count - 1) {
			
			//Debug.Log ("Current Animation: " + currentAnimationIndex);

			float animationClipLength = animationKeyframes [currentAnimationIndex + 1] - animationKeyframes [currentAnimationIndex];
			float animationClipStart = animationKeyframes [currentAnimationIndex];
			float animationClipProgess = (elapsedTime - animationClipStart) / animationClipLength;


			for (int i = 0; i < currentColors.Length; i++) {

				currentColors [i] = Color.Lerp (animationMeshes [currentAnimationIndex]._colors [i], animationMeshes [currentAnimationIndex + 1]._colors [i], animationClipProgess);

			}

		} else {
			currentColors = animationMeshes [currentAnimationIndex]._colors;

		}
		GetComponent<VertexColorStream> ().setColors (currentColors);

	}
}
